using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Helpers;
using FibiEmlakDanismanlik.Application.Interfaces.FibiServiceInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.MapInterfaces;
using FibiEmlakDanismanlik.Application.Models.FibiServiceModels;
using FibiEmlakDanismanlik.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Infrastructure.MarketData.Providers
{
    /// <summary>Koordinatı olmayan ilanlara OpenStreetMap Nominatim servisi üzerinden yaklaşık konum (mahalle > ilçe > il) bulan sağlayıcı.</summary>
    public class ListingGeocodeProvider : IMarketDataProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMapRepository _mapRepository;

        public ListingGeocodeProvider(
            IHttpClientFactory httpClientFactory,
            IMapRepository mapRepository)
        {
            _httpClientFactory = httpClientFactory;
            _mapRepository = mapRepository;
        }

        public string Key => ListingGeoConstants.GeocodeProviderKey;

        public TimeSpan MinimumInterval => TimeSpan.FromHours(6);

        public int Order => 40;

        public async Task<MarketDataSyncOutcome> SyncAsync(CancellationToken cancellationToken)
        {
            var candidates = await _mapRepository.GetListingsWithoutGeoPointAsync(ListingGeoConstants.GeocodeCandidateLimit);
            if (candidates == null || candidates.Count == 0)
            {
                return MarketDataSyncOutcome.Success(0, "Koordinatı eksik ilan yok.");
            }

            var groups = candidates
                .GroupBy(c => (
                    CityKey: TurkishTextHelper.ToComparisonKey(c.City),
                    DistrictKey: TurkishTextHelper.ToComparisonKey(c.District),
                    NeighborhoodKey: TurkishTextHelper.ToComparisonKey(c.Neighborhood)
                ))
                .Select(g => g.ToList())
                .ToList();

            var queryCache = new Dictionary<string, (decimal Latitude, decimal Longitude)?>(StringComparer.OrdinalIgnoreCase);
            var pointsToAdd = new List<ListingGeoPoint>();

            int neighborhoodCount = 0;
            int districtCount = 0;
            int cityCount = 0;
            int notFoundGroups = 0;
            int remainingGroups = 0;
            int requestCount = 0;
            int? rateLimitedStatusCode = null;

            var client = _httpClientFactory.CreateClient(FibiMarketDataSyncConstants.HttpClientName);

            for (int g = 0; g < groups.Count; g++)
            {
                var group = groups[g];
                var representative = group[0];

                var tiers = BuildTiers(representative.City, representative.District, representative.Neighborhood);
                if (tiers.Count == 0)
                {
                    notFoundGroups++;
                    continue;
                }

                (decimal Latitude, decimal Longitude)? foundCoord = null;
                string? foundPrecision = null;
                bool groupInterrupted = false;

                foreach (var (precision, query) in tiers)
                {
                    if (queryCache.TryGetValue(query, out var cached))
                    {
                        if (cached.HasValue)
                        {
                            foundCoord = cached.Value;
                            foundPrecision = precision;
                            break;
                        }

                        // Daha önceki istekte sonuç bulunamamış veya hata alınmış, sonraki kademeye geç
                        continue;
                    }

                    // Önbellekte yoksa yeni HTTP isteği gerekiyor; istek limiti kontrolü
                    if (requestCount >= ListingGeoConstants.GeocodeMaxRequestsPerRun)
                    {
                        groupInterrupted = true;
                        break;
                    }

                    requestCount++;
                    var fetchResult = await ExecuteNominatimSearchAsync(client, query, cancellationToken);

                    if (fetchResult.IsRateLimited)
                    {
                        rateLimitedStatusCode = fetchResult.StatusCode;
                        groupInterrupted = true;
                        break;
                    }

                    if (fetchResult.IsSuccess)
                    {
                        queryCache[query] = (fetchResult.Latitude, fetchResult.Longitude);
                        foundCoord = (fetchResult.Latitude, fetchResult.Longitude);
                        foundPrecision = precision;
                        break;
                    }

                    // Sonuç bulunamadı veya hata oluştu; bu sorguyu null olarak önbelleğe alıp sonraki kademeye geç
                    queryCache[query] = null;
                }

                if (groupInterrupted)
                {
                    // İstek limiti dolduğu veya servis sınırladığı için bu grup ve sonraki tüm gruplar bir sonraki çalıştırmaya kalır
                    remainingGroups = groups.Count - g;
                    break;
                }

                if (foundCoord.HasValue && foundPrecision != null)
                {
                    foreach (var candidate in group)
                    {
                        pointsToAdd.Add(new ListingGeoPoint
                        {
                            ListingId = candidate.ListingId,
                            ListingTypeId = candidate.ListingTypeId,
                            Latitude = foundCoord.Value.Latitude,
                            Longitude = foundCoord.Value.Longitude,
                            Source = ListingGeoConstants.SourceNominatim,
                            Precision = foundPrecision,
                            CreatedDate = DateTime.UtcNow
                        });
                    }

                    if (foundPrecision == ListingGeoConstants.PrecisionNeighborhood)
                        neighborhoodCount += group.Count;
                    else if (foundPrecision == ListingGeoConstants.PrecisionDistrict)
                        districtCount += group.Count;
                    else if (foundPrecision == ListingGeoConstants.PrecisionCity)
                        cityCount += group.Count;
                }
                else
                {
                    notFoundGroups++;
                }
            }

            int eklenen = 0;
            if (pointsToAdd.Count > 0)
            {
                eklenen = await _mapRepository.AddGeoPointsAsync(pointsToAdd);
            }

            if (rateLimitedStatusCode.HasValue && eklenen == 0)
            {
                return MarketDataSyncOutcome.Failure($"Konum servisi istekleri sınırladı (HTTP {rateLimitedStatusCode.Value}).");
            }

            return MarketDataSyncOutcome.Success(
                eklenen,
                $"{eklenen} ilana yaklaşık konum eklendi (mahalle: {neighborhoodCount}, ilçe: {districtCount}, il: {cityCount}); bulunamayan grup: {notFoundGroups}, sonraki çalıştırmaya kalan grup: {remainingGroups}.");
        }

        private static List<(string Precision, string Query)> BuildTiers(string? city, string? district, string? neighborhood)
        {
            var tiers = new List<(string Precision, string Query)>(3);

            var cleanNeighborhood = CleanNeighborhood(neighborhood);
            var trimmedDistrict = district?.Trim();
            var trimmedCity = city?.Trim();

            // 1. Kademe: Mahalle (boş değilse)
            if (!string.IsNullOrWhiteSpace(cleanNeighborhood))
            {
                var parts = new List<string> { cleanNeighborhood };
                if (!string.IsNullOrWhiteSpace(trimmedDistrict))
                    parts.Add(trimmedDistrict);
                if (!string.IsNullOrWhiteSpace(trimmedCity))
                    parts.Add(trimmedCity);
                parts.Add(ListingGeoConstants.CountryName);

                tiers.Add((ListingGeoConstants.PrecisionNeighborhood, string.Join(", ", parts)));
            }

            // 2. Kademe: İlçe (boş değilse)
            if (!string.IsNullOrWhiteSpace(trimmedDistrict))
            {
                var parts = new List<string> { trimmedDistrict };
                if (!string.IsNullOrWhiteSpace(trimmedCity))
                    parts.Add(trimmedCity);
                parts.Add(ListingGeoConstants.CountryName);

                tiers.Add((ListingGeoConstants.PrecisionDistrict, string.Join(", ", parts)));
            }

            // 3. Kademe: İl (boş değilse)
            if (!string.IsNullOrWhiteSpace(trimmedCity))
            {
                var query = $"{trimmedCity}, {ListingGeoConstants.CountryName}";
                tiers.Add((ListingGeoConstants.PrecisionCity, query));
            }

            return tiers;
        }

        private static string CleanNeighborhood(string? neighborhood)
        {
            if (string.IsNullOrWhiteSpace(neighborhood))
                return string.Empty;

            var trimmed = neighborhood.Trim();
            var trCulture = new CultureInfo("tr-TR");

            const string suffixWithSpace = " Mahallesi";
            if (trimmed.EndsWith(suffixWithSpace, true, trCulture) ||
                trimmed.EndsWith(" MAHALLESİ", StringComparison.OrdinalIgnoreCase) ||
                trimmed.EndsWith(suffixWithSpace, StringComparison.OrdinalIgnoreCase))
            {
                return trimmed.Substring(0, trimmed.Length - 10).Trim();
            }

            const string suffixWithoutSpace = "Mahallesi";
            if (trimmed.EndsWith(suffixWithoutSpace, true, trCulture) ||
                trimmed.EndsWith("MAHALLESİ", StringComparison.OrdinalIgnoreCase) ||
                trimmed.EndsWith(suffixWithoutSpace, StringComparison.OrdinalIgnoreCase))
            {
                return trimmed.Substring(0, trimmed.Length - 9).Trim();
            }

            return trimmed;
        }

        private static async Task<GeocodeFetchResult> ExecuteNominatimSearchAsync(
            HttpClient client,
            string query,
            CancellationToken cancellationToken)
        {
            var requestUrl = $"{ListingGeoConstants.NominatimSearchUrl}?q={Uri.EscapeDataString(query)}&format=jsonv2&limit=1&countrycodes={ListingGeoConstants.NominatimCountryCode}&accept-language=tr";

            try
            {
                using var response = await client.GetAsync(requestUrl, cancellationToken);
                var statusCode = (int)response.StatusCode;

                // Her HTTP isteğinden sonra Nominatim kullanım politikasına uygun gecikme
                await Task.Delay(ListingGeoConstants.GeocodeRequestDelayMilliseconds, cancellationToken);

                if (statusCode == 429 || statusCode == 403)
                {
                    return GeocodeFetchResult.RateLimited(statusCode);
                }

                if (!response.IsSuccessStatusCode)
                {
                    return GeocodeFetchResult.Failure(statusCode);
                }

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                using var doc = JsonDocument.Parse(json);

                if (doc.RootElement.ValueKind != JsonValueKind.Array || doc.RootElement.GetArrayLength() == 0)
                {
                    return GeocodeFetchResult.NotFound();
                }

                var first = doc.RootElement[0];
                if (!first.TryGetProperty("lat", out var latProp) || !first.TryGetProperty("lon", out var lonProp))
                {
                    return GeocodeFetchResult.NotFound();
                }

                var latStr = latProp.GetString();
                var lonStr = lonProp.GetString();
                if (string.IsNullOrWhiteSpace(latStr) || string.IsNullOrWhiteSpace(lonStr))
                {
                    return GeocodeFetchResult.NotFound();
                }

                if (!decimal.TryParse(latStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var lat) ||
                    !decimal.TryParse(lonStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var lon))
                {
                    return GeocodeFetchResult.NotFound();
                }

                lat = Math.Round(lat, 6, MidpointRounding.AwayFromZero);
                lon = Math.Round(lon, 6, MidpointRounding.AwayFromZero);

                return GeocodeFetchResult.Success(lat, lon);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                // Tek bir istek hata verirse o kademeyi başarısız say ve bir sonraki kademeye geç; istisnayı yukarı fırlatma.
                return GeocodeFetchResult.Failure(0);
            }
        }

        private readonly struct GeocodeFetchResult
        {
            public bool IsSuccess { get; }
            public bool IsRateLimited { get; }
            public int StatusCode { get; }
            public decimal Latitude { get; }
            public decimal Longitude { get; }

            private GeocodeFetchResult(bool isSuccess, bool isRateLimited, int statusCode, decimal lat, decimal lon)
            {
                IsSuccess = isSuccess;
                IsRateLimited = isRateLimited;
                StatusCode = statusCode;
                Latitude = lat;
                Longitude = lon;
            }

            public static GeocodeFetchResult Success(decimal lat, decimal lon) =>
                new(true, false, 200, lat, lon);

            public static GeocodeFetchResult NotFound() =>
                new(false, false, 200, 0, 0);

            public static GeocodeFetchResult Failure(int statusCode) =>
                new(false, false, statusCode, 0, 0);

            public static GeocodeFetchResult RateLimited(int statusCode) =>
                new(false, true, statusCode, 0, 0);
        }
    }
}
