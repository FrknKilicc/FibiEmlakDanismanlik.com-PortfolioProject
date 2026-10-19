using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Results.MapResults;
using FibiEmlakDanismanlik.Application.Interfaces.MapInterfaces;
using FibiEmlakDanismanlik.Application.Models.MapModels;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;
using FibiEmlakDanismanlik.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Persistence.Repositories.MapRepositories
{
    public class MapRepository : IMapRepository
    {
        private readonly FibiEmlakDanismanlikContext _context;

        public MapRepository(FibiEmlakDanismanlikContext context)
        {
            _context = context;
        }

        public async Task<ListingMarkerResult> GetListingMarkerAsync(int listingId, int listingTypeId)
        {
            var lt = await _context.listingTypes.FirstOrDefaultAsync(x => x.ListingTypeId == listingTypeId);
            if (lt == null) return null;

            var geo = await _context.ListingGeoPoints
                .FirstOrDefaultAsync(x => x.ListingId == listingId && x.ListingTypeId == listingTypeId);

            if (geo == null) return null;

            var catKey = NormalizeCategoryKey(lt.Name);
            var usageKey = lt.UsageType == UsageType.ForSale ? "sale"
                        : lt.UsageType == UsageType.ForRent ? "rent"
                        : "both";

            string? status = null;
            string title = "";
            string? subtitle = null;

            if (lt.UsageType == UsageType.ForSale)
            {
                if (catKey == "housing")
                {
                    var item = await _context.forSaleHousingPropertyListings
                        .FirstOrDefaultAsync(x => x.ForSaleHousingListId == listingId && x.ListingTypeId == listingTypeId);
                    if (item == null) return null;
                    status = item.PropertyStatus;
                    title = item.PropertyName;
                    subtitle = $"{item.City} / {item.District}";
                }
                else if (catKey == "land")
                {
                    var item = await _context.forSaleLandListings
                        .FirstOrDefaultAsync(x => x.ForSaleLandListingId == listingId && x.ListingTypeId == listingTypeId);
                    if (item == null) return null;
                    status = item.PropertyStatus;
                    title = item.PropertyName;
                    subtitle = $"{item.City} / {item.District}";
                }
                else
                {
                    var item = await _context.forSaleCommercialPropertyListings
                        .FirstOrDefaultAsync(x => x.ForSaleCommercialListingId == listingId && x.ListingTypeId == listingTypeId);
                    if (item == null) return null;
                    status = item.PropertyStatus;
                    title = item.PropertyName;
                    subtitle = $"{item.City} / {item.District}";
                }
            }
            else if (lt.UsageType == UsageType.ForRent)
            {
                if (catKey == "housing")
                {
                    var item = await _context.rentalHousingListings
                        .FirstOrDefaultAsync(x => x.RentalHousingListId == listingId && x.ListingTypeId == listingTypeId);
                    if (item == null) return null;
                    status = item.PropertyStatus;
                    title = item.PropertyName;
                    subtitle = $"{item.City} / {item.District}";
                }
                else if (catKey == "land")
                {
                    var item = await _context.rentalLandListings
                        .FirstOrDefaultAsync(x => x.RentalLandListingId == listingId && x.ListingTypeId == listingTypeId);
                    if (item == null) return null;
                    status = item.PropertyStatus;
                    title = item.PropertyName;
                    subtitle = $"{item.City} / {item.District}";
                }
                else
                {
                    var item = await _context.rentalCommercialPropertyListings
                        .FirstOrDefaultAsync(x => x.RentalCommercialListId == listingId && x.ListingTypeId == listingTypeId);
                    if (item == null) return null;
                    status = item.PropertyStatus;
                    title = item.PropertyName;
                    subtitle = $"{item.City} / {item.District}";
                }
            }
            else
            {
                return null;
            }

            var statusKey = NormalizeStatusKey(status);
            var iconKey = $"{usageKey}_{catKey}_{statusKey}"; 

            return new ListingMarkerResult
            {
                ListingId = listingId,
                ListingTypeId = listingTypeId,
                Latitude = geo.Latitude,
                Longitude = geo.Longitude,
                Zoom = 15,
                Title = title,
                Subtitle = subtitle,
                PropertyStatus = status,
                IconKey = iconKey,
                UsageTypeId = (int)lt.UsageType,
                ListingTypeName = lt.Name
            };
        }

        private static string NormalizeCategoryKey(string? name)
        {
            var s = (name ?? "").Trim().ToLowerInvariant();
            if (s.Contains("konut")) return "housing";
            if (s.Contains("arsa")) return "land";
            return "commercial";
        }

        private static string NormalizeStatusKey(string? status)
        {
            var s = (status ?? "").Trim().ToLowerInvariant();
            if (s == "aktif" || s == "active" || s == "1" || s.Contains("aktif")) return "active";
            return "passive";
        }

        public async Task<List<GetMapListingsResult>> GetMapListingsAsync(int? usageType, string? category, int? cityId)
        {
            // 1. listingTypes'ı belleğe al (6 kayıt); usageType ve category filtresine uymayan tiplerin tablolarını hiç sorgulama
            var allListingTypes = await _context.listingTypes.AsNoTracking().ToListAsync();

            bool IsTypeMatched(ListingType lt)
            {
                if (usageType.HasValue && (int)lt.UsageType != usageType.Value)
                    return false;

                if (!string.IsNullOrWhiteSpace(category))
                {
                    var catKey = NormalizeCategoryKey(lt.Name);
                    if (!string.Equals(catKey, category.Trim(), StringComparison.OrdinalIgnoreCase))
                        return false;
                }

                return true;
            }

            var matchedTypes = allListingTypes.Where(IsTypeMatched).ToList();
            if (matchedTypes.Count == 0)
            {
                return new List<GetMapListingsResult>();
            }

            bool shouldQuery(UsageType usage, string expectedCat) =>
                matchedTypes.Any(lt => lt.UsageType == usage && NormalizeCategoryKey(lt.Name) == expectedCat);

            bool querySaleHousing = shouldQuery(UsageType.ForSale, ListingGeoConstants.CategoryHousing);
            bool querySaleLand = shouldQuery(UsageType.ForSale, ListingGeoConstants.CategoryLand);
            bool querySaleCommercial = shouldQuery(UsageType.ForSale, ListingGeoConstants.CategoryCommercial);

            bool queryRentHousing = shouldQuery(UsageType.ForRent, ListingGeoConstants.CategoryHousing);
            bool queryRentLand = shouldQuery(UsageType.ForRent, ListingGeoConstants.CategoryLand);
            bool queryRentCommercial = shouldQuery(UsageType.ForRent, ListingGeoConstants.CategoryCommercial);

            // 2. Tüm ListingGeoPoints'i AsNoTracking ile belleğe al ve (ListingTypeId, ListingId) anahtarlı sözlüğe koy
            // (Aynı anahtar iki kez varsa en yüksek ListingGeoPointId'li kayıt kalır)
            var allGeoPoints = await _context.ListingGeoPoints.AsNoTracking().ToListAsync();
            var geoDict = allGeoPoints
                .GroupBy(g => (g.ListingTypeId, g.ListingId))
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderByDescending(x => x.ListingGeoPointId).First()
                );

            var rawItems = new List<ListingItemDto>();
            var activeSet = PropertyStatuses.ActiveSet;

            // 3. Gerekli tabloların her birini aktiflik ve (varsa) cityId filtresiyle, yalnızca gereken alanları Select ederek sorgula
            if (querySaleHousing)
            {
                var q = _context.forSaleHousingPropertyListings.AsNoTracking()
                    .Where(x => x.PropertyStatus != null && activeSet.Contains(x.PropertyStatus));
                if (cityId.HasValue) q = q.Where(x => x.CityId == cityId.Value);

                var items = await q.Select(x => new ListingItemDto
                {
                    ListingId = x.ForSaleHousingListId,
                    ListingTypeId = x.ListingTypeId,
                    Title = x.PropertyName,
                    Price = x.Price,
                    Area = x.GrossArea,
                    Rooms = x.NumberOfRoom,
                    City = x.City,
                    District = x.District,
                    Neighborhood = x.Neighborhood,
                    ImageUrl = x.PropImgUrl1
                }).ToListAsync();
                rawItems.AddRange(items);
            }

            if (querySaleLand)
            {
                var q = _context.forSaleLandListings.AsNoTracking()
                    .Where(x => x.PropertyStatus != null && activeSet.Contains(x.PropertyStatus));
                if (cityId.HasValue) q = q.Where(x => x.CityId == cityId.Value);

                var items = await q.Select(x => new ListingItemDto
                {
                    ListingId = x.ForSaleLandListingId,
                    ListingTypeId = x.ListingTypeId,
                    Title = x.PropertyName,
                    Price = x.Price,
                    Area = x.Area,
                    Rooms = null,
                    City = x.City,
                    District = x.District,
                    Neighborhood = x.Neighborhood,
                    ImageUrl = x.PropImgUrl1
                }).ToListAsync();
                rawItems.AddRange(items);
            }

            if (querySaleCommercial)
            {
                var q = _context.forSaleCommercialPropertyListings.AsNoTracking()
                    .Where(x => x.PropertyStatus != null && activeSet.Contains(x.PropertyStatus));
                if (cityId.HasValue) q = q.Where(x => x.CityId == cityId.Value);

                var items = await q.Select(x => new ListingItemDto
                {
                    ListingId = x.ForSaleCommercialListingId,
                    ListingTypeId = x.ListingTypeId,
                    Title = x.PropertyName,
                    Price = x.Price,
                    Area = x.GrossArea,
                    Rooms = null,
                    City = x.City,
                    District = x.District,
                    Neighborhood = x.Neighborhood,
                    ImageUrl = x.PropImgUrl1
                }).ToListAsync();
                rawItems.AddRange(items);
            }

            if (queryRentHousing)
            {
                var q = _context.rentalHousingListings.AsNoTracking()
                    .Where(x => x.PropertyStatus != null && activeSet.Contains(x.PropertyStatus));
                if (cityId.HasValue) q = q.Where(x => x.CityId == cityId.Value);

                var items = await q.Select(x => new ListingItemDto
                {
                    ListingId = x.RentalHousingListId,
                    ListingTypeId = x.ListingTypeId,
                    Title = x.PropertyName,
                    Price = x.Rent,
                    Area = x.GrossArea,
                    Rooms = x.NumberOfRoom,
                    City = x.City,
                    District = x.District,
                    Neighborhood = x.Neighborhood,
                    ImageUrl = x.PropImgUrl1
                }).ToListAsync();
                rawItems.AddRange(items);
            }

            if (queryRentLand)
            {
                var q = _context.rentalLandListings.AsNoTracking()
                    .Where(x => x.PropertyStatus != null && activeSet.Contains(x.PropertyStatus));
                if (cityId.HasValue) q = q.Where(x => x.CityId == cityId.Value);

                var items = await q.Select(x => new ListingItemDto
                {
                    ListingId = x.RentalLandListingId,
                    ListingTypeId = x.ListingTypeId,
                    Title = x.PropertyName,
                    Price = x.Rent,
                    Area = x.Area,
                    Rooms = null,
                    City = x.City,
                    District = x.District,
                    Neighborhood = x.Neighborhood,
                    ImageUrl = x.PropImgUrl1
                }).ToListAsync();
                rawItems.AddRange(items);
            }

            if (queryRentCommercial)
            {
                var q = _context.rentalCommercialPropertyListings.AsNoTracking()
                    .Where(x => x.PropertyStatus != null && activeSet.Contains(x.PropertyStatus));
                if (cityId.HasValue) q = q.Where(x => x.CityId == cityId.Value);

                var items = await q.Select(x => new ListingItemDto
                {
                    ListingId = x.RentalCommercialListId,
                    ListingTypeId = x.ListingTypeId,
                    Title = x.PropertyName,
                    Price = x.Rent,
                    Area = x.GrossArea,
                    Rooms = null,
                    City = x.City,
                    District = x.District,
                    Neighborhood = x.Neighborhood,
                    ImageUrl = x.PropImgUrl1
                }).ToListAsync();
                rawItems.AddRange(items);
            }

            // 4. Bellekte geo sözlüğüyle eşleştir; koordinatı olmayanları atla. Kiralıkta ListingTypeId null ise atla.
            var typesById = allListingTypes.ToDictionary(x => x.ListingTypeId);
            var results = new List<GetMapListingsResult>();

            foreach (var item in rawItems)
            {
                if (!item.ListingTypeId.HasValue)
                    continue;

                if (!typesById.TryGetValue(item.ListingTypeId.Value, out var lt))
                    continue;

                if (!geoDict.TryGetValue((lt.ListingTypeId, item.ListingId), out var geo))
                    continue;

                results.Add(new GetMapListingsResult
                {
                    ListingId = item.ListingId,
                    ListingTypeId = lt.ListingTypeId,
                    UsageTypeId = (int)lt.UsageType,
                    CategoryKey = NormalizeCategoryKey(lt.Name),
                    ListingTypeName = lt.Name ?? "",
                    Title = item.Title ?? "",
                    Price = item.Price,
                    Area = item.Area,
                    Rooms = item.Rooms,
                    City = item.City,
                    District = item.District,
                    Neighborhood = item.Neighborhood,
                    ImageUrl = item.ImageUrl,
                    Latitude = geo.Latitude,
                    Longitude = geo.Longitude,
                    IsApproximate = ListingGeoConstants.IsApproximate(geo.Precision)
                });
            }

            // 5. UsageTypeId sonra Price artan sırala
            return results
                .OrderBy(x => x.UsageTypeId)
                .ThenBy(x => x.Price)
                .ToList();
        }

        public async Task<List<ListingGeocodeCandidate>> GetListingsWithoutGeoPointAsync(int take)
        {
            if (take <= 0)
                return new List<ListingGeocodeCandidate>();

            var existingKeys = await _context.ListingGeoPoints
                .AsNoTracking()
                .Select(x => new { x.ListingTypeId, x.ListingId })
                .ToListAsync();

            var geoSet = new HashSet<(int ListingTypeId, int ListingId)>(
                existingKeys.Select(x => (x.ListingTypeId, x.ListingId)));

            var activeSet = PropertyStatuses.ActiveSet;
            var candidates = new List<ListingGeocodeCandidate>();

            // 1. Satılık Konut
            var saleHousing = await _context.forSaleHousingPropertyListings
                .AsNoTracking()
                .Where(x => x.PropertyStatus != null && activeSet.Contains(x.PropertyStatus))
                .Select(x => new ListingGeocodeCandidate
                {
                    ListingId = x.ForSaleHousingListId,
                    ListingTypeId = x.ListingTypeId,
                    City = x.City,
                    District = x.District,
                    Neighborhood = x.Neighborhood
                })
                .ToListAsync();

            foreach (var item in saleHousing)
            {
                if (!geoSet.Contains((item.ListingTypeId, item.ListingId)))
                {
                    candidates.Add(item);
                    if (candidates.Count >= take) return candidates;
                }
            }

            // 2. Satılık Arsa
            var saleLand = await _context.forSaleLandListings
                .AsNoTracking()
                .Where(x => x.PropertyStatus != null && activeSet.Contains(x.PropertyStatus))
                .Select(x => new ListingGeocodeCandidate
                {
                    ListingId = x.ForSaleLandListingId,
                    ListingTypeId = x.ListingTypeId,
                    City = x.City,
                    District = x.District,
                    Neighborhood = x.Neighborhood
                })
                .ToListAsync();

            foreach (var item in saleLand)
            {
                if (!geoSet.Contains((item.ListingTypeId, item.ListingId)))
                {
                    candidates.Add(item);
                    if (candidates.Count >= take) return candidates;
                }
            }

            // 3. Satılık Ticari
            var saleCommercial = await _context.forSaleCommercialPropertyListings
                .AsNoTracking()
                .Where(x => x.PropertyStatus != null && activeSet.Contains(x.PropertyStatus))
                .Select(x => new ListingGeocodeCandidate
                {
                    ListingId = x.ForSaleCommercialListingId,
                    ListingTypeId = x.ListingTypeId,
                    City = x.City,
                    District = x.District,
                    Neighborhood = x.Neighborhood
                })
                .ToListAsync();

            foreach (var item in saleCommercial)
            {
                if (!geoSet.Contains((item.ListingTypeId, item.ListingId)))
                {
                    candidates.Add(item);
                    if (candidates.Count >= take) return candidates;
                }
            }

            // 4. Kiralık Konut
            var rentHousing = await _context.rentalHousingListings
                .AsNoTracking()
                .Where(x => x.PropertyStatus != null && activeSet.Contains(x.PropertyStatus) && x.ListingTypeId != null)
                .Select(x => new ListingGeocodeCandidate
                {
                    ListingId = x.RentalHousingListId,
                    ListingTypeId = x.ListingTypeId!.Value,
                    City = x.City,
                    District = x.District,
                    Neighborhood = x.Neighborhood
                })
                .ToListAsync();

            foreach (var item in rentHousing)
            {
                if (!geoSet.Contains((item.ListingTypeId, item.ListingId)))
                {
                    candidates.Add(item);
                    if (candidates.Count >= take) return candidates;
                }
            }

            // 5. Kiralık Arsa
            var rentLand = await _context.rentalLandListings
                .AsNoTracking()
                .Where(x => x.PropertyStatus != null && activeSet.Contains(x.PropertyStatus) && x.ListingTypeId != null)
                .Select(x => new ListingGeocodeCandidate
                {
                    ListingId = x.RentalLandListingId,
                    ListingTypeId = x.ListingTypeId!.Value,
                    City = x.City,
                    District = x.District,
                    Neighborhood = x.Neighborhood
                })
                .ToListAsync();

            foreach (var item in rentLand)
            {
                if (!geoSet.Contains((item.ListingTypeId, item.ListingId)))
                {
                    candidates.Add(item);
                    if (candidates.Count >= take) return candidates;
                }
            }

            // 6. Kiralık Ticari
            var rentCommercial = await _context.rentalCommercialPropertyListings
                .AsNoTracking()
                .Where(x => x.PropertyStatus != null && activeSet.Contains(x.PropertyStatus) && x.ListingTypeId != null)
                .Select(x => new ListingGeocodeCandidate
                {
                    ListingId = x.RentalCommercialListId,
                    ListingTypeId = x.ListingTypeId!.Value,
                    City = x.City,
                    District = x.District,
                    Neighborhood = x.Neighborhood
                })
                .ToListAsync();

            foreach (var item in rentCommercial)
            {
                if (!geoSet.Contains((item.ListingTypeId, item.ListingId)))
                {
                    candidates.Add(item);
                    if (candidates.Count >= take) return candidates;
                }
            }

            return candidates;
        }

        public async Task<int> AddGeoPointsAsync(IReadOnlyList<ListingGeoPoint> points)
        {
            if (points == null || points.Count == 0)
                return 0;

            var existingKeys = await _context.ListingGeoPoints
                .AsNoTracking()
                .Select(x => new { x.ListingTypeId, x.ListingId })
                .ToListAsync();

            var existingSet = new HashSet<(int ListingTypeId, int ListingId)>(
                existingKeys.Select(x => (x.ListingTypeId, x.ListingId)));

            var toAdd = new List<ListingGeoPoint>();
            foreach (var pt in points)
            {
                var key = (pt.ListingTypeId, pt.ListingId);
                if (!existingSet.Contains(key))
                {
                    toAdd.Add(pt);
                    existingSet.Add(key);
                }
            }

            if (toAdd.Count == 0)
                return 0;

            await _context.ListingGeoPoints.AddRangeAsync(toAdd);
            await _context.SaveChangesAsync();
            return toAdd.Count;
        }

        private sealed class ListingItemDto
        {
            public int ListingId { get; set; }
            public int? ListingTypeId { get; set; }
            public string Title { get; set; } = "";
            public decimal Price { get; set; }
            public double? Area { get; set; }
            public string? Rooms { get; set; }
            public string? City { get; set; }
            public string? District { get; set; }
            public string? Neighborhood { get; set; }
            public string? ImageUrl { get; set; }
        }
    }
}

