using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FibiEmlakDanismanlik.Persistence.Repositories.AdminListingRepositories
{
    /// <summary>
    /// Yönetim paneli için 6 farklı ilan tablosunu birleştiren ve erişim kapsamını uygulayan repository implementasyonu.
    /// </summary>
    public class AdminListingRepository : IAdminListingRepository
    {
        private readonly FibiEmlakDanismanlikContext _context;

        public AdminListingRepository(FibiEmlakDanismanlikContext context)
        {
            _context = context;
        }

        public async Task<int?> GetListingAgentIdAsync(int listingTypeId, int listingId)
        {
            return listingTypeId switch
            {
                1 => await _context.forSaleLandListings.AsNoTracking()
                    .Where(x => x.ForSaleLandListingId == listingId)
                    .Select(x => (int?)x.AgentId)
                    .FirstOrDefaultAsync(),

                2 => await _context.forSaleHousingPropertyListings.AsNoTracking()
                    .Where(x => x.ForSaleHousingListId == listingId)
                    .Select(x => (int?)x.AgentId)
                    .FirstOrDefaultAsync(),

                3 => await _context.forSaleCommercialPropertyListings.AsNoTracking()
                    .Where(x => x.ForSaleCommercialListingId == listingId)
                    .Select(x => (int?)x.AgentId)
                    .FirstOrDefaultAsync(),

                7 => await _context.rentalLandListings.AsNoTracking()
                    .Where(x => x.RentalLandListingId == listingId)
                    .Select(x => (int?)x.AgentId)
                    .FirstOrDefaultAsync(),

                8 => await _context.rentalHousingListings.AsNoTracking()
                    .Where(x => x.RentalHousingListId == listingId)
                    .Select(x => (int?)x.AgentId)
                    .FirstOrDefaultAsync(),

                9 => await _context.rentalCommercialPropertyListings.AsNoTracking()
                    .Where(x => x.RentalCommercialListId == listingId)
                    .Select(x => (int?)x.AgentId)
                    .FirstOrDefaultAsync(),

                _ => null
            };
        }

        public async Task<bool> GetCanViewAllListingsAsync(int agentId)
        {
            return await _context.Agents.AsNoTracking()
                .Where(a => a.AgentId == agentId)
                .Select(a => a.CanViewAllListings)
                .FirstOrDefaultAsync();
        }

        public async Task<List<ListingAccessGrant>> GetGrantsForAgentAsync(int agentId)
        {
            return await _context.ListingAccessGrants.AsNoTracking()
                .Where(g => g.AgentId == agentId)
                .ToListAsync();
        }

        public async Task<(int Total, List<AdminListingRow> Rows)> SearchAsync(AdminListingFilter filter, ListingScope scope)
        {
            var allowedTypeIds = new HashSet<int>(AdminListingConstants.AllListingTypeIds);

            if (filter.IsRent.HasValue)
            {
                allowedTypeIds.IntersectWith(filter.IsRent.Value ? new[] { 7, 8, 9 } : new[] { 1, 2, 3 });
            }

            if (!string.IsNullOrWhiteSpace(filter.Category))
            {
                var cat = filter.Category.Trim().ToLowerInvariant();
                var catTypeIds = cat switch
                {
                    "land" => new[] { 1, 7 },
                    "housing" => new[] { 2, 8 },
                    "commercial" => new[] { 3, 9 },
                    _ => Array.Empty<int>()
                };
                allowedTypeIds.IntersectWith(catTypeIds);
            }

            if (allowedTypeIds.Count == 0)
            {
                return (0, new List<AdminListingRow>());
            }

            var query = BuildUnifiedQuery(allowedTypeIds);

            // Kapsam denetimi
            if (!scope.SeeAll)
            {
                query = query.Where(BuildScopePredicate(scope));
            }

            // Durum filtresi
            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                var status = filter.Status.Trim();
                query = query.Where(x => x.PropertyStatus == status);
            }

            // Danışman filtresi
            if (filter.AgentId.HasValue)
            {
                query = query.Where(x => x.AgentId == filter.AgentId.Value);
            }

            // Arama filtresi (ilan no, başlık, il, ilçe, mahalle)
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim();
                if (int.TryParse(search, out var searchNo))
                {
                    query = query.Where(x =>
                        x.PropertyNo == searchNo ||
                        x.Title.Contains(search) ||
                        x.City.Contains(search) ||
                        x.District.Contains(search) ||
                        x.Neighborhood.Contains(search));
                }
                else
                {
                    query = query.Where(x =>
                        x.Title.Contains(search) ||
                        x.City.Contains(search) ||
                        x.District.Contains(search) ||
                        x.Neighborhood.Contains(search));
                }
            }

            var total = await query.CountAsync();

            // Sıralama
            query = (filter.Sort?.Trim().ToLowerInvariant()) switch
            {
                "oldest" => query.OrderBy(x => x.CreatedDate).ThenBy(x => x.ListingId),
                "price_asc" => query.OrderBy(x => x.Price).ThenByDescending(x => x.CreatedDate),
                "price_desc" => query.OrderByDescending(x => x.Price).ThenByDescending(x => x.CreatedDate),
                _ => query.OrderByDescending(x => x.CreatedDate).ThenByDescending(x => x.ListingId)
            };

            // Sayfalama
            var page = filter.Page < 1 ? 1 : filter.Page;
            var pageSize = filter.PageSize < 1 ? AdminListingConstants.DefaultPageSize : filter.PageSize;
            if (pageSize > AdminListingConstants.MaxPageSize)
            {
                pageSize = AdminListingConstants.MaxPageSize;
            }

            var pagedEntries = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            if (pagedEntries.Count == 0)
            {
                return (total, new List<AdminListingRow>());
            }

            // Danışman adları
            var agentIds = pagedEntries.Select(e => e.AgentId).Distinct().ToList();
            var agentMap = await _context.Agents.AsNoTracking()
                .Where(a => agentIds.Contains(a.AgentId))
                .Select(a => new { a.AgentId, a.AgentName })
                .ToDictionaryAsync(a => a.AgentId, a => a.AgentName);

            // Görseller (Kapak ve görsel sayısı)
            var typeIdsInPage = pagedEntries.Select(e => e.ListingTypeId).Distinct().ToList();
            var listingIdsInPage = pagedEntries.Select(e => e.ListingId).Distinct().ToList();

            var images = await _context.ListingImages.AsNoTracking()
                .Where(img => typeIdsInPage.Contains(img.ListingTypeId) && listingIdsInPage.Contains(img.ListingId))
                .Select(img => new
                {
                    img.ListingTypeId,
                    img.ListingId,
                    img.IsCover,
                    img.SortOrder,
                    img.ThumbUrl,
                    img.Url
                })
                .ToListAsync();

            var imagesByListing = images
                .GroupBy(img => (img.ListingTypeId, img.ListingId))
                .ToDictionary(g => g.Key, g => g.ToList());

            var rows = new List<AdminListingRow>(pagedEntries.Count);
            foreach (var entry in pagedEntries)
            {
                var key = (entry.ListingTypeId, entry.ListingId);
                string? coverThumbUrl = null;
                var imageCount = 0;

                if (imagesByListing.TryGetValue(key, out var listingImgs))
                {
                    imageCount = listingImgs.Count;
                    var coverImg = listingImgs.FirstOrDefault(i => i.IsCover)
                                ?? listingImgs.OrderBy(i => i.SortOrder).FirstOrDefault();

                    if (coverImg != null)
                    {
                        coverThumbUrl = !string.IsNullOrWhiteSpace(coverImg.ThumbUrl) ? coverImg.ThumbUrl : coverImg.Url;
                    }
                }

                var agentName = agentMap.TryGetValue(entry.AgentId, out var name) ? name : string.Empty;

                rows.Add(new AdminListingRow
                {
                    ListingTypeId = entry.ListingTypeId,
                    ListingId = entry.ListingId,
                    IsRent = AdminListingConstants.IsRent(entry.ListingTypeId),
                    Category = AdminListingConstants.Category(entry.ListingTypeId),
                    CategoryDisplay = AdminListingConstants.CategoryDisplay(entry.ListingTypeId),
                    Title = entry.Title,
                    City = entry.City,
                    District = entry.District,
                    Neighborhood = entry.Neighborhood,
                    Price = entry.Price,
                    Status = entry.PropertyStatus,
                    AgentId = entry.AgentId,
                    AgentName = agentName,
                    CreatedDate = entry.CreatedDate,
                    BestDeals = entry.BestDeals,
                    CoverThumbUrl = coverThumbUrl,
                    ImageCount = imageCount,
                    CanEdit = false
                });
            }

            return (total, rows);
        }

        public async Task<AdminListingSummary> GetSummaryAsync(ListingScope scope)
        {
            var query = BuildUnifiedQuery(new HashSet<int>(AdminListingConstants.AllListingTypeIds));
            if (!scope.SeeAll)
            {
                query = query.Where(BuildScopePredicate(scope));
            }

            var groups = await query
                .GroupBy(x => new { x.ListingTypeId, x.PropertyStatus, x.BestDeals })
                .Select(g => new
                {
                    g.Key.ListingTypeId,
                    g.Key.PropertyStatus,
                    g.Key.BestDeals,
                    Count = g.Count()
                })
                .ToListAsync();

            var summary = new AdminListingSummary
            {
                Total = groups.Sum(g => g.Count),
                SaleTotal = groups.Where(g => !AdminListingConstants.IsRent(g.ListingTypeId)).Sum(g => g.Count),
                RentTotal = groups.Where(g => AdminListingConstants.IsRent(g.ListingTypeId)).Sum(g => g.Count),
                BestDealsSale = groups.Where(g => !AdminListingConstants.IsRent(g.ListingTypeId) && g.BestDeals).Sum(g => g.Count),
                BestDealsRent = groups.Where(g => AdminListingConstants.IsRent(g.ListingTypeId) && g.BestDeals).Sum(g => g.Count),
                ByCategorySale = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
                {
                    ["land"] = groups.Where(g => g.ListingTypeId == 1).Sum(g => g.Count),
                    ["housing"] = groups.Where(g => g.ListingTypeId == 2).Sum(g => g.Count),
                    ["commercial"] = groups.Where(g => g.ListingTypeId == 3).Sum(g => g.Count)
                },
                ByCategoryRent = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
                {
                    ["land"] = groups.Where(g => g.ListingTypeId == 7).Sum(g => g.Count),
                    ["housing"] = groups.Where(g => g.ListingTypeId == 8).Sum(g => g.Count),
                    ["commercial"] = groups.Where(g => g.ListingTypeId == 9).Sum(g => g.Count)
                }
            };

            var statusDict = AdminListingConstants.AllStatuses.ToDictionary(s => s, _ => 0, StringComparer.OrdinalIgnoreCase);
            foreach (var g in groups)
            {
                if (!string.IsNullOrWhiteSpace(g.PropertyStatus))
                {
                    if (statusDict.ContainsKey(g.PropertyStatus))
                    {
                        statusDict[g.PropertyStatus] += g.Count;
                    }
                    else
                    {
                        statusDict[g.PropertyStatus] = g.Count;
                    }
                }
            }
            summary.ByStatus = statusDict;

            return summary;
        }

        public async Task<AdminListingDetail?> GetDetailAsync(int listingTypeId, int listingId)
        {
            AdminListingDetail? detail = null;

            switch (listingTypeId)
            {
                case 1:
                    {
                        var land = await _context.forSaleLandListings.AsNoTracking()
                            .FirstOrDefaultAsync(x => x.ForSaleLandListingId == listingId);
                        if (land == null) return null;

                        detail = new AdminListingDetail
                        {
                            ListingTypeId = 1,
                            ListingId = land.ForSaleLandListingId,
                            IsRent = false,
                            Category = "land",
                            CategoryDisplay = AdminListingConstants.CategoryDisplay("land"),
                            PropertyNo = land.PropertyNo,
                            PropertyName = land.PropertyName,
                            PropertyDescription = land.PropertyDescription,
                            PropertyStatus = land.PropertyStatus,
                            CreatedDate = land.CreatedDate,
                            Price = land.Price,
                            PricePerSquareMeter = land.PricePerSquareMeter,
                            City = land.City,
                            District = land.District,
                            CityId = land.CityId,
                            DistrictId = land.DistrictId,
                            NeighborhoodId = land.NeighborhoodId,
                            Neighborhood = land.Neighborhood,
                            AddressDesc = land.AddressDesc,
                            ZoningStatus = land.ZoningStatus,
                            SharePercentage = land.SharePercentage,
                            Area = land.Area,
                            ParcelNumber = land.ParcelNumber,
                            PlotNumber = land.PlotNumber,
                            MapSheetNumber = land.MapSheetNumber,
                            FloorAreaRatio = land.FloorAreaRatio,
                            BaseAreaRatio = land.BaseAreaRatio,
                            ZoningPlan = land.ZoningPlan,
                            TitleDeedStatus = land.TitleDeedStatus,
                            DevelopmentRight = land.DevelopmentRight?.ToString(),
                            LandLoan = land.LandLoan,
                            Exchange = land.Exchange,
                            BestDeals = land.BestDeals,
                            LandCategoryId = land.LandCategoryId,
                            AgentId = land.AgentId
                        };

                        detail.AmenityIds = await _context.ForSaleLandListingAmenities.AsNoTracking()
                            .Where(a => a.ForSaleLandListId == listingId)
                            .Select(a => a.AmenityId)
                            .ToListAsync();
                        break;
                    }

                case 2:
                    {
                        var house = await _context.forSaleHousingPropertyListings.AsNoTracking()
                            .FirstOrDefaultAsync(x => x.ForSaleHousingListId == listingId);
                        if (house == null) return null;

                        detail = new AdminListingDetail
                        {
                            ListingTypeId = 2,
                            ListingId = house.ForSaleHousingListId,
                            IsRent = false,
                            Category = "housing",
                            CategoryDisplay = AdminListingConstants.CategoryDisplay("housing"),
                            PropertyNo = house.PropertyNo,
                            PropertyName = house.PropertyName,
                            PropertyDescription = house.PropertyDescription,
                            PropertyStatus = house.PropertyStatus,
                            CreatedDate = house.CreatedDate,
                            Price = house.Price,
                            City = house.City,
                            District = house.District,
                            CityId = house.CityId,
                            DistrictId = house.DistrictId,
                            NeighborhoodId = house.NeighborhoodId,
                            Neighborhood = house.Neighborhood,
                            AddressDesc = house.AddressDesc,
                            Facade = house.Facade,
                            IsElevator = house.IsElevator,
                            GrossArea = house.GrossArea,
                            NetArea = house.NetArea,
                            OpenArea = house.OpenArea,
                            BuildingAge = house.BuildingAge,
                            TotalNumberOfFloor = house.TotalNumberOfFloor,
                            NumberOfFloors = house.NumberOfFloors,
                            NumberOfBathRoom = house.NumberOfBathRoom,
                            NumberOfBathrooms = house.NumberOfBathRoom,
                            NumberOfRoom = house.NumberOfRoom,
                            Heating = house.Heating,
                            BlackBox = house.BlackBox,
                            NumberOfBalconies = house.NumberOfBalconies,
                            ParkingLot = house.ParkingLot,
                            Furnished = house.Furnished,
                            UsageStatus = house.UsageStatus,
                            WithinTheComplex = house.WithinTheComplex,
                            Dues = house.Dues,
                            Exchange = house.Exchange,
                            TitleDeedStatus = house.TitleDeedStatus,
                            HomeLoan = house.HomeLoan,
                            BestDeals = house.BestDeals,
                            HousingCategoryId = house.HousingCategoryId,
                            AgentId = house.AgentId
                        };

                        detail.AmenityIds = await _context.ForSaleHousingListingAmenities.AsNoTracking()
                            .Where(a => a.ForSaleHousingListId == listingId)
                            .Select(a => a.AmenityId)
                            .ToListAsync();
                        break;
                    }

                case 3:
                    {
                        var comm = await _context.forSaleCommercialPropertyListings.AsNoTracking()
                            .FirstOrDefaultAsync(x => x.ForSaleCommercialListingId == listingId);
                        if (comm == null) return null;

                        detail = new AdminListingDetail
                        {
                            ListingTypeId = 3,
                            ListingId = comm.ForSaleCommercialListingId,
                            IsRent = false,
                            Category = "commercial",
                            CategoryDisplay = AdminListingConstants.CategoryDisplay("commercial"),
                            PropertyNo = comm.PropertyNo,
                            PropertyName = comm.PropertyName,
                            PropertyDescription = comm.PropertyDescription,
                            PropertyStatus = comm.PropertyStatus,
                            CreatedDate = comm.CreatedDate,
                            Price = comm.Price,
                            City = comm.City,
                            District = comm.District,
                            CityId = comm.CityId,
                            DistrictId = comm.DistrictId,
                            NeighborhoodId = comm.NeighborhoodId,
                            Neighborhood = comm.Neighborhood,
                            AddressDesc = comm.AddressDesc,
                            Facade = comm.Facade,
                            NumberOfSection = comm.NumberOfSection,
                            NumberOfKitchens = comm.NumberOfKitchens,
                            NumberOfBathrooms = comm.NumberOfBathrooms,
                            NumberOfBathRoom = comm.NumberOfBathrooms,
                            NumberOfFloors = comm.NumberOfFloors,
                            GrossArea = comm.GrossArea,
                            Area = comm.Area,
                            TitleDeedStatus = comm.TitleDeedStatus,
                            SharePercentage = comm.SharePercentage,
                            LandLoan = comm.LandLoan,
                            Exchange = comm.Exchange,
                            Transferable = comm.Transferable,
                            BestDeals = comm.BestDeals,
                            AgentId = comm.AgentId
                        };

                        detail.AmenityIds = await _context.ForSaleCommercialListingAmenities.AsNoTracking()
                            .Where(a => a.ForSaleCommercialListingId == listingId)
                            .Select(a => a.AmenityId)
                            .ToListAsync();
                        break;
                    }

                case 7:
                    {
                        var rland = await _context.rentalLandListings.AsNoTracking()
                            .FirstOrDefaultAsync(x => x.RentalLandListingId == listingId);
                        if (rland == null) return null;

                        detail = new AdminListingDetail
                        {
                            ListingTypeId = 7,
                            ListingId = rland.RentalLandListingId,
                            IsRent = true,
                            Category = "land",
                            CategoryDisplay = AdminListingConstants.CategoryDisplay("land"),
                            PropertyNo = rland.PropertyNo,
                            PropertyName = rland.PropertyName,
                            PropertyDescription = rland.PropertyDescription,
                            PropertyStatus = rland.PropertyStatus,
                            CreatedDate = rland.CreatedDate,
                            Rent = rland.Rent,
                            Price = rland.Rent,
                            PricePerSquareMeter = rland.PricePerSquareMeter,
                            City = rland.City,
                            District = rland.District,
                            CityId = rland.CityId,
                            DistrictId = rland.DistrictId,
                            NeighborhoodId = rland.NeighborhoodId,
                            Neighborhood = rland.Neighborhood,
                            AddressDesc = rland.AddressDesc,
                            ZoningStatus = rland.ZoningStatus,
                            Area = rland.Area,
                            ParcelNumber = rland.ParcelNumber,
                            PlotNumber = rland.PlotNumber,
                            MapSheetNumber = rland.MapSheetNumber,
                            FloorAreaRatio = rland.FloorAreaRatio,
                            BaseAreaRatio = rland.BaseAreaRatio,
                            ZoningPlan = rland.ZoningPlan,
                            TitleDeedStatus = rland.TitleDeedStatus,
                            DevelopmentRight = rland.DevelopmentRight,
                            BestDeals = rland.BestDeals,
                            LandCategoryId = rland.LandCategoryId,
                            AgentId = rland.AgentId
                        };

                        detail.AmenityIds = await _context.RentalLandListingAmenities.AsNoTracking()
                            .Where(a => a.RentalLandListingId == listingId)
                            .Select(a => a.AmenityId)
                            .ToListAsync();
                        break;
                    }

                case 8:
                    {
                        var rhouse = await _context.rentalHousingListings.AsNoTracking()
                            .FirstOrDefaultAsync(x => x.RentalHousingListId == listingId);
                        if (rhouse == null) return null;

                        detail = new AdminListingDetail
                        {
                            ListingTypeId = 8,
                            ListingId = rhouse.RentalHousingListId,
                            IsRent = true,
                            Category = "housing",
                            CategoryDisplay = AdminListingConstants.CategoryDisplay("housing"),
                            PropertyNo = rhouse.PropertyNo,
                            PropertyName = rhouse.PropertyName,
                            PropertyDescription = rhouse.PropertyDescription,
                            PropertyStatus = rhouse.PropertyStatus,
                            CreatedDate = rhouse.CreatedDate,
                            Rent = rhouse.Rent,
                            Price = rhouse.Rent,
                            Deposit = rhouse.Deposit,
                            City = rhouse.City,
                            District = rhouse.District,
                            CityId = rhouse.CityId,
                            DistrictId = rhouse.DistrictId,
                            NeighborhoodId = rhouse.NeighborhoodId,
                            Neighborhood = rhouse.Neighborhood,
                            AddressDesc = rhouse.AddressDesc,
                            Facade = rhouse.Facade,
                            IsElevator = rhouse.IsElevator,
                            GrossArea = rhouse.GrossArea,
                            NetArea = rhouse.NetArea,
                            OpenArea = rhouse.OpenArea,
                            BuildingAge = rhouse.BuildingAge,
                            TotalNumberOfFloor = rhouse.TotalNumberOfFloor,
                            NumberOfFloors = rhouse.NumberOfFloors,
                            NumberOfBathRoom = rhouse.NumberOfBathRoom,
                            NumberOfBathrooms = rhouse.NumberOfBathRoom,
                            NumberOfRoom = rhouse.NumberOfRoom,
                            NumberOfBalconies = rhouse.NumberOfBalconies,
                            Heating = rhouse.Heating,
                            BlackBox = rhouse.BlackBox,
                            ParkingLot = rhouse.ParkingLot,
                            Furnished = rhouse.Furnished,
                            WithinTheComplex = rhouse.WithinTheComplex,
                            Dues = rhouse.Dues,
                            BestDeals = rhouse.BestDeals,
                            HousingCategoryId = rhouse.HousingCategoryId,
                            AgentId = rhouse.AgentId
                        };

                        detail.AmenityIds = await _context.RentalHousingListingAmenities.AsNoTracking()
                            .Where(a => a.RentalHousingListId == listingId)
                            .Select(a => a.AmenityId)
                            .ToListAsync();
                        break;
                    }

                case 9:
                    {
                        var rcomm = await _context.rentalCommercialPropertyListings.AsNoTracking()
                            .FirstOrDefaultAsync(x => x.RentalCommercialListId == listingId);
                        if (rcomm == null) return null;

                        detail = new AdminListingDetail
                        {
                            ListingTypeId = 9,
                            ListingId = rcomm.RentalCommercialListId,
                            IsRent = true,
                            Category = "commercial",
                            CategoryDisplay = AdminListingConstants.CategoryDisplay("commercial"),
                            PropertyNo = rcomm.PropertyNo,
                            PropertyName = rcomm.PropertyName,
                            PropertyDescription = rcomm.PropertyDescription,
                            PropertyStatus = rcomm.PropertyStatus,
                            CreatedDate = rcomm.CreatedDate,
                            Rent = rcomm.Rent,
                            Price = rcomm.Rent,
                            Deposit = rcomm.Deposit,
                            City = rcomm.City,
                            District = rcomm.District,
                            CityId = rcomm.CityId,
                            DistrictId = rcomm.DistrictId,
                            NeighborhoodId = rcomm.NeighborhoodId,
                            Neighborhood = rcomm.Neighborhood,
                            AddressDesc = rcomm.AddressDesc,
                            Facade = rcomm.Facade,
                            NumberOfSection = rcomm.NumberOfSection,
                            NumberOfKitchens = rcomm.NumberOfKitchens,
                            NumberOfBathrooms = rcomm.NumberOfBathrooms,
                            NumberOfBathRoom = rcomm.NumberOfBathrooms,
                            NumberOfFloors = rcomm.NumberOfFloors,
                            GrossArea = rcomm.GrossArea,
                            NetArea = rcomm.NetArea,
                            BuildingAge = rcomm.BuildingAge,
                            Heating = rcomm.Heating,
                            TitleDeedStatus = rcomm.TitleDeedStatus,
                            BestDeals = rcomm.BestDeals,
                            AgentId = rcomm.AgentId
                        };

                        detail.AmenityIds = await _context.RentalCommercialListingAmenities.AsNoTracking()
                            .Where(a => a.RentalCommercialListId == listingId)
                            .Select(a => a.AmenityId)
                            .ToListAsync();
                        break;
                    }

                default:
                    return null;
            }

            // Coğrafi konum (manual tercih edilir, yoksa en yenisi)
            var geoPoints = await _context.ListingGeoPoints.AsNoTracking()
                .Where(g => g.ListingTypeId == listingTypeId && g.ListingId == listingId)
                .ToListAsync();

            var geo = geoPoints.FirstOrDefault(g => string.Equals(g.Source, "manual", StringComparison.OrdinalIgnoreCase))
                   ?? geoPoints.OrderByDescending(g => g.CreatedDate).FirstOrDefault();

            if (geo != null)
            {
                detail.Latitude = geo.Latitude;
                detail.Longitude = geo.Longitude;
                detail.GeoPrecision = geo.Precision;
                detail.GeoSource = geo.Source;
            }

            // Danışman adı
            if (detail.AgentId.HasValue)
            {
                detail.AgentName = await _context.Agents.AsNoTracking()
                    .Where(a => a.AgentId == detail.AgentId.Value)
                    .Select(a => a.AgentName)
                    .FirstOrDefaultAsync();
            }

            return detail;
        }

        public async Task<List<AmenityDefinition>> GetAmenitiesAsync()
        {
            return await _context.AmenityDefinitions.AsNoTracking()
                .Where(a => a.isActive)
                .OrderBy(a => a.SortOrder)
                .ToListAsync();
        }

        public async Task<List<HousingCategory>> GetHousingCategoriesAsync()
        {
            return await _context.HousingCategories.AsNoTracking().ToListAsync();
        }

        public async Task<List<LandCategory>> GetLandCategoriesAsync()
        {
            return await _context.LandCategories.AsNoTracking().ToListAsync();
        }

        public async Task<List<(int AgentId, string AgentName)>> GetActiveAgentsAsync()
        {
            var list = await _context.Agents.AsNoTracking()
                .Where(a => a.IsActive)
                .OrderBy(a => a.AgentName)
                .Select(a => new { a.AgentId, a.AgentName })
                .ToListAsync();

            return list.Select(x => (x.AgentId, x.AgentName)).ToList();
        }

        #region Yardımcı Metotlar ve Birleşik Projeksiyon

        private IQueryable<ListingUnifiedEntry> BuildUnifiedQuery(HashSet<int> allowedTypeIds)
        {
            IQueryable<ListingUnifiedEntry>? query = null;

            void AppendSubquery(int typeId, IQueryable<ListingUnifiedEntry> sub)
            {
                if (allowedTypeIds.Contains(typeId))
                {
                    query = query == null ? sub : query.Concat(sub);
                }
            }

            AppendSubquery(1, _context.forSaleLandListings.AsNoTracking().Select(x => new ListingUnifiedEntry
            {
                ListingTypeId = 1,
                ListingId = x.ForSaleLandListingId,
                PropertyNo = x.PropertyNo,
                Title = x.PropertyName,
                PropertyStatus = x.PropertyStatus ?? string.Empty,
                Price = x.Price,
                City = x.City,
                District = x.District,
                Neighborhood = x.Neighborhood,
                CreatedDate = x.CreatedDate,
                AgentId = x.AgentId,
                BestDeals = x.BestDeals ?? false
            }));

            AppendSubquery(2, _context.forSaleHousingPropertyListings.AsNoTracking().Select(x => new ListingUnifiedEntry
            {
                ListingTypeId = 2,
                ListingId = x.ForSaleHousingListId,
                PropertyNo = x.PropertyNo,
                Title = x.PropertyName,
                PropertyStatus = x.PropertyStatus ?? string.Empty,
                Price = x.Price,
                City = x.City,
                District = x.District,
                Neighborhood = x.Neighborhood,
                CreatedDate = x.CreatedDate,
                AgentId = x.AgentId,
                BestDeals = x.BestDeals ?? false
            }));

            AppendSubquery(3, _context.forSaleCommercialPropertyListings.AsNoTracking().Select(x => new ListingUnifiedEntry
            {
                ListingTypeId = 3,
                ListingId = x.ForSaleCommercialListingId,
                PropertyNo = x.PropertyNo,
                Title = x.PropertyName,
                PropertyStatus = x.PropertyStatus ?? string.Empty,
                Price = x.Price,
                City = x.City,
                District = x.District,
                Neighborhood = x.Neighborhood,
                CreatedDate = x.CreatedDate,
                AgentId = x.AgentId,
                BestDeals = x.BestDeals ?? false
            }));

            AppendSubquery(7, _context.rentalLandListings.AsNoTracking().Select(x => new ListingUnifiedEntry
            {
                ListingTypeId = 7,
                ListingId = x.RentalLandListingId,
                PropertyNo = x.PropertyNo,
                Title = x.PropertyName,
                PropertyStatus = x.PropertyStatus ?? string.Empty,
                Price = x.Rent,
                City = x.City,
                District = x.District,
                Neighborhood = x.Neighborhood,
                CreatedDate = x.CreatedDate,
                AgentId = x.AgentId,
                BestDeals = x.BestDeals ?? false
            }));

            AppendSubquery(8, _context.rentalHousingListings.AsNoTracking().Select(x => new ListingUnifiedEntry
            {
                ListingTypeId = 8,
                ListingId = x.RentalHousingListId,
                PropertyNo = x.PropertyNo,
                Title = x.PropertyName,
                PropertyStatus = x.PropertyStatus ?? string.Empty,
                Price = x.Rent,
                City = x.City,
                District = x.District,
                Neighborhood = x.Neighborhood,
                CreatedDate = x.CreatedDate,
                AgentId = x.AgentId,
                BestDeals = x.BestDeals ?? false
            }));

            AppendSubquery(9, _context.rentalCommercialPropertyListings.AsNoTracking().Select(x => new ListingUnifiedEntry
            {
                ListingTypeId = 9,
                ListingId = x.RentalCommercialListId,
                PropertyNo = x.PropertyNo,
                Title = x.PropertyName,
                PropertyStatus = x.PropertyStatus ?? string.Empty,
                Price = x.Rent,
                City = x.City,
                District = x.District,
                Neighborhood = x.Neighborhood,
                CreatedDate = x.CreatedDate,
                AgentId = x.AgentId,
                BestDeals = x.BestDeals ?? false
            }));

            return query!;
        }

        private static Expression<Func<ListingUnifiedEntry, bool>> BuildScopePredicate(ListingScope scope)
        {
            var parameter = Expression.Parameter(typeof(ListingUnifiedEntry), "x");
            Expression? body = null;

            if (scope.OwnAgentId.HasValue)
            {
                var ownId = scope.OwnAgentId.Value;
                var agentIdProp = Expression.Property(parameter, nameof(ListingUnifiedEntry.AgentId));
                var ownIdConst = Expression.Constant(ownId);
                body = Expression.Equal(agentIdProp, ownIdConst);
            }

            if (scope.GrantedListings != null && scope.GrantedListings.Count > 0)
            {
                var grantsByType = scope.GrantedListings
                    .GroupBy(g => g.ListingTypeId)
                    .ToDictionary(g => g.Key, g => g.Select(x => x.ListingId).Distinct().ToList());

                var listingTypeIdProp = Expression.Property(parameter, nameof(ListingUnifiedEntry.ListingTypeId));
                var listingIdProp = Expression.Property(parameter, nameof(ListingUnifiedEntry.ListingId));
                var containsMethod = typeof(List<int>).GetMethod("Contains", new[] { typeof(int) })!;

                foreach (var (typeId, ids) in grantsByType)
                {
                    var typeIdConst = Expression.Constant(typeId);
                    var typeEqual = Expression.Equal(listingTypeIdProp, typeIdConst);

                    var idsConst = Expression.Constant(ids);
                    var idInList = Expression.Call(idsConst, containsMethod, listingIdProp);

                    var grantMatch = Expression.AndAlso(typeEqual, idInList);

                    body = body == null ? grantMatch : Expression.OrElse(body, grantMatch);
                }
            }

            if (body == null)
            {
                return x => false;
            }

            return Expression.Lambda<Func<ListingUnifiedEntry, bool>>(body, parameter);
        }

        private sealed class ListingUnifiedEntry
        {
            public int ListingTypeId { get; set; }
            public int ListingId { get; set; }
            public int? PropertyNo { get; set; }
            public string Title { get; set; } = string.Empty;
            public string PropertyStatus { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public string City { get; set; } = string.Empty;
            public string District { get; set; } = string.Empty;
            public string Neighborhood { get; set; } = string.Empty;
            public DateTime CreatedDate { get; set; }
            public int AgentId { get; set; }
            public bool BestDeals { get; set; }
        }

        #endregion
    }
}
