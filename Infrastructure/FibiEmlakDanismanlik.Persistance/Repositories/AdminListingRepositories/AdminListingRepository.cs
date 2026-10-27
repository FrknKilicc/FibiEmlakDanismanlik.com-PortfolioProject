using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.AdminListingCommands;
using FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces;
using FibiEmlakDanismanlik.Domain.Common;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;
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

        public async Task<(int ListingTypeId, int ListingId)> SaveAsync(SaveAdminListingCommand cmd, int agentId, bool isNew)
        {
            int listingId = cmd.ListingId ?? 0;

            switch (cmd.ListingTypeId)
            {
                case 1: // Satılık Arsa
                    {
                        ForSaleLandListing entity;
                        if (isNew)
                        {
                            entity = new ForSaleLandListing
                            {
                                CreatedDate = TurkeyClock.Now,
                                PropertyStatus = PropertyStatuses.Aktif,
                                BestDeals = false,
                                AgentId = agentId,
                                ListingTypeId = cmd.ListingTypeId,
                                PropImgUrl1 = string.Empty
                            };
                            _context.forSaleLandListings.Add(entity);
                        }
                        else
                        {
                            var existing = await _context.forSaleLandListings.FirstOrDefaultAsync(x => x.ForSaleLandListingId == listingId);
                            if (existing == null) throw new InvalidOperationException("İlan bulunamadı.");
                            entity = existing;
                            entity.AgentId = agentId;
                        }

                        // Panel formu ilan numarası göndermez; gönderilmediyse mevcut numara korunur
                        if (cmd.PropertyNo.HasValue)
                        {
                            entity.PropertyNo = cmd.PropertyNo;
                        }
                        entity.PropertyName = cmd.PropertyName ?? string.Empty;
                        entity.PropertyDescription = cmd.PropertyDescription ?? string.Empty;
                        entity.City = cmd.City ?? string.Empty;
                        entity.District = cmd.District ?? string.Empty;
                        entity.Neighborhood = cmd.Neighborhood ?? string.Empty;
                        entity.AddressDesc = cmd.AddressDesc ?? string.Empty;
                        entity.ZoningStatus = cmd.ZoningStatus ?? string.Empty;
                        entity.Area = cmd.Area ?? 0;
                        entity.Price = cmd.Price ?? 0;
                        entity.PricePerSquareMeter = cmd.PricePerSquareMeter ?? (cmd.Area > 0 ? (cmd.Price ?? 0) / (decimal)cmd.Area : 0);
                        entity.SharePercentage = cmd.SharePercentage;
                        entity.ParcelNumber = cmd.ParcelNumber;
                        entity.PlotNumber = cmd.PlotNumber;
                        entity.MapSheetNumber = cmd.MapSheetNumber;
                        entity.FloorAreaRatio = cmd.FloorAreaRatio;
                        entity.BaseAreaRatio = cmd.BaseAreaRatio;
                        entity.ZoningPlan = cmd.ZoningPlan;
                        entity.TitleDeedStatus = cmd.TitleDeedStatus ?? string.Empty;
                        entity.LandLoan = cmd.LandLoan;
                        entity.Exchange = cmd.Exchange;
                        entity.LandCategoryId = cmd.LandCategoryId ?? 0;
                        entity.DevelopmentRight = bool.TryParse(cmd.DevelopmentRight, out var drBool) ? drBool : (bool?)null;

                        if (isNew)
                        {
                            await _context.SaveChangesAsync();
                            listingId = entity.ForSaleLandListingId;
                        }

                        await ReplaceAmenitiesAsync(1, listingId, cmd.AmenityIds);
                        await _context.SaveChangesAsync();
                        return (1, listingId);
                    }

                case 2: // Satılık Konut
                    {
                        ForSaleHousingListing entity;
                        if (isNew)
                        {
                            entity = new ForSaleHousingListing
                            {
                                CreatedDate = TurkeyClock.Now,
                                PropertyStatus = PropertyStatuses.Aktif,
                                BestDeals = false,
                                AgentId = agentId,
                                ListingTypeId = cmd.ListingTypeId,
                                PropImgUrl1 = string.Empty
                            };
                            _context.forSaleHousingPropertyListings.Add(entity);
                        }
                        else
                        {
                            var existing = await _context.forSaleHousingPropertyListings.FirstOrDefaultAsync(x => x.ForSaleHousingListId == listingId);
                            if (existing == null) throw new InvalidOperationException("İlan bulunamadı.");
                            entity = existing;
                            entity.AgentId = agentId;
                        }

                        // Panel formu ilan numarası göndermez; gönderilmediyse mevcut numara korunur
                        if (cmd.PropertyNo.HasValue)
                        {
                            entity.PropertyNo = cmd.PropertyNo;
                        }
                        entity.PropertyName = cmd.PropertyName ?? string.Empty;
                        entity.PropertyDescription = cmd.PropertyDescription ?? string.Empty;
                        entity.City = cmd.City ?? string.Empty;
                        entity.District = cmd.District ?? string.Empty;
                        entity.Neighborhood = cmd.Neighborhood ?? string.Empty;
                        entity.AddressDesc = cmd.AddressDesc;
                        entity.Price = cmd.Price ?? 0;
                        entity.Facade = cmd.Facade;
                        entity.IsElevator = cmd.IsElevator ?? false;
                        entity.GrossArea = cmd.GrossArea ?? 0;
                        entity.NetArea = cmd.NetArea ?? 0;
                        entity.OpenArea = cmd.OpenArea;
                        entity.BuildingAge = cmd.BuildingAge ?? string.Empty;
                        entity.TotalNumberOfFloor = cmd.TotalNumberOfFloor ?? 0;
                        entity.NumberOfFloors = cmd.NumberOfFloors ?? 0;
                        entity.NumberOfBathRoom = cmd.NumberOfBathRoom ?? cmd.NumberOfBathrooms;
                        entity.NumberOfRoom = cmd.NumberOfRoom ?? string.Empty;
                        entity.Heating = cmd.Heating ?? string.Empty;
                        entity.BlackBox = cmd.BlackBox;
                        entity.NumberOfBalconies = cmd.NumberOfBalconies;
                        entity.ParkingLot = cmd.ParkingLot;
                        entity.Furnished = cmd.Furnished;
                        entity.UsageStatus = cmd.UsageStatus ?? string.Empty;
                        entity.WithinTheComplex = cmd.WithinTheComplex;
                        entity.Dues = cmd.Dues;
                        entity.Exchange = cmd.Exchange ?? false;
                        entity.TitleDeedStatus = cmd.TitleDeedStatus ?? string.Empty;
                        entity.HomeLoan = cmd.HomeLoan ?? false;
                        entity.HousingCategoryId = cmd.HousingCategoryId ?? 0;

                        if (isNew)
                        {
                            await _context.SaveChangesAsync();
                            listingId = entity.ForSaleHousingListId;
                        }

                        await ReplaceAmenitiesAsync(2, listingId, cmd.AmenityIds);
                        await _context.SaveChangesAsync();
                        return (2, listingId);
                    }

                case 3: // Satılık İşyeri
                    {
                        ForSaleCommercialPropertyListing entity;
                        if (isNew)
                        {
                            entity = new ForSaleCommercialPropertyListing
                            {
                                CreatedDate = TurkeyClock.Now,
                                PropertyStatus = PropertyStatuses.Aktif,
                                BestDeals = false,
                                AgentId = agentId,
                                ListingTypeId = cmd.ListingTypeId,
                                PropImgUrl1 = string.Empty
                            };
                            _context.forSaleCommercialPropertyListings.Add(entity);
                        }
                        else
                        {
                            var existing = await _context.forSaleCommercialPropertyListings.FirstOrDefaultAsync(x => x.ForSaleCommercialListingId == listingId);
                            if (existing == null) throw new InvalidOperationException("İlan bulunamadı.");
                            entity = existing;
                            entity.AgentId = agentId;
                        }

                        // Panel formu ilan numarası göndermez; gönderilmediyse mevcut numara korunur
                        if (cmd.PropertyNo.HasValue)
                        {
                            entity.PropertyNo = cmd.PropertyNo;
                        }
                        entity.PropertyName = cmd.PropertyName ?? string.Empty;
                        entity.PropertyDescription = cmd.PropertyDescription ?? string.Empty;
                        entity.City = cmd.City ?? string.Empty;
                        entity.District = cmd.District ?? string.Empty;
                        entity.Neighborhood = cmd.Neighborhood ?? string.Empty;
                        entity.AddressDesc = cmd.AddressDesc ?? string.Empty;
                        entity.Price = cmd.Price ?? 0;
                        entity.Facade = cmd.Facade;
                        entity.NumberOfSection = cmd.NumberOfSection;
                        entity.NumberOfKitchens = cmd.NumberOfKitchens;
                        entity.NumberOfBathrooms = cmd.NumberOfBathrooms ?? cmd.NumberOfBathRoom;
                        entity.NumberOfFloors = cmd.NumberOfFloors ?? 0;
                        entity.GrossArea = cmd.GrossArea ?? 0;
                        entity.Area = cmd.Area;
                        entity.TitleDeedStatus = cmd.TitleDeedStatus ?? string.Empty;
                        entity.SharePercentage = cmd.SharePercentage;
                        entity.LandLoan = cmd.LandLoan ?? false;
                        entity.Exchange = cmd.Exchange ?? false;
                        entity.Transferable = cmd.Transferable ?? false;

                        if (isNew)
                        {
                            await _context.SaveChangesAsync();
                            listingId = entity.ForSaleCommercialListingId;
                        }

                        await ReplaceAmenitiesAsync(3, listingId, cmd.AmenityIds);
                        await _context.SaveChangesAsync();
                        return (3, listingId);
                    }

                case 7: // Kiralık Arsa
                    {
                        RentalLandListing entity;
                        if (isNew)
                        {
                            entity = new RentalLandListing
                            {
                                CreatedDate = TurkeyClock.Now,
                                PropertyStatus = PropertyStatuses.Aktif,
                                BestDeals = false,
                                AgentId = agentId,
                                ListingTypeId = cmd.ListingTypeId,
                                PropImgUrl1 = string.Empty
                            };
                            _context.rentalLandListings.Add(entity);
                        }
                        else
                        {
                            var existing = await _context.rentalLandListings.FirstOrDefaultAsync(x => x.RentalLandListingId == listingId);
                            if (existing == null) throw new InvalidOperationException("İlan bulunamadı.");
                            entity = existing;
                            entity.AgentId = agentId;
                        }

                        // Panel formu ilan numarası göndermez; gönderilmediyse mevcut numara korunur
                        if (cmd.PropertyNo.HasValue)
                        {
                            entity.PropertyNo = cmd.PropertyNo;
                        }
                        entity.PropertyName = cmd.PropertyName ?? string.Empty;
                        entity.PropertyDescription = cmd.PropertyDescription ?? string.Empty;
                        entity.City = cmd.City ?? string.Empty;
                        entity.District = cmd.District ?? string.Empty;
                        entity.Neighborhood = cmd.Neighborhood ?? string.Empty;
                        entity.AddressDesc = cmd.AddressDesc ?? string.Empty;
                        entity.Rent = cmd.Rent ?? cmd.Price ?? 0;
                        entity.PricePerSquareMeter = cmd.PricePerSquareMeter ?? (cmd.Area > 0 ? ((cmd.Rent ?? cmd.Price ?? 0) / (decimal)cmd.Area) : 0);
                        entity.Area = cmd.Area ?? 0;
                        entity.ZoningStatus = cmd.ZoningStatus ?? string.Empty;
                        entity.ParcelNumber = cmd.ParcelNumber;
                        entity.PlotNumber = cmd.PlotNumber;
                        entity.MapSheetNumber = cmd.MapSheetNumber;
                        entity.FloorAreaRatio = cmd.FloorAreaRatio;
                        entity.BaseAreaRatio = cmd.BaseAreaRatio;
                        entity.ZoningPlan = cmd.ZoningPlan;
                        entity.TitleDeedStatus = cmd.TitleDeedStatus;
                        entity.DevelopmentRight = cmd.DevelopmentRight;
                        entity.LandCategoryId = cmd.LandCategoryId ?? 0;

                        if (isNew)
                        {
                            await _context.SaveChangesAsync();
                            listingId = entity.RentalLandListingId;
                        }

                        await ReplaceAmenitiesAsync(7, listingId, cmd.AmenityIds);
                        await _context.SaveChangesAsync();
                        return (7, listingId);
                    }

                case 8: // Kiralık Konut
                    {
                        RentalHousingListing entity;
                        if (isNew)
                        {
                            entity = new RentalHousingListing
                            {
                                CreatedDate = TurkeyClock.Now,
                                PropertyStatus = PropertyStatuses.Aktif,
                                BestDeals = false,
                                AgentId = agentId,
                                ListingTypeId = cmd.ListingTypeId,
                                PropImgUrl1 = string.Empty
                            };
                            _context.rentalHousingListings.Add(entity);
                        }
                        else
                        {
                            var existing = await _context.rentalHousingListings.FirstOrDefaultAsync(x => x.RentalHousingListId == listingId);
                            if (existing == null) throw new InvalidOperationException("İlan bulunamadı.");
                            entity = existing;
                            entity.AgentId = agentId;
                        }

                        // Panel formu ilan numarası göndermez; gönderilmediyse mevcut numara korunur
                        if (cmd.PropertyNo.HasValue)
                        {
                            entity.PropertyNo = cmd.PropertyNo;
                        }
                        entity.PropertyName = cmd.PropertyName ?? string.Empty;
                        entity.PropertyDescription = cmd.PropertyDescription ?? string.Empty;
                        entity.City = cmd.City ?? string.Empty;
                        entity.District = cmd.District ?? string.Empty;
                        entity.Neighborhood = cmd.Neighborhood ?? string.Empty;
                        entity.AddressDesc = cmd.AddressDesc;
                        entity.Rent = cmd.Rent ?? cmd.Price ?? 0;
                        entity.Deposit = cmd.Deposit ?? 0;
                        entity.Facade = cmd.Facade;
                        entity.IsElevator = cmd.IsElevator ?? false;
                        entity.GrossArea = cmd.GrossArea ?? 0;
                        entity.NetArea = cmd.NetArea ?? 0;
                        entity.OpenArea = cmd.OpenArea;
                        entity.BuildingAge = cmd.BuildingAge ?? string.Empty;
                        entity.TotalNumberOfFloor = cmd.TotalNumberOfFloor ?? 0;
                        entity.NumberOfFloors = cmd.NumberOfFloors ?? 0;
                        entity.NumberOfBathRoom = cmd.NumberOfBathRoom ?? cmd.NumberOfBathrooms;
                        entity.NumberOfRoom = cmd.NumberOfRoom ?? string.Empty;
                        entity.NumberOfBalconies = cmd.NumberOfBalconies;
                        entity.Heating = cmd.Heating ?? string.Empty;
                        entity.BlackBox = cmd.BlackBox;
                        entity.ParkingLot = cmd.ParkingLot;
                        entity.Furnished = cmd.Furnished;
                        entity.WithinTheComplex = cmd.WithinTheComplex;
                        entity.Dues = cmd.Dues;
                        entity.HousingCategoryId = cmd.HousingCategoryId ?? 0;

                        if (isNew)
                        {
                            await _context.SaveChangesAsync();
                            listingId = entity.RentalHousingListId;
                        }

                        await ReplaceAmenitiesAsync(8, listingId, cmd.AmenityIds);
                        await _context.SaveChangesAsync();
                        return (8, listingId);
                    }

                case 9: // Kiralık İşyeri
                    {
                        RentalCommercialPropertyListing entity;
                        if (isNew)
                        {
                            entity = new RentalCommercialPropertyListing
                            {
                                CreatedDate = TurkeyClock.Now,
                                PropertyStatus = PropertyStatuses.Aktif,
                                BestDeals = false,
                                AgentId = agentId,
                                ListingTypeId = cmd.ListingTypeId,
                                PropImgUrl1 = string.Empty
                            };
                            _context.rentalCommercialPropertyListings.Add(entity);
                        }
                        else
                        {
                            var existing = await _context.rentalCommercialPropertyListings.FirstOrDefaultAsync(x => x.RentalCommercialListId == listingId);
                            if (existing == null) throw new InvalidOperationException("İlan bulunamadı.");
                            entity = existing;
                            entity.AgentId = agentId;
                        }

                        // Panel formu ilan numarası göndermez; gönderilmediyse mevcut numara korunur
                        if (cmd.PropertyNo.HasValue)
                        {
                            entity.PropertyNo = cmd.PropertyNo;
                        }
                        entity.PropertyName = cmd.PropertyName ?? string.Empty;
                        entity.PropertyDescription = cmd.PropertyDescription ?? string.Empty;
                        entity.City = cmd.City ?? string.Empty;
                        entity.District = cmd.District ?? string.Empty;
                        entity.Neighborhood = cmd.Neighborhood ?? string.Empty;
                        entity.AddressDesc = cmd.AddressDesc;
                        entity.Rent = cmd.Rent ?? cmd.Price ?? 0;
                        entity.Deposit = cmd.Deposit ?? 0;
                        entity.Facade = cmd.Facade;
                        entity.NumberOfSection = cmd.NumberOfSection;
                        entity.NumberOfKitchens = cmd.NumberOfKitchens;
                        entity.NumberOfBathrooms = cmd.NumberOfBathrooms ?? cmd.NumberOfBathRoom;
                        entity.NumberOfFloors = cmd.NumberOfFloors ?? 0;
                        entity.GrossArea = cmd.GrossArea;
                        entity.NetArea = cmd.NetArea;
                        entity.BuildingAge = cmd.BuildingAge ?? string.Empty;
                        entity.Heating = cmd.Heating ?? string.Empty;
                        entity.TitleDeedStatus = cmd.TitleDeedStatus;

                        if (isNew)
                        {
                            await _context.SaveChangesAsync();
                            listingId = entity.RentalCommercialListId;
                        }

                        await ReplaceAmenitiesAsync(9, listingId, cmd.AmenityIds);
                        await _context.SaveChangesAsync();
                        return (9, listingId);
                    }

                default:
                    throw new ArgumentException($"Desteklenmeyen ilan türü: {cmd.ListingTypeId}");
            }
        }

        private async Task ReplaceAmenitiesAsync(int listingTypeId, int listingId, List<int>? requestedAmenityIds)
        {
            var category = AdminListingConstants.Category(listingTypeId);
            var activeAmenities = await _context.AmenityDefinitions
                .AsNoTracking()
                .Where(a => a.isActive)
                .ToListAsync();

            var validAmenityIds = (requestedAmenityIds ?? new List<int>())
                .Distinct()
                .Where(id => activeAmenities.Any(a => a.AmenityId == id && category switch
                {
                    "land" => a.AppliesToLand,
                    "housing" => a.AppliesToHousing,
                    "commercial" => a.AppliesToCommercial,
                    _ => false
                }))
                .ToList();

            switch (listingTypeId)
            {
                case 1:
                    {
                        var current = await _context.ForSaleLandListingAmenities
                            .Where(x => x.ForSaleLandListId == listingId)
                            .ToListAsync();
                        _context.ForSaleLandListingAmenities.RemoveRange(current);

                        if (validAmenityIds.Count > 0)
                        {
                            var newRows = validAmenityIds.Select(id => new ForSaleLandListingAmenity
                            {
                                ForSaleLandListId = listingId,
                                AmenityId = id
                            });
                            _context.ForSaleLandListingAmenities.AddRange(newRows);
                        }
                        break;
                    }

                case 2:
                    {
                        var current = await _context.ForSaleHousingListingAmenities
                            .Where(x => x.ForSaleHousingListId == listingId)
                            .ToListAsync();
                        _context.ForSaleHousingListingAmenities.RemoveRange(current);

                        if (validAmenityIds.Count > 0)
                        {
                            var newRows = validAmenityIds.Select(id => new ForSaleHousingListingAmenity
                            {
                                ForSaleHousingListId = listingId,
                                AmenityId = id
                            });
                            _context.ForSaleHousingListingAmenities.AddRange(newRows);
                        }
                        break;
                    }

                case 3:
                    {
                        var current = await _context.ForSaleCommercialListingAmenities
                            .Where(x => x.ForSaleCommercialListingId == listingId)
                            .ToListAsync();
                        _context.ForSaleCommercialListingAmenities.RemoveRange(current);

                        if (validAmenityIds.Count > 0)
                        {
                            var newRows = validAmenityIds.Select(id => new ForSaleCommercialListingAmenity
                            {
                                ForSaleCommercialListingId = listingId,
                                AmenityId = id
                            });
                            _context.ForSaleCommercialListingAmenities.AddRange(newRows);
                        }
                        break;
                    }

                case 7:
                    {
                        var current = await _context.RentalLandListingAmenities
                            .Where(x => x.RentalLandListingId == listingId)
                            .ToListAsync();
                        _context.RentalLandListingAmenities.RemoveRange(current);

                        if (validAmenityIds.Count > 0)
                        {
                            var newRows = validAmenityIds.Select(id => new RentalLandListingAmenities
                            {
                                RentalLandListingId = listingId,
                                AmenityId = id
                            });
                            _context.RentalLandListingAmenities.AddRange(newRows);
                        }
                        break;
                    }

                case 8:
                    {
                        var current = await _context.RentalHousingListingAmenities
                            .Where(x => x.RentalHousingListId == listingId)
                            .ToListAsync();
                        _context.RentalHousingListingAmenities.RemoveRange(current);

                        if (validAmenityIds.Count > 0)
                        {
                            var newRows = validAmenityIds.Select(id => new RentalHousingListingAmenities
                            {
                                RentalHousingListId = listingId,
                                AmenityId = id
                            });
                            _context.RentalHousingListingAmenities.AddRange(newRows);
                        }
                        break;
                    }

                case 9:
                    {
                        var current = await _context.RentalCommercialListingAmenities
                            .Where(x => x.RentalCommercialListId == listingId)
                            .ToListAsync();
                        _context.RentalCommercialListingAmenities.RemoveRange(current);

                        if (validAmenityIds.Count > 0)
                        {
                            var newRows = validAmenityIds.Select(id => new RentalCommercialListingAmenities
                            {
                                RentalCommercialListId = listingId,
                                AmenityId = id
                            });
                            _context.RentalCommercialListingAmenities.AddRange(newRows);
                        }
                        break;
                    }
            }
        }

        public async Task<bool> SetStatusAsync(int listingTypeId, int listingId, string status)
        {
            switch (listingTypeId)
            {
                case 1:
                    {
                        var item = await _context.forSaleLandListings.FirstOrDefaultAsync(x => x.ForSaleLandListingId == listingId);
                        if (item == null) return false;
                        item.PropertyStatus = status;
                        if (status != PropertyStatuses.Aktif && status != PropertyStatuses.Opsiyonlu) item.BestDeals = false;
                        break;
                    }
                case 2:
                    {
                        var item = await _context.forSaleHousingPropertyListings.FirstOrDefaultAsync(x => x.ForSaleHousingListId == listingId);
                        if (item == null) return false;
                        item.PropertyStatus = status;
                        if (status != PropertyStatuses.Aktif && status != PropertyStatuses.Opsiyonlu) item.BestDeals = false;
                        break;
                    }
                case 3:
                    {
                        var item = await _context.forSaleCommercialPropertyListings.FirstOrDefaultAsync(x => x.ForSaleCommercialListingId == listingId);
                        if (item == null) return false;
                        item.PropertyStatus = status;
                        if (status != PropertyStatuses.Aktif && status != PropertyStatuses.Opsiyonlu) item.BestDeals = false;
                        break;
                    }
                case 7:
                    {
                        var item = await _context.rentalLandListings.FirstOrDefaultAsync(x => x.RentalLandListingId == listingId);
                        if (item == null) return false;
                        item.PropertyStatus = status;
                        if (status != PropertyStatuses.Aktif && status != PropertyStatuses.Opsiyonlu) item.BestDeals = false;
                        break;
                    }
                case 8:
                    {
                        var item = await _context.rentalHousingListings.FirstOrDefaultAsync(x => x.RentalHousingListId == listingId);
                        if (item == null) return false;
                        item.PropertyStatus = status;
                        if (status != PropertyStatuses.Aktif && status != PropertyStatuses.Opsiyonlu) item.BestDeals = false;
                        break;
                    }
                case 9:
                    {
                        var item = await _context.rentalCommercialPropertyListings.FirstOrDefaultAsync(x => x.RentalCommercialListId == listingId);
                        if (item == null) return false;
                        item.PropertyStatus = status;
                        if (status != PropertyStatuses.Aktif && status != PropertyStatuses.Opsiyonlu) item.BestDeals = false;
                        break;
                    }
                default:
                    return false;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string?> GetListingStatusAsync(int listingTypeId, int listingId)
        {
            return listingTypeId switch
            {
                1 => await _context.forSaleLandListings.AsNoTracking().Where(x => x.ForSaleLandListingId == listingId).Select(x => x.PropertyStatus).FirstOrDefaultAsync(),
                2 => await _context.forSaleHousingPropertyListings.AsNoTracking().Where(x => x.ForSaleHousingListId == listingId).Select(x => x.PropertyStatus).FirstOrDefaultAsync(),
                3 => await _context.forSaleCommercialPropertyListings.AsNoTracking().Where(x => x.ForSaleCommercialListingId == listingId).Select(x => x.PropertyStatus).FirstOrDefaultAsync(),
                7 => await _context.rentalLandListings.AsNoTracking().Where(x => x.RentalLandListingId == listingId).Select(x => x.PropertyStatus).FirstOrDefaultAsync(),
                8 => await _context.rentalHousingListings.AsNoTracking().Where(x => x.RentalHousingListId == listingId).Select(x => x.PropertyStatus).FirstOrDefaultAsync(),
                9 => await _context.rentalCommercialPropertyListings.AsNoTracking().Where(x => x.RentalCommercialListId == listingId).Select(x => x.PropertyStatus).FirstOrDefaultAsync(),
                _ => null
            };
        }

        public async Task<int> CountBestDealsAsync(bool isRent)
        {
            if (!isRent)
            {
                var land = await _context.forSaleLandListings.CountAsync(x => x.BestDeals == true);
                var house = await _context.forSaleHousingPropertyListings.CountAsync(x => x.BestDeals == true);
                var comm = await _context.forSaleCommercialPropertyListings.CountAsync(x => x.BestDeals == true);
                return land + house + comm;
            }
            else
            {
                var rland = await _context.rentalLandListings.CountAsync(x => x.BestDeals == true);
                var rhouse = await _context.rentalHousingListings.CountAsync(x => x.BestDeals == true);
                var rcomm = await _context.rentalCommercialPropertyListings.CountAsync(x => x.BestDeals == true);
                return rland + rhouse + rcomm;
            }
        }

        public async Task<bool> SetBestDealAsync(int listingTypeId, int listingId, bool isBestDeal)
        {
            switch (listingTypeId)
            {
                case 1:
                    {
                        var item = await _context.forSaleLandListings.FirstOrDefaultAsync(x => x.ForSaleLandListingId == listingId);
                        if (item == null) return false;
                        item.BestDeals = isBestDeal;
                        break;
                    }
                case 2:
                    {
                        var item = await _context.forSaleHousingPropertyListings.FirstOrDefaultAsync(x => x.ForSaleHousingListId == listingId);
                        if (item == null) return false;
                        item.BestDeals = isBestDeal;
                        break;
                    }
                case 3:
                    {
                        var item = await _context.forSaleCommercialPropertyListings.FirstOrDefaultAsync(x => x.ForSaleCommercialListingId == listingId);
                        if (item == null) return false;
                        item.BestDeals = isBestDeal;
                        break;
                    }
                case 7:
                    {
                        var item = await _context.rentalLandListings.FirstOrDefaultAsync(x => x.RentalLandListingId == listingId);
                        if (item == null) return false;
                        item.BestDeals = isBestDeal;
                        break;
                    }
                case 8:
                    {
                        var item = await _context.rentalHousingListings.FirstOrDefaultAsync(x => x.RentalHousingListId == listingId);
                        if (item == null) return false;
                        item.BestDeals = isBestDeal;
                        break;
                    }
                case 9:
                    {
                        var item = await _context.rentalCommercialPropertyListings.FirstOrDefaultAsync(x => x.RentalCommercialListId == listingId);
                        if (item == null) return false;
                        item.BestDeals = isBestDeal;
                        break;
                    }
                default:
                    return false;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetGeoPointAsync(int listingTypeId, int listingId, decimal latitude, decimal longitude)
        {
            var existing = await _context.ListingGeoPoints
                .Where(g => g.ListingTypeId == listingTypeId && g.ListingId == listingId)
                .ToListAsync();

            if (existing.Count > 0)
            {
                _context.ListingGeoPoints.RemoveRange(existing);
            }

            _context.ListingGeoPoints.Add(new ListingGeoPoint
            {
                ListingTypeId = listingTypeId,
                ListingId = listingId,
                Latitude = latitude,
                Longitude = longitude,
                Source = ListingGeoConstants.SourceManual,
                Precision = ListingGeoConstants.PrecisionBuilding,
                CreatedDate = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<string>> DeleteListingAsync(int listingTypeId, int listingId)
        {
            // 1. Olanak ara kayıtlarını sil
            switch (listingTypeId)
            {
                case 1:
                    {
                        var ams = await _context.ForSaleLandListingAmenities.Where(x => x.ForSaleLandListId == listingId).ToListAsync();
                        if (ams.Count > 0) _context.ForSaleLandListingAmenities.RemoveRange(ams);
                        break;
                    }
                case 2:
                    {
                        var ams = await _context.ForSaleHousingListingAmenities.Where(x => x.ForSaleHousingListId == listingId).ToListAsync();
                        if (ams.Count > 0) _context.ForSaleHousingListingAmenities.RemoveRange(ams);
                        break;
                    }
                case 3:
                    {
                        var ams = await _context.ForSaleCommercialListingAmenities.Where(x => x.ForSaleCommercialListingId == listingId).ToListAsync();
                        if (ams.Count > 0) _context.ForSaleCommercialListingAmenities.RemoveRange(ams);
                        break;
                    }
                case 7:
                    {
                        var ams = await _context.RentalLandListingAmenities.Where(x => x.RentalLandListingId == listingId).ToListAsync();
                        if (ams.Count > 0) _context.RentalLandListingAmenities.RemoveRange(ams);
                        break;
                    }
                case 8:
                    {
                        var ams = await _context.RentalHousingListingAmenities.Where(x => x.RentalHousingListId == listingId).ToListAsync();
                        if (ams.Count > 0) _context.RentalHousingListingAmenities.RemoveRange(ams);
                        break;
                    }
                case 9:
                    {
                        var ams = await _context.RentalCommercialListingAmenities.Where(x => x.RentalCommercialListId == listingId).ToListAsync();
                        if (ams.Count > 0) _context.RentalCommercialListingAmenities.RemoveRange(ams);
                        break;
                    }
            }

            // 2. ListingGeoPoints sil
            var geoPoints = await _context.ListingGeoPoints
                .Where(g => g.ListingTypeId == listingTypeId && g.ListingId == listingId)
                .ToListAsync();
            if (geoPoints.Count > 0) _context.ListingGeoPoints.RemoveRange(geoPoints);

            // 3. ListingAccessGrants sil
            var grants = await _context.ListingAccessGrants
                .Where(g => g.ListingTypeId == listingTypeId && g.ListingId == listingId)
                .ToListAsync();
            if (grants.Count > 0) _context.ListingAccessGrants.RemoveRange(grants);

            // 4. ListingNearbyPlaces sil
            var places = await _context.ListingNearbyPlaces
                .Where(p => p.ListingTypeId == listingTypeId && p.ListingId == listingId)
                .ToListAsync();
            if (places.Count > 0) _context.ListingNearbyPlaces.RemoveRange(places);

            // 5. ListingImages sil ve StoragePath listesini topla
            var images = await _context.ListingImages
                .Where(i => i.ListingTypeId == listingTypeId && i.ListingId == listingId)
                .ToListAsync();
            var storagePaths = images
                .Where(i => !string.IsNullOrWhiteSpace(i.StoragePath))
                .Select(i => i.StoragePath!)
                .ToList();
            if (images.Count > 0) _context.ListingImages.RemoveRange(images);

            // 6. İlan ana kaydını sil
            switch (listingTypeId)
            {
                case 1:
                    {
                        var entity = await _context.forSaleLandListings.FirstOrDefaultAsync(x => x.ForSaleLandListingId == listingId);
                        if (entity != null) _context.forSaleLandListings.Remove(entity);
                        break;
                    }
                case 2:
                    {
                        var entity = await _context.forSaleHousingPropertyListings.FirstOrDefaultAsync(x => x.ForSaleHousingListId == listingId);
                        if (entity != null) _context.forSaleHousingPropertyListings.Remove(entity);
                        break;
                    }
                case 3:
                    {
                        var entity = await _context.forSaleCommercialPropertyListings.FirstOrDefaultAsync(x => x.ForSaleCommercialListingId == listingId);
                        if (entity != null) _context.forSaleCommercialPropertyListings.Remove(entity);
                        break;
                    }
                case 7:
                    {
                        var entity = await _context.rentalLandListings.FirstOrDefaultAsync(x => x.RentalLandListingId == listingId);
                        if (entity != null) _context.rentalLandListings.Remove(entity);
                        break;
                    }
                case 8:
                    {
                        var entity = await _context.rentalHousingListings.FirstOrDefaultAsync(x => x.RentalHousingListId == listingId);
                        if (entity != null) _context.rentalHousingListings.Remove(entity);
                        break;
                    }
                case 9:
                    {
                        var entity = await _context.rentalCommercialPropertyListings.FirstOrDefaultAsync(x => x.RentalCommercialListId == listingId);
                        if (entity != null) _context.rentalCommercialPropertyListings.Remove(entity);
                        break;
                    }
            }

            await _context.SaveChangesAsync();
            return storagePaths;
        }

        public async Task<List<ListingAccessGrant>> GetGrantsForListingAsync(int listingTypeId, int listingId)
        {
            return await _context.ListingAccessGrants
                .AsNoTracking()
                .Where(g => g.ListingTypeId == listingTypeId && g.ListingId == listingId)
                .OrderByDescending(g => g.GrantedAtUtc)
                .ToListAsync();
        }

        public async Task AddOrUpdateGrantAsync(int listingTypeId, int listingId, int targetAgentId, ListingAccessLevel access, int grantedByAgentId)
        {
            var existing = await _context.ListingAccessGrants
                .FirstOrDefaultAsync(g => g.ListingTypeId == listingTypeId && g.ListingId == listingId && g.AgentId == targetAgentId);

            if (existing != null)
            {
                existing.Access = access;
                existing.GrantedByAgentId = grantedByAgentId;
                existing.GrantedAtUtc = DateTime.UtcNow;
            }
            else
            {
                _context.ListingAccessGrants.Add(new ListingAccessGrant
                {
                    ListingTypeId = listingTypeId,
                    ListingId = listingId,
                    AgentId = targetAgentId,
                    Access = access,
                    GrantedByAgentId = grantedByAgentId,
                    GrantedAtUtc = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveGrantAsync(int listingAccessGrantId)
        {
            var grant = await _context.ListingAccessGrants
                .FirstOrDefaultAsync(g => g.ListingAccessGrantId == listingAccessGrantId);

            if (grant == null) return false;

            _context.ListingAccessGrants.Remove(grant);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AgentExistsAndActiveAsync(int agentId)
        {
            return await _context.Agents.AsNoTracking().AnyAsync(a => a.AgentId == agentId && a.IsActive);
        }

        public async Task<bool> IsActiveAgentAsync(int agentId)
        {
            return await _context.Agents.AsNoTracking().AnyAsync(a => a.AgentId == agentId && a.IsActive);
        }

        public async Task<bool> HousingCategoryExistsAsync(int id)
        {
            return await _context.HousingCategories.AsNoTracking().AnyAsync(c => c.HousingCategoryId == id);
        }

        public async Task<bool> LandCategoryExistsAsync(int id)
        {
            return await _context.LandCategories.AsNoTracking().AnyAsync(c => c.LandCategoryId == id);
        }

        public async Task<Agent?> GetAgentByIdAsync(int agentId)
        {
            return await _context.Agents.AsNoTracking().FirstOrDefaultAsync(a => a.AgentId == agentId);
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

        public async Task<(List<NearbyCategory> Categories, List<ListingNearbyPlace> Places)> GetNearbyPlacesAsync(int listingTypeId, int listingId)
        {
            var categories = await _context.NearbyCategories.AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.SortOrder)
                .ToListAsync();

            var places = await _context.ListingNearbyPlaces.AsNoTracking()
                .Include(p => p.NearbyCategory)
                .Where(p => p.ListingTypeId == listingTypeId && p.ListingId == listingId)
                .OrderBy(p => p.NearbyCategory!.SortOrder)
                .ThenBy(p => p.DistanceKm)
                .ThenBy(p => p.SortOrder)
                .ToListAsync();

            return (categories, places);
        }

        public async Task<bool> ReplaceNearbyPlacesAsync(int listingTypeId, int listingId, List<(int NearbyCategoryId, string PlaceName, decimal DistanceKm)> places)
        {
            var existing = await _context.ListingNearbyPlaces
                .Where(p => p.ListingTypeId == listingTypeId && p.ListingId == listingId)
                .ToListAsync();
            _context.ListingNearbyPlaces.RemoveRange(existing);

            _context.ListingNearbyPlaces.AddRange(places.Select((p, i) => new ListingNearbyPlace
            {
                ListingTypeId = listingTypeId,
                ListingId = listingId,
                NearbyCategoryId = p.NearbyCategoryId,
                PlaceName = p.PlaceName,
                DistanceKm = Math.Round(p.DistanceKm, 2),
                Stars = null,
                SortOrder = i,
                IsActive = true
            }));

            await _context.SaveChangesAsync();
            return true;
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
