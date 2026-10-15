using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Results.PopularPlaceResults;
using FibiEmlakDanismanlik.Application.Helpers;
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

            // Her tablodan (CityId, DistrictId, City, District, Count) çiftlerini SQL'de grupla
            List<(int? CityId, int? DistrictId, string City, string District, int Count)> housing;
            List<(int? CityId, int? DistrictId, string City, string District, int Count)> commercial;
            List<(int? CityId, int? DistrictId, string City, string District, int Count)> land;

            if (usageType == 1)
            {
                var rawH = await _context.forSaleHousingPropertyListings
                    .AsNoTracking()
                    .Where(x => x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus)
                                && x.City != null && x.City.Trim() != "")
                    .GroupBy(x => new { x.CityId, x.DistrictId, City = x.City.Trim(), District = x.District == null ? "" : x.District.Trim() })
                    .Select(g => new { g.Key.CityId, g.Key.DistrictId, g.Key.City, g.Key.District, Count = g.Count() })
                    .ToListAsync(ct);
                housing = rawH.Select(x => (x.CityId, x.DistrictId, x.City, x.District, x.Count)).ToList();

                var rawC = await _context.forSaleCommercialPropertyListings
                    .AsNoTracking()
                    .Where(x => x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus)
                                && x.City != null && x.City.Trim() != "")
                    .GroupBy(x => new { x.CityId, x.DistrictId, City = x.City.Trim(), District = x.District == null ? "" : x.District.Trim() })
                    .Select(g => new { g.Key.CityId, g.Key.DistrictId, g.Key.City, g.Key.District, Count = g.Count() })
                    .ToListAsync(ct);
                commercial = rawC.Select(x => (x.CityId, x.DistrictId, x.City, x.District, x.Count)).ToList();

                var rawL = await _context.forSaleLandListings
                    .AsNoTracking()
                    .Where(x => x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus)
                                && x.City != null && x.City.Trim() != "")
                    .GroupBy(x => new { x.CityId, x.DistrictId, City = x.City.Trim(), District = x.District == null ? "" : x.District.Trim() })
                    .Select(g => new { g.Key.CityId, g.Key.DistrictId, g.Key.City, g.Key.District, Count = g.Count() })
                    .ToListAsync(ct);
                land = rawL.Select(x => (x.CityId, x.DistrictId, x.City, x.District, x.Count)).ToList();
            }
            else
            {
                var rawH = await _context.rentalHousingListings
                    .AsNoTracking()
                    .Where(x => x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus)
                                && x.City != null && x.City.Trim() != "")
                    .GroupBy(x => new { x.CityId, x.DistrictId, City = x.City.Trim(), District = x.District == null ? "" : x.District.Trim() })
                    .Select(g => new { g.Key.CityId, g.Key.DistrictId, g.Key.City, g.Key.District, Count = g.Count() })
                    .ToListAsync(ct);
                housing = rawH.Select(x => (x.CityId, x.DistrictId, x.City, x.District, x.Count)).ToList();

                var rawC = await _context.rentalCommercialPropertyListings
                    .AsNoTracking()
                    .Where(x => x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus)
                                && x.City != null && x.City.Trim() != "")
                    .GroupBy(x => new { x.CityId, x.DistrictId, City = x.City.Trim(), District = x.District == null ? "" : x.District.Trim() })
                    .Select(g => new { g.Key.CityId, g.Key.DistrictId, g.Key.City, g.Key.District, Count = g.Count() })
                    .ToListAsync(ct);
                commercial = rawC.Select(x => (x.CityId, x.DistrictId, x.City, x.District, x.Count)).ToList();

                var rawL = await _context.rentalLandListings
                    .AsNoTracking()
                    .Where(x => x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus)
                                && x.City != null && x.City.Trim() != "")
                    .GroupBy(x => new { x.CityId, x.DistrictId, City = x.City.Trim(), District = x.District == null ? "" : x.District.Trim() })
                    .Select(g => new { g.Key.CityId, g.Key.DistrictId, g.Key.City, g.Key.District, Count = g.Count() })
                    .ToListAsync(ct);
                land = rawL.Select(x => (x.CityId, x.DistrictId, x.City, x.District, x.Count)).ToList();
            }

            var all = housing.Concat(commercial).Concat(land).ToList();

            var trCulture = new CultureInfo("tr-TR");

            // İl seviyesi: farklı il sayısını kontrol et
            var cityGroups = all
                .GroupBy(x => GetCityKey(x.CityId, x.City))
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

                        var cityId = g.Key.StartsWith("id:")
                            ? g.Select(x => x.CityId).FirstOrDefault(id => id.HasValue)
                            : null;

                        return new PopularPlaceItemResult
                        {
                            CityId = cityId,
                            DistrictId = null,
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
                    .GroupBy(x => GetDistrictKey(x.CityId, x.City, x.DistrictId, x.District))
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

                        var parts = g.Key.Split('|');
                        var cityPart = parts[0];
                        var districtPart = parts.Length > 1 ? parts[1] : "";

                        var cityId = cityPart.StartsWith("id:")
                            ? g.Select(x => x.CityId).FirstOrDefault(id => id.HasValue)
                            : null;

                        var districtId = districtPart.StartsWith("id:")
                            ? g.Select(x => x.DistrictId).FirstOrDefault(id => id.HasValue)
                            : null;

                        return new PopularPlaceItemResult
                        {
                            CityId = cityId,
                            DistrictId = districtId,
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

        private static string GetCityKey(int? cityId, string city)
        {
            return cityId.HasValue
                ? "id:" + cityId.Value
                : "name:" + LocationNameNormalizer.Normalize(city);
        }

        private static string GetDistrictKey(int? cityId, string city, int? districtId, string district)
        {
            var cityKey = GetCityKey(cityId, city);
            var districtPart = districtId.HasValue
                ? "id:" + districtId.Value
                : "name:" + LocationNameNormalizer.Normalize(district);
            return cityKey + "|" + districtPart;
        }
    }
}
