using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Helpers;
using FibiEmlakDanismanlik.Application.Interfaces.FibiServiceInterfaces;
using FibiEmlakDanismanlik.Application.Models.FibiServiceModels;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;
using Microsoft.Extensions.Options;
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
    /// <summary>TCMB EVDS sisteminden konut birim satış ve kira fiyatlarını çeken sağlayıcı.</summary>
    public class EvdsUnitPriceProvider : IMarketDataProvider
    {
        private const int CountryKey = 0;

        private static readonly Dictionary<int, (string PriceSuffix, string RentSuffix)> SeriesCodeMap = new()
        {
            [CountryKey] = ("TR", "TR"),
            [34] = ("IST", "ISTANBUL"),
            [6] = ("ANK", "ANKARA"),
            [35] = ("IZM", "IZMIR"),
            [1] = ("ADANA", "ADANA"),
            [2] = ("ADIYAMAN", "ADIYAMAN"),
            [3] = ("AFYON", "AFYONKARAHISAR"),
            [4] = ("AGRI", "AGRI"),
            [5] = ("AMASYA", "AMASYA"),
            [7] = ("ANTALYA", "ANTALYA"),
            [8] = ("ARTVIN", "ARTVIN"),
            [9] = ("AYDIN", "AYDIN"),
            [10] = ("BALIKESIR", "BALIKESIR"),
            [11] = ("BILECIK", "BILECIK"),
            [12] = ("BINGOL", "BINGOL"),
            [13] = ("BITLIS", "BITLIS"),
            [14] = ("BOLU", "BOLU"),
            [15] = ("BURDUR", "BURDUR"),
            [16] = ("BURSA", "BURSA"),
            [17] = ("CANAKKALE", "CANAKKALE"),
            [18] = ("CANKIRI", "CANKIRI"),
            [19] = ("CORUM", "CORUM"),
            [20] = ("DENIZLI", "DENIZLI"),
            [21] = ("DIYARBAKIR", "DIYARBAKIR"),
            [22] = ("EDIRNE", "EDIRNE"),
            [23] = ("ELAZIG", "ELAZIG"),
            [24] = ("ERZINCAN", "ERZINCAN"),
            [25] = ("ERZURUM", "ERZURUM"),
            [26] = ("ESKISEHIR", "ESKISEHIR"),
            [27] = ("ANTEP", "GAZIANTEP"),
            [28] = ("GIRESUN", "GIRESUN"),
            [29] = ("GUMUSHANE", "GUMUSHANE"),
            [30] = ("HAKKARI", "HAKKARI"),
            [31] = ("HATAY", "HATAY"),
            [32] = ("ISPARTA", "ISPARTA"),
            [33] = ("MERSIN", "MERSIN"),
            [36] = ("KARS", "KARS"),
            [37] = ("KASTAMONU", "KASTAMONU"),
            [38] = ("KAYSERI", "KAYSERI"),
            [39] = ("KIRKLARELI", "KIRKLARELI"),
            [40] = ("KIRSEHIR", "KIRSEHIR"),
            [41] = ("KOCAELI", "KOCAELI"),
            [42] = ("KONYA", "KONYA"),
            [43] = ("KUTAHYA", "KUTAHYA"),
            [44] = ("MALATYA", "MALATYA"),
            [45] = ("MANISA", "MANISA"),
            [46] = ("MARAS", "KAHRAMANMARAS"),
            [47] = ("MARDIN", "MARDIN"),
            [48] = ("MUGLA", "MUGLA"),
            [49] = ("MUS", "MUS"),
            [50] = ("NEVSEHIR", "NEVSEHIR"),
            [51] = ("NIGDE", "NIGDE"),
            [52] = ("ORDU", "ORDU"),
            [53] = ("RIZE", "RIZE"),
            [54] = ("SAKARYA", "SAKARYA"),
            [55] = ("SAMSUN", "SAMSUN"),
            [56] = ("SIIRT", "SIIRT"),
            [57] = ("SINOP", "SINOP"),
            [58] = ("SIVAS", "SIVAS"),
            [59] = ("TEKIRDAG", "TEKIRDAG"),
            [60] = ("TOKAT", "TOKAT"),
            [61] = ("TRABZON", "TRABZON"),
            [62] = ("TUNCELI", "TUNCELI"),
            [63] = ("URFA", "SANLIURFA"),
            [64] = ("USAK", "USAK"),
            [65] = ("VAN", "VAN"),
            [66] = ("YOZGAT", "YOZGAT"),
            [67] = ("ZONGULDAK", "ZONGULDAK"),
            [68] = ("AKSARAY", "AKSARAY"),
            [69] = ("BAYBURT", "BAYBURT"),
            [70] = ("KARAMAN", "KARAMAN"),
            [71] = ("KIRIKKALE", "KIRIKKALE"),
            [72] = ("BATMAN", "BATMAN"),
            [73] = ("SIRNAK", "SIRNAK"),
            [74] = ("BARTIN", "BARTIN"),
            [75] = ("ARDAHAN", "ARDAHAN"),
            [76] = ("IGDIR", "IGDIR"),
            [77] = ("YALOVA", "YALOVA"),
            [78] = ("KARABUK", "KARABUK"),
            [79] = ("KILIS", "KILIS"),
            [80] = ("OSMANIYE", "OSMANIYE"),
            [81] = ("DUZCE", "DUZCE")
        };

        private readonly IOptions<MarketDataSyncOptions> _options;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMarketDataSyncRepository _repository;

        public EvdsUnitPriceProvider(
            IOptions<MarketDataSyncOptions> options,
            IHttpClientFactory httpClientFactory,
            IMarketDataSyncRepository repository)
        {
            _options = options;
            _httpClientFactory = httpClientFactory;
            _repository = repository;
        }

        public string Key => FibiMarketDataSyncConstants.EvdsProviderKey;

        public TimeSpan MinimumInterval => TimeSpan.FromDays(7);

        public int Order => 15;

        public async Task<MarketDataSyncOutcome> SyncAsync(CancellationToken cancellationToken)
        {
            var apiKey = _options.Value.EvdsApiKey;
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return MarketDataSyncOutcome.Skipped("EVDS API anahtarı tanımlı değil; sağlayıcı devre dışı.");
            }

            var tracked = await _repository.GetTrackedStatsWithCityAsync();
            var relevantLocations = new HashSet<int>();

            foreach (var item in tracked)
            {
                if (item.Stat.Scope == MarketStatScope.Country)
                {
                    relevantLocations.Add(CountryKey);
                }
                else if (item.Stat.Scope == MarketStatScope.City && item.CityPlaceCode.HasValue)
                {
                    relevantLocations.Add(item.CityPlaceCode.Value);
                }
            }

            var seriesCodes = new List<string>();
            foreach (var loc in relevantLocations)
            {
                if (SeriesCodeMap.TryGetValue(loc, out var suffixes))
                {
                    seriesCodes.Add($"TP.BIRIMFIYAT.{suffixes.PriceSuffix}");
                    seriesCodes.Add($"TP.BK.{suffixes.RentSuffix}");
                }
            }

            seriesCodes = seriesCodes.Distinct().ToList();
            if (seriesCodes.Count == 0)
            {
                return MarketDataSyncOutcome.Skipped("EVDS için sorgulanacak konum bulunamadı.");
            }

            var client = _httpClientFactory.CreateClient(FibiMarketDataSyncConstants.HttpClientName);
            var now = DateTime.Now;
            var lastYear = now.Year - 1;
            var thisYear = now.Year;
            var baseUrl = _options.Value.EvdsBaseUrl;
            if (!baseUrl.EndsWith("/"))
            {
                baseUrl += "/";
            }

            var latestValues = new Dictionary<string, (decimal Value, string? Tarih)>(StringComparer.OrdinalIgnoreCase);

            foreach (var chunk in seriesCodes.Chunk(40))
            {
                var seriesJoined = string.Join("-", chunk);
                var url = $"{baseUrl}series={seriesJoined}&startDate=01-01-{lastYear}&endDate=31-12-{thisYear}&type=json";

                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("key", apiKey);

                // Resmi EVDS API'si; anahtar girilince yanıt biçimi doğrulanmalıdır.
                using var response = await client.SendAsync(request, cancellationToken);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync(cancellationToken);
                using var jsonDoc = JsonDocument.Parse(jsonString);

                if (jsonDoc.RootElement.TryGetProperty("items", out var itemsElement) &&
                    itemsElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in itemsElement.EnumerateArray())
                    {
                        string? tarih = null;
                        if (item.TryGetProperty("Tarih", out var dateProp) && dateProp.ValueKind == JsonValueKind.String)
                        {
                            tarih = dateProp.GetString();
                        }

                        foreach (var code in chunk)
                        {
                            var propName = code.Replace('.', '_');
                            if (TryGetDecimal(item, propName, out var val))
                            {
                                latestValues[code] = (val, tarih);
                            }
                        }
                    }
                }
            }

            var unitValues = new List<CityUnitValue>();
            foreach (var loc in relevantLocations)
            {
                if (!SeriesCodeMap.TryGetValue(loc, out var suffixes))
                    continue;

                var priceCode = $"TP.BIRIMFIYAT.{suffixes.PriceSuffix}";
                var rentCode = $"TP.BK.{suffixes.RentSuffix}";

                if (latestValues.TryGetValue(priceCode, out var priceData) &&
                    latestValues.TryGetValue(rentCode, out var rentData))
                {
                    var rawPeriod = priceData.Tarih ?? rentData.Tarih;
                    var period = ResolvePeriod(rawPeriod, now);
                    int? placeCode = loc == CountryKey ? null : loc;
                    unitValues.Add(new CityUnitValue(placeCode, priceData.Value, rentData.Value, period));
                }
            }

            if (unitValues.Count == 0)
            {
                return MarketDataSyncOutcome.Failure("EVDS yanıtında kullanılabilir değer yok.");
            }

            var affectedCount = 0;
            foreach (var unitVal in unitValues)
            {
                if (unitVal.CityPlaceCode == null)
                {
                    var countryItem = tracked.FirstOrDefault(x => x.Stat.Scope == MarketStatScope.Country);
                    if (countryItem.Stat != null)
                    {
                        if (MarketIndexApplier.ApplyCityUnitValue(countryItem.Stat, Array.Empty<ExternalMarketStat>(), unitVal, now))
                        {
                            affectedCount++;
                        }
                    }
                }
                else
                {
                    var cityItem = tracked.FirstOrDefault(x => x.Stat.Scope == MarketStatScope.City && x.CityPlaceCode == unitVal.CityPlaceCode);
                    if (cityItem.Stat != null && cityItem.Stat.CityId.HasValue)
                    {
                        var cityId = cityItem.Stat.CityId.Value;
                        var children = tracked
                            .Where(x => x.Stat.CityId == cityId && (x.Stat.Scope == MarketStatScope.District || x.Stat.Scope == MarketStatScope.Neighborhood))
                            .Select(x => x.Stat)
                            .ToList();

                        if (MarketIndexApplier.ApplyCityUnitValue(cityItem.Stat, children, unitVal, now))
                        {
                            affectedCount += 1 + children.Count;
                        }
                    }
                }
            }

            if (affectedCount > 0)
            {
                await _repository.SaveChangesAsync();
            }

            return MarketDataSyncOutcome.Success(
                affectedCount,
                $"{unitValues.Count} konum için EVDS birim değerleri güncellendi ({affectedCount} kayıt).");
        }

        private static bool TryGetDecimal(JsonElement item, string propertyName, out decimal value)
        {
            value = 0m;
            if (!item.TryGetProperty(propertyName, out var prop))
                return false;

            if (prop.ValueKind == JsonValueKind.Number)
                return prop.TryGetDecimal(out value);

            if (prop.ValueKind == JsonValueKind.String)
            {
                var str = prop.GetString();
                if (string.IsNullOrWhiteSpace(str))
                    return false;

                var cleaned = str.Trim().Replace(',', '.');
                return decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
            }

            return false;
        }

        private static string ResolvePeriod(string? tarih, DateTime now)
        {
            if (!string.IsNullOrWhiteSpace(tarih))
            {
                var trimmed = tarih.Trim();
                var parts = trimmed.Split(new[] { '-', '/', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2 && int.TryParse(parts[0], out var year) && year >= 2000 && year <= 2100)
                {
                    var p1 = parts[1].Trim().ToUpperInvariant();
                    if (p1 == "1" || p1 == "Q1") return $"{year:D4}-03";
                    if (p1 == "2" || p1 == "Q2") return $"{year:D4}-06";
                    if (p1 == "3" || p1 == "Q3") return $"{year:D4}-09";
                    if (p1 == "4" || p1 == "Q4") return $"{year:D4}-12";

                    if (int.TryParse(p1, out var month) && month >= 1 && month <= 12)
                    {
                        return $"{year:D4}-{month:D2}";
                    }
                }
            }

            return GetPreviousQuarterEnd(now);
        }

        private static string GetPreviousQuarterEnd(DateTime now)
        {
            var currentQuarter = (now.Month - 1) / 3 + 1;
            return currentQuarter switch
            {
                1 => $"{now.Year - 1}-12",
                2 => $"{now.Year}-03",
                3 => $"{now.Year}-06",
                _ => $"{now.Year}-09"
            };
        }
    }
}
