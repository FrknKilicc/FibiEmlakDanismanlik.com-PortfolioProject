using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Results.PopularPlaceResults;
using FibiEmlakDanismanlik.Application.Interfaces.PopularPlaceInterfaces;
using FibiEmlakDanismanlik.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Persistence.Repositories.PopularPlaceRepositories
{
    public class PopularPlaceRepository : IPopularPlaceRepository
    {
        private readonly FibiEmlakDanismanlikContext _context;

        public PopularPlaceRepository(FibiEmlakDanismanlikContext context)
        {
            _context = context;
        }

        public async Task<GetPopularPlaceResult> GetPopularPlacesAsync(int usageType, int take, int minCityCount, CancellationToken cancellationToken)
        {
            if ((usageType != 1 && usageType != 2) || take <= 0)
            {
                return new GetPopularPlaceResult();
            }

            var activeStatuses = PropertyStatuses.ActiveSet;
            var ct = cancellationToken;

            // Her tablodan (City, District, Count) çiftlerini SQL'de grupla
            List<(string City, string District, int Count)> housing;
            List<(string City, string District, int Count)> commercial;
            List<(string City, string District, int Count)> land;

            if (usageType == 1)
            {
                var rawH = await _context.forSaleHousingPropertyListings
                    .AsNoTracking()
                    .Where(x => x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus)
                                && x.City != null && x.City.Trim() != "")
                    .GroupBy(x => new { City = x.City.Trim(), District = x.District == null ? "" : x.District.Trim() })
                    .Select(g => new { g.Key.City, g.Key.District, Count = g.Count() })
                    .ToListAsync(ct);
                housing = rawH.Select(x => (x.City, x.District, x.Count)).ToList();

                var rawC = await _context.forSaleCommercialPropertyListings
                    .AsNoTracking()
                    .Where(x => x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus)
                                && x.City != null && x.City.Trim() != "")
                    .GroupBy(x => new { City = x.City.Trim(), District = x.District == null ? "" : x.District.Trim() })
                    .Select(g => new { g.Key.City, g.Key.District, Count = g.Count() })
                    .ToListAsync(ct);
                commercial = rawC.Select(x => (x.City, x.District, x.Count)).ToList();

                var rawL = await _context.forSaleLandListings
                    .AsNoTracking()
                    .Where(x => x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus)
                                && x.City != null && x.City.Trim() != "")
                    .GroupBy(x => new { City = x.City.Trim(), District = x.District == null ? "" : x.District.Trim() })
                    .Select(g => new { g.Key.City, g.Key.District, Count = g.Count() })
                    .ToListAsync(ct);
                land = rawL.Select(x => (x.City, x.District, x.Count)).ToList();
            }
            else
            {
                var rawH = await _context.rentalHousingListings
                    .AsNoTracking()
                    .Where(x => x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus)
                                && x.City != null && x.City.Trim() != "")
                    .GroupBy(x => new { City = x.City.Trim(), District = x.District == null ? "" : x.District.Trim() })
                    .Select(g => new { g.Key.City, g.Key.District, Count = g.Count() })
                    .ToListAsync(ct);
                housing = rawH.Select(x => (x.City, x.District, x.Count)).ToList();

                var rawC = await _context.rentalCommercialPropertyListings
                    .AsNoTracking()
                    .Where(x => x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus)
                                && x.City != null && x.City.Trim() != "")
                    .GroupBy(x => new { City = x.City.Trim(), District = x.District == null ? "" : x.District.Trim() })
                    .Select(g => new { g.Key.City, g.Key.District, Count = g.Count() })
                    .ToListAsync(ct);
                commercial = rawC.Select(x => (x.City, x.District, x.Count)).ToList();

                var rawL = await _context.rentalLandListings
                    .AsNoTracking()
                    .Where(x => x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus)
                                && x.City != null && x.City.Trim() != "")
                    .GroupBy(x => new { City = x.City.Trim(), District = x.District == null ? "" : x.District.Trim() })
                    .Select(g => new { g.Key.City, g.Key.District, Count = g.Count() })
                    .ToListAsync(ct);
                land = rawL.Select(x => (x.City, x.District, x.Count)).ToList();
            }

            var all = housing.Concat(commercial).Concat(land).ToList();

            var trComparer = StringComparer.Create(new CultureInfo("tr-TR"), ignoreCase: true);
            var trCulture = new CultureInfo("tr-TR");

            // İl seviyesi: farklı il sayısını kontrol et
            var cityGroups = all
                .GroupBy(x => x.City, trComparer)
                .ToList();

            var distinctCityCount = cityGroups.Count;

            if (distinctCityCount >= minCityCount)
            {
                // === İl seviyesi ===
                var items = cityGroups
                    .Select(g =>
                    {
                        // Görünen ad: en çok ilanı olan YAZIM (Ordinal ile alt grupla)
                        var displayCity = g
                            .GroupBy(x => x.City, StringComparer.Ordinal)
                            .OrderByDescending(sub => sub.Sum(x => x.Count))
                            .First()
                            .Key;

                        return new PopularPlaceItemResult
                        {
                            City = displayCity,
                            District = null,
                            ListingCount = g.Sum(x => x.Count)
                        };
                    })
                    .OrderByDescending(x => x.ListingCount)
                    .ThenBy(x => x.City, StringComparer.Create(trCulture, ignoreCase: false))
                    .Take(take)
                    .ToList();

                return new GetPopularPlaceResult
                {
                    Level = PopularPlaceConstants.LevelCity,
                    Items = items
                };
            }
            else
            {
                // === İlçe seviyesi ===
                // District boş olanları at
                var districtRows = all.Where(x => !string.IsNullOrWhiteSpace(x.District)).ToList();

                var items = districtRows
                    .GroupBy(x => (x.City, x.District), new CityDistrictComparer(trComparer))
                    .Select(g =>
                    {
                        // Görünen ad: en çok ilanı olan YAZIM (Ordinal)
                        var displayCity = g
                            .GroupBy(x => x.City, StringComparer.Ordinal)
                            .OrderByDescending(sub => sub.Sum(x => x.Count))
                            .First()
                            .Key;

                        var displayDistrict = g
                            .GroupBy(x => x.District, StringComparer.Ordinal)
                            .OrderByDescending(sub => sub.Sum(x => x.Count))
                            .First()
                            .Key;

                        return new PopularPlaceItemResult
                        {
                            City = displayCity,
                            District = displayDistrict,
                            ListingCount = g.Sum(x => x.Count)
                        };
                    })
                    .OrderByDescending(x => x.ListingCount)
                    .ThenBy(x => x.District, StringComparer.Create(trCulture, ignoreCase: false))
                    .ThenBy(x => x.City, StringComparer.Create(trCulture, ignoreCase: false))
                    .Take(take)
                    .ToList();

                return new GetPopularPlaceResult
                {
                    Level = PopularPlaceConstants.LevelDistrict,
                    Items = items
                };
            }
        }

        /// <summary>
        /// (City, District) tuple'ını Türkçe duyarsız karşılaştırma ile eşleştirir.
        /// </summary>
        private sealed class CityDistrictComparer : IEqualityComparer<(string City, string District)>
        {
            private readonly StringComparer _comparer;

            public CityDistrictComparer(StringComparer comparer)
            {
                _comparer = comparer;
            }

            public bool Equals((string City, string District) x, (string City, string District) y)
            {
                return _comparer.Equals(x.City, y.City) && _comparer.Equals(x.District, y.District);
            }

            public int GetHashCode((string City, string District) obj)
            {
                return HashCode.Combine(_comparer.GetHashCode(obj.City), _comparer.GetHashCode(obj.District));
            }
        }
    }
}
