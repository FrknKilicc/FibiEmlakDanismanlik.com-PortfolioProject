using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Requests.PropertyRequests;
using FibiEmlakDanismanlik.Application.Features.Results.AmenityFacetResults;
using FibiEmlakDanismanlik.Application.Features.Results.CommonPropertyResults;
using FibiEmlakDanismanlik.Application.Features.Results.ForRentalPropertyResults;
using FibiEmlakDanismanlik.Application.Features.Results.ForSalePropertyResults;
using FibiEmlakDanismanlik.Application.Features.Results.ListingTypeResults;
using FibiEmlakDanismanlik.Application.Helpers;
using FibiEmlakDanismanlik.Application.Interfaces.PropertyInterfaces;
using FibiEmlakDanismanlik.Application.ViewModels;
using FibiEmlakDanismanlik.Domain.DTOs;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;
using FibiEmlakDanismanlik.Persistence.Context;
using FibiEmlakDanismanlik.Persistence.Repositories.ListingImageRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Persistence.Repositories.PropertyRepositories
{
    public partial class PropertyRepository
    {
        public List<RentalPropertyBaseViewModel> GetAllRentalPropertyWithAgent()
        {
            var rentalCommercialProperties = (from rentalCommercial in _context.rentalCommercialPropertyListings
                                              join agent in _context.Agents on rentalCommercial.AgentId equals agent.AgentId
                                              select new RentalCommercialPropertyViewModel
                                              {
                                                  PropertyType = "İş Yeri",
                                                  PropertyName = rentalCommercial.PropertyName,
                                                  PropertyStatus = rentalCommercial.PropertyStatus,
                                                  PropertyDescription = rentalCommercial.PropertyDescription,
                                                  PropertyNo = rentalCommercial.PropertyNo,
                                                  Rent = rentalCommercial.Rent,
                                                  AddressDesc = rentalCommercial.AddressDesc,
                                                  AgentId = rentalCommercial.AgentId,
                                                  AgentName = agent.AgentName,
                                                  AgentImgUrl = agent.AgentImgUrl,
                                                  City = rentalCommercial.City,
                                                  CreatedDate = rentalCommercial.CreatedDate,
                                                  District = rentalCommercial.District,
                                                  Neighborhood = rentalCommercial.Neighborhood,
                                                  PropImgUrl1 = _context.ListingImages
                                                      .Where(i => i.ListingTypeId == 9 && i.ListingId == rentalCommercial.RentalCommercialListId && i.Kind == ListingImageKind.Photo)
                                                      .OrderByDescending(i => i.IsCover).ThenBy(i => i.SortOrder).ThenBy(i => i.ListingImageId)
                                                      .Select(i => i.Url)
                                                      .FirstOrDefault() ?? "",
                                                  Facade = rentalCommercial.Facade,
                                                  BestDeals = rentalCommercial.BestDeals,
                                                  BuildingAge = rentalCommercial.BuildingAge,
                                                  Deposit = rentalCommercial.Deposit,
                                                  GrossArea = rentalCommercial.GrossArea,
                                                  Heating = rentalCommercial.Heating,
                                                  NetArea = rentalCommercial.NetArea,
                                                  NumberOfBathrooms = rentalCommercial.NumberOfBathrooms,
                                                  NumberOfFloors = rentalCommercial.NumberOfFloors,
                                                  NumberOfKitchens = rentalCommercial.NumberOfKitchens,
                                                  NumberOfSection = rentalCommercial.NumberOfSection,
                                                  TitleDeedStatus = rentalCommercial.TitleDeedStatus
                                              }).ToList();

            var rentalLandProperties = (from rentalLand in _context.rentalLandListings
                                        join agent in _context.Agents on rentalLand.AgentId equals agent.AgentId
                                        select new RentalLandPropertyViewModel
                                        {
                                            PropertyType = "Arsa",
                                            PropertyNo = rentalLand.PropertyNo,
                                            PropertyName = rentalLand.PropertyName,
                                            PropertyDescription = rentalLand.PropertyDescription,
                                            PropertyStatus = rentalLand.PropertyStatus,
                                            CreatedDate = rentalLand.CreatedDate,
                                            Rent = rentalLand.Rent,
                                            City = rentalLand.City,
                                            District = rentalLand.District,
                                            Neighborhood = rentalLand.Neighborhood,
                                            AddressDesc = rentalLand.AddressDesc,
                                            AgentId = agent.AgentId,
                                            AgentName = agent.AgentName,
                                            AgentImgUrl = agent.AgentImgUrl,
                                            PropImgUrl1 = _context.ListingImages
                                                .Where(i => i.ListingTypeId == 7 && i.ListingId == rentalLand.RentalLandListingId && i.Kind == ListingImageKind.Photo)
                                                .OrderByDescending(i => i.IsCover).ThenBy(i => i.SortOrder).ThenBy(i => i.ListingImageId)
                                                .Select(i => i.Url)
                                                .FirstOrDefault() ?? "",
                                            ZoningStatus = rentalLand.ZoningStatus,
                                            Area = rentalLand.Area,
                                            PricePerSquareMeter = rentalLand.PricePerSquareMeter,
                                            ParcelNumber = rentalLand.ParcelNumber,
                                            PlotNumber = rentalLand.PlotNumber,
                                            MapSheetNumber = rentalLand.MapSheetNumber,
                                            FloorAreaRatio = rentalLand.FloorAreaRatio,
                                            BaseAreaRatio = rentalLand.BaseAreaRatio,
                                            ZoningPlan = rentalLand.ZoningPlan,
                                            TitleDeedStatus = rentalLand.TitleDeedStatus,
                                            DevelopmentRight = rentalLand.DevelopmentRight,
                                            BestDeals = rentalLand.BestDeals
                                        }).ToList();

            var rentalHousingProperties = (from rentalHousing in _context.rentalHousingListings
                                           join agent in _context.Agents on rentalHousing.AgentId equals agent.AgentId
                                           select new RentalHousingPropertyViewModel
                                           {
                                               PropertyType = "Konut",
                                               PropertyNo = rentalHousing.PropertyNo,
                                               PropertyName = rentalHousing.PropertyName,
                                               PropertyDescription = rentalHousing.PropertyDescription,
                                               PropertyStatus = rentalHousing.PropertyStatus,
                                               CreatedDate = rentalHousing.CreatedDate,
                                               Rent = rentalHousing.Rent,
                                               City = rentalHousing.City,
                                               District = rentalHousing.District,
                                               Neighborhood = rentalHousing.Neighborhood,
                                               AddressDesc = rentalHousing.AddressDesc,
                                               AgentId = agent.AgentId,
                                               AgentName = agent.AgentName,
                                               AgentImgUrl = agent.AgentImgUrl,
                                               PropImgUrl1 = _context.ListingImages
                                                   .Where(i => i.ListingTypeId == 8 && i.ListingId == rentalHousing.RentalHousingListId && i.Kind == ListingImageKind.Photo)
                                                   .OrderByDescending(i => i.IsCover).ThenBy(i => i.SortOrder).ThenBy(i => i.ListingImageId)
                                                   .Select(i => i.Url)
                                                   .FirstOrDefault() ?? "",
                                               IsElevator = rentalHousing.IsElevator,
                                               GrossArea = rentalHousing.GrossArea,
                                               NetArea = rentalHousing.NetArea,
                                               OpenArea = rentalHousing.OpenArea,
                                               BuildingAge = rentalHousing.BuildingAge,
                                               TotalNumberOfFloor = rentalHousing.TotalNumberOfFloor,
                                               NumberOfFloors = rentalHousing.NumberOfFloors,
                                               NumberOfBathRoom = rentalHousing.NumberOfBathRoom,
                                               NumberOfRoom = rentalHousing.NumberOfRoom,
                                               NumberOfBalconies = rentalHousing.NumberOfBalconies,
                                               Heating = rentalHousing.Heating,
                                               BlackBox = rentalHousing.BlackBox,
                                               ParkingLot = rentalHousing.ParkingLot,
                                               Furnished = rentalHousing.Furnished,
                                               WithinTheComplex = rentalHousing.WithinTheComplex,
                                               Dues = rentalHousing.Dues,
                                               Deposit = rentalHousing.Deposit,
                                               BestDeals = rentalHousing.BestDeals
                                           }).ToList();


            var getAllRentalProperties = rentalCommercialProperties.Cast<RentalPropertyBaseViewModel>()
                .Union(rentalLandProperties.Cast<RentalPropertyBaseViewModel>())
                .Union(rentalHousingProperties.Cast<RentalPropertyBaseViewModel>())
                .OrderByDescending(x => x.CreatedDate)
                .Take(10)
                .ToList();

            return getAllRentalProperties;
        }

        private IQueryable<ForRentalPropertForListingViewModel> RentalHousingRows()
        {
            return
                from house in _context.rentalHousingListings
                join agent in _context.Agents on house.AgentId equals agent.AgentId
                select new ForRentalPropertForListingViewModel
                {
                    ListingId = house.RentalHousingListId,
                    RentalHousingListId = house.RentalHousingListId,
                    RentalLandListingId = null,
                    RentalCommercialListId = null,

                    PropertyNo = house.PropertyNo,
                    PropertyName = house.PropertyName,
                    PropertyDescription = house.PropertyDescription,
                    PropertyStatus = house.PropertyStatus,
                    CreatedDate = house.CreatedDate,
                    Rent = house.Rent,
                    Deposit = house.Deposit,
                    Dues = house.Dues,
                    BestDeals = house.BestDeals,

                    PropertyType = "Kiralık",
                    ListingType = "Konut",
                    ListingTypeId = house.ListingTypeId,
                    ListingTypeName = house.ListingType.Name,
                    UsageTypeId = (int)house.ListingType.UsageType,

                    City = house.City,
                    District = house.District,
                    Neighborhood = house.Neighborhood,
                    AddressDesc = house.AddressDesc,

                    HousingCategoryId = house.HousingCategory.HousingCategoryId,
                    HousingCategoryName = house.HousingCategory.HousingCategoryName,
                    HousingCategory = null,
                    LandCategoryId = null,
                    LandCategoryName = null!,
                    LandCategory = null,

                    Facade = house.Facade,
                    IsElevator = house.IsElevator,
                    GrossArea = house.GrossArea,
                    NetArea = house.NetArea,
                    OpenArea = house.OpenArea,
                    BuildingAge = house.BuildingAge,
                    TotalNumberOfFloor = house.TotalNumberOfFloor,
                    NumberOfFloors = house.NumberOfFloors,
                    NumberOfBathRoom = house.NumberOfBathRoom,
                    NumberOfRoom = house.NumberOfRoom,
                    Heating = house.Heating,
                    BlackBox = house.BlackBox,
                    NumberOfBalconies = house.NumberOfBalconies,
                    ParkingLot = house.ParkingLot,
                    Furnished = house.Furnished,
                    WithinTheComplex = house.WithinTheComplex,

                    Area = null,
                    PricePerSquareMeter = null,
                    ParcelNumber = null,
                    PlotNumber = null,
                    MapSheetNumber = null,
                    FloorAreaRatio = null,
                    BaseAreaRatio = null,
                    ZoningPlan = null,
                    ZoningStatus = null,
                    DevelopmentRight = null,
                    TitleDeedStatus = null,

                    NumberOfSection = null,
                    NumberOfKitchens = null,
                    NumberOfBathrooms = null,

                    AgentId = house.AgentId,
                    AgentName = agent.AgentName,
                    AgentTitle = agent.AgentTitle,
                    AgentImgUrl = agent.AgentImgUrl,
                    AgentDescription = agent.AgentDescription,
                    AgentPhoneNumber = agent.AgentPhoneNumber,
                    Mail = agent.Mail
                };
        }

        private IQueryable<ForRentalPropertForListingViewModel> RentalLandRows()
        {
            return
                from land in _context.rentalLandListings
                join agent in _context.Agents on land.AgentId equals agent.AgentId
                select new ForRentalPropertForListingViewModel
                {
                    ListingId = land.RentalLandListingId,
                    RentalHousingListId = null,
                    RentalLandListingId = land.RentalLandListingId,
                    RentalCommercialListId = null,

                    PropertyNo = land.PropertyNo,
                    PropertyName = land.PropertyName,
                    PropertyDescription = land.PropertyDescription,
                    PropertyStatus = land.PropertyStatus,
                    CreatedDate = land.CreatedDate,
                    Rent = land.Rent,
                    Deposit = null,
                    Dues = null,
                    BestDeals = land.BestDeals,

                    PropertyType = "Kiralık",
                    ListingType = "Arsa",
                    ListingTypeId = land.ListingTypeId,
                    ListingTypeName = land.ListingType.Name,
                    UsageTypeId = (int)land.ListingType.UsageType,

                    City = land.City,
                    District = land.District,
                    Neighborhood = land.Neighborhood,
                    AddressDesc = land.AddressDesc,

                    TitleDeedStatus = land.TitleDeedStatus,
                    LandCategoryId = land.LandCategory.LandCategoryId,
                    LandCategoryName = land.LandCategory.LandCategoryName,
                    LandCategory = null,
                    HousingCategoryId = null,
                    HousingCategoryName = null!,
                    HousingCategory = null,

                    Area = land.Area,
                    PricePerSquareMeter = land.PricePerSquareMeter,
                    ParcelNumber = land.ParcelNumber,
                    PlotNumber = land.PlotNumber,
                    MapSheetNumber = land.MapSheetNumber,
                    FloorAreaRatio = land.FloorAreaRatio,
                    BaseAreaRatio = land.BaseAreaRatio,
                    ZoningPlan = land.ZoningPlan,
                    ZoningStatus = land.ZoningStatus,
                    DevelopmentRight = land.DevelopmentRight,

                    BuildingAge = null,
                    GrossArea = null,
                    NetArea = null,
                    OpenArea = null,
                    Heating = null,
                    NumberOfFloors = null,
                    TotalNumberOfFloor = null,
                    NumberOfBathRoom = null,
                    NumberOfRoom = null,
                    NumberOfBalconies = null,
                    Facade = null,
                    IsElevator = null,
                    BlackBox = null,
                    ParkingLot = null,
                    Furnished = null,
                    WithinTheComplex = null,

                    NumberOfSection = null,
                    NumberOfKitchens = null,
                    NumberOfBathrooms = null,

                    AgentId = land.AgentId,
                    AgentName = agent.AgentName,
                    AgentTitle = agent.AgentTitle,
                    AgentImgUrl = agent.AgentImgUrl,
                    AgentDescription = agent.AgentDescription,
                    AgentPhoneNumber = agent.AgentPhoneNumber,
                    Mail = agent.Mail
                };
        }

        private IQueryable<ForRentalPropertForListingViewModel> RentalCommercialRows()
        {
            return
                from comm in _context.rentalCommercialPropertyListings
                join agent in _context.Agents on comm.AgentId equals agent.AgentId
                select new ForRentalPropertForListingViewModel
                {
                    ListingId = comm.RentalCommercialListId,
                    RentalHousingListId = null,
                    RentalLandListingId = null,
                    RentalCommercialListId = comm.RentalCommercialListId,

                    PropertyNo = comm.PropertyNo,
                    PropertyName = comm.PropertyName,
                    PropertyDescription = comm.PropertyDescription,
                    PropertyStatus = comm.PropertyStatus,
                    CreatedDate = comm.CreatedDate,
                    Rent = comm.Rent,
                    Deposit = comm.Deposit,
                    Dues = null,
                    BestDeals = comm.BestDeals,

                    PropertyType = "Kiralık",
                    ListingType = "İşyeri",
                    ListingTypeId = comm.ListingTypeId,
                    ListingTypeName = comm.ListingType.Name,
                    UsageTypeId = (int)comm.ListingType.UsageType,

                    City = comm.City,
                    District = comm.District,
                    Neighborhood = comm.Neighborhood,
                    AddressDesc = comm.AddressDesc,

                    TitleDeedStatus = comm.TitleDeedStatus,
                    HousingCategoryId = null,
                    HousingCategoryName = null!,
                    HousingCategory = null,
                    LandCategoryId = null,
                    LandCategoryName = null!,
                    LandCategory = null,

                    Facade = comm.Facade,
                    NumberOfSection = comm.NumberOfSection,
                    NumberOfKitchens = comm.NumberOfKitchens,
                    NumberOfBathrooms = comm.NumberOfBathrooms,
                    NumberOfFloors = comm.NumberOfFloors,
                    GrossArea = comm.GrossArea,
                    NetArea = comm.NetArea,
                    BuildingAge = comm.BuildingAge,
                    Heating = comm.Heating,

                    OpenArea = null,
                    TotalNumberOfFloor = null,
                    NumberOfBathRoom = null,
                    NumberOfRoom = null,
                    NumberOfBalconies = null,
                    IsElevator = null,
                    BlackBox = null,
                    ParkingLot = null,
                    Furnished = null,
                    WithinTheComplex = null,

                    Area = null,
                    PricePerSquareMeter = null,
                    ParcelNumber = null,
                    PlotNumber = null,
                    MapSheetNumber = null,
                    FloorAreaRatio = null,
                    BaseAreaRatio = null,
                    ZoningPlan = null,
                    ZoningStatus = null,
                    DevelopmentRight = null,

                    AgentId = comm.AgentId,
                    AgentName = agent.AgentName,
                    AgentTitle = agent.AgentTitle,
                    AgentImgUrl = agent.AgentImgUrl,
                    AgentDescription = agent.AgentDescription,
                    AgentPhoneNumber = agent.AgentPhoneNumber,
                    Mail = agent.Mail
                };
        }

        public List<ForRentalPropertForListingViewModel> GetAllForRentalPropertyForListing()
        {
            var listing = RentalHousingRows().Concat(RentalLandRows()).Concat(RentalCommercialRows()).OrderByDescending(x => x.CreatedDate).ToList();

            FillPhotoSlots(listing, x =>
            {
                if (x.RentalHousingListId.HasValue) return (8, x.RentalHousingListId.Value);
                if (x.RentalLandListingId.HasValue) return (7, x.RentalLandListingId.Value);
                if (x.RentalCommercialListId.HasValue) return (9, x.RentalCommercialListId.Value);
                return (0, 0);
            });

            return listing;
        }

        public async Task<List<ListingTypeFacetDto>> GetForRentalListingTypeFacetsAsync()
        {
            var activeStatuses = PropertyStatuses.ActiveSet;

            var listingTypes = await _context.listingTypes
                .AsNoTracking()
                .Where(x => x.UsageType == UsageType.ForRent)
                .Select(x => new { x.ListingTypeId, x.Name })
                .ToListAsync();

            var housingTypeIds = _context.rentalHousingListings.AsNoTracking()
.Where(x => x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus)).Select(x => x.ListingTypeId);

            var landTypeIds = _context.rentalLandListings.AsNoTracking()
.Where(x => x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus)).Select(x => x.ListingTypeId);

            var commTypeIds = _context.rentalCommercialPropertyListings.AsNoTracking()
.Where(x => x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus)).Select(x => x.ListingTypeId);

            var groupedCounts = await housingTypeIds.Concat(landTypeIds).Concat(commTypeIds)
                .Where(id => id != null)
                .GroupBy(id => id!)
                .Select(g => new { ListingTypeId = g.Key, Count = g.Count() })
                .ToListAsync();

            return listingTypes
                .Select(lt => new ListingTypeFacetDto
                {
                    ListingTypeId = lt.ListingTypeId,
                    Name = lt.Name,
                    Count = groupedCounts.FirstOrDefault(x => x.ListingTypeId == lt.ListingTypeId)?.Count ?? 0
                })
                .OrderBy(x => x.ListingTypeId)
                .ToList();
        }

        private IQueryable<RentalHousingListingResult> BuildRentalHousingQuery(PropertyFilterRequest filter)
        {
            var activeStatuses = PropertyStatuses.ActiveSet;

            var query =
                from h in _context.rentalHousingListings
                join a in _context.Agents on h.AgentId equals a.AgentId
                join lt in _context.listingTypes on h.ListingTypeId equals lt.ListingTypeId
                where lt.UsageType == UsageType.ForRent
                      && h.PropertyStatus != null
                      && activeStatuses.Contains(h.PropertyStatus)
                select new RentalHousingListingResult
                {
                    ItemType = "housing",
                    ListingId = h.RentalHousingListId,
                    ListingTypeId = h.ListingTypeId,
                    UsageTypeId = (int)lt.UsageType,
                    ListingType = h.ListingType.Name,

                    PropertyNo = h.PropertyNo,
                    PropertyName = h.PropertyName,
                    PropertyDescription = h.PropertyDescription,
                    PropertyStatus = h.PropertyStatus,
                    CreatedDate = h.CreatedDate,

                    Price = h.Rent,

                    Deposit = h.Deposit,

                    BestDeals = h.BestDeals,

                    City = h.City,
                    District = h.District,
                    CityId = h.CityId,
                    DistrictId = h.DistrictId,
                    NeighborhoodId = h.NeighborhoodId,
                    Neighborhood = h.Neighborhood,
                    AddressDesc = h.AddressDesc,

                    AgentId = h.AgentId,
                    AgentName = a.AgentName,
                    AgentTitle = a.AgentTitle,
                    AgentImgUrl = a.AgentImgUrl,

                    // Konut özel alanlar
                    HousingCategoryId = h.HousingCategoryId,
                    Facade = h.Facade,
                    IsElevator = h.IsElevator,
                    GrossArea = h.GrossArea,
                    NetArea = h.NetArea,
                    OpenArea = h.OpenArea,
                    BuildingAge = h.BuildingAge,
                    TotalNumberOfFloor = h.TotalNumberOfFloor,
                    NumberOfFloors = h.NumberOfFloors,
                    NumberOfBathRoom = h.NumberOfBathRoom,
                    NumberOfRoom = h.NumberOfRoom,
                    Heating = h.Heating,
                    BlackBox = h.BlackBox,
                    NumberOfBalconies = h.NumberOfBalconies,
                    ParkingLot = h.ParkingLot,
                    Furnished = h.Furnished,
                    WithinTheComplex = h.WithinTheComplex,
                    Dues = h.Dues
                };

            query = ApplyCommonRentalFilters(query, filter);

            if (!string.IsNullOrWhiteSpace(filter.NumberOfRoom))
                query = query.Where(x => x.NumberOfRoom == filter.NumberOfRoom);

            if (filter.MinArea.HasValue)
                query = query.Where(x => x.GrossArea >= filter.MinArea.Value);

            if (filter.MaxArea.HasValue)
                query = query.Where(x => x.GrossArea <= filter.MaxArea.Value);

            if (filter.NumberOfBathRoom.HasValue)
                query = query.Where(x => x.NumberOfBathRoom == filter.NumberOfBathRoom.Value);

            if (filter.Floor.HasValue)
                query = query.Where(x => x.NumberOfFloors == filter.Floor.Value);

            if (filter.NumberOfSection.HasValue)
                query = query.Where(x => false);

            if (!string.IsNullOrWhiteSpace(filter.ZoningStatus))
                query = query.Where(x => false);

            if (filter.HousingCategoryId.HasValue)
                query = query.Where(x => x.HousingCategoryId == filter.HousingCategoryId.Value);
            if (!string.IsNullOrWhiteSpace(filter.BuildingAge))
                query = query.Where(x => x.BuildingAge == filter.BuildingAge);
            if (!string.IsNullOrWhiteSpace(filter.Heating))
                query = query.Where(x => x.Heating == filter.Heating);
            if (filter.Furnished.HasValue)
                query = query.Where(x => (x.Furnished ?? false) == filter.Furnished.Value);
            if (filter.WithinTheComplex.HasValue)
                query = query.Where(x => (x.WithinTheComplex ?? false) == filter.WithinTheComplex.Value);

            if (filter.LandCategoryId.HasValue || !string.IsNullOrWhiteSpace(filter.TitleDeedStatus) || filter.LoanEligible.HasValue || filter.Transferable.HasValue)
                query = query.Where(x => false);

            return query;
        }
        private IQueryable<RentalLandListingResult> BuildRentalLandQuery(PropertyFilterRequest filter)
        {
            var activeStatuses = PropertyStatuses.ActiveSet;

            var query =
                from l in _context.rentalLandListings
                join a in _context.Agents on l.AgentId equals a.AgentId
                join lt in _context.listingTypes on l.ListingTypeId equals lt.ListingTypeId
                where lt.UsageType == UsageType.ForRent
                      && l.PropertyStatus != null
                      && activeStatuses.Contains(l.PropertyStatus)
                select new RentalLandListingResult
                {
                    ItemType = "land",
                    ListingId = l.RentalLandListingId,
                    ListingTypeId = l.ListingTypeId,
                    UsageTypeId = (int)lt.UsageType,
                    ListingType = l.ListingType.Name,

                    PropertyNo = l.PropertyNo,
                    PropertyName = l.PropertyName,
                    PropertyDescription = l.PropertyDescription,
                    PropertyStatus = l.PropertyStatus,
                    CreatedDate = l.CreatedDate,

                    Price = l.Rent,
                    Deposit = 0,
                    BestDeals = l.BestDeals,

                    City = l.City,
                    District = l.District,
                    CityId = l.CityId,
                    DistrictId = l.DistrictId,
                    NeighborhoodId = l.NeighborhoodId,
                    Neighborhood = l.Neighborhood,
                    AddressDesc = l.AddressDesc,

                    AgentId = l.AgentId,
                    AgentName = a.AgentName,
                    AgentTitle = a.AgentTitle,
                    AgentImgUrl = a.AgentImgUrl,

                    // Arsa özel
                    LandCategoryId = l.LandCategoryId,
                    ZoningStatus = l.ZoningStatus,
                    TitleDeedStatus = l.TitleDeedStatus,
                    Area = l.Area,
                    PricePerSquareMeter = l.PricePerSquareMeter,
                    ParcelNumber = l.ParcelNumber,
                    PlotNumber = l.PlotNumber,
                    MapSheetNumber = l.MapSheetNumber,
                    FloorAreaRatio = l.FloorAreaRatio,
                    BaseAreaRatio = l.BaseAreaRatio,
                    ZoningPlan = l.ZoningPlan,
                    DevelopmentRight = l.DevelopmentRight
                };

            query = ApplyCommonRentalFilters(query, filter);

            if (filter.MinArea.HasValue)
                query = query.Where(x => x.Area >= filter.MinArea.Value);

            if (filter.MaxArea.HasValue)
                query = query.Where(x => x.Area <= filter.MaxArea.Value);

            if (filter.NumberOfBathRoom.HasValue)
                query = query.Where(x => false);

            if (filter.Floor.HasValue)
                query = query.Where(x => false);

            if (filter.NumberOfSection.HasValue)
                query = query.Where(x => false);

            if (!string.IsNullOrWhiteSpace(filter.ZoningStatus))
                query = query.Where(x => x.ZoningStatus == filter.ZoningStatus);

            if (filter.LandCategoryId.HasValue)
                query = query.Where(x => x.LandCategoryId == filter.LandCategoryId.Value);
            if (!string.IsNullOrWhiteSpace(filter.TitleDeedStatus))
                query = query.Where(x => x.TitleDeedStatus == filter.TitleDeedStatus);

            if (filter.HousingCategoryId.HasValue || !string.IsNullOrWhiteSpace(filter.BuildingAge) || !string.IsNullOrWhiteSpace(filter.Heating) || filter.Furnished.HasValue || filter.WithinTheComplex.HasValue || filter.LoanEligible.HasValue || filter.Transferable.HasValue)
                query = query.Where(x => false);

            return query;
        }
        private IQueryable<RentalCommercialListingResult> BuildRentalCommercialQuery(PropertyFilterRequest filter)
        {
            var activeStatuses = PropertyStatuses.ActiveSet;

            var query =
                from c in _context.rentalCommercialPropertyListings
                join a in _context.Agents on c.AgentId equals a.AgentId
                join lt in _context.listingTypes on c.ListingTypeId equals lt.ListingTypeId
                where lt.UsageType == UsageType.ForRent
                      && c.PropertyStatus != null
                      && activeStatuses.Contains(c.PropertyStatus)
                select new RentalCommercialListingResult
                {
                    ItemType = "commercial",
                    ListingId = c.RentalCommercialListId,
                    ListingTypeId = c.ListingTypeId,
                    UsageTypeId = (int)lt.UsageType,
                    ListingType = c.ListingType.Name,

                    PropertyNo = c.PropertyNo,
                    PropertyName = c.PropertyName,
                    PropertyDescription = c.PropertyDescription,
                    PropertyStatus = c.PropertyStatus,
                    CreatedDate = c.CreatedDate,

                    Price = c.Rent,

                    Deposit = c.Deposit,

                    BestDeals = c.BestDeals,

                    City = c.City,
                    District = c.District,
                    CityId = c.CityId,
                    DistrictId = c.DistrictId,
                    NeighborhoodId = c.NeighborhoodId,
                    Neighborhood = c.Neighborhood,
                    AddressDesc = c.AddressDesc,

                    AgentId = c.AgentId,
                    AgentName = a.AgentName,
                    AgentTitle = a.AgentTitle,
                    AgentImgUrl = a.AgentImgUrl,

                    // İşyeri özel
                    Facade = c.Facade,
                    NumberOfSection = c.NumberOfSection,
                    NumberOfKitchens = c.NumberOfKitchens,
                    NumberOfBathrooms = c.NumberOfBathrooms,
                    NumberOfFloors = c.NumberOfFloors,
                    GrossArea = c.GrossArea,
                    NetArea = c.NetArea,
                    BuildingAge = c.BuildingAge,
                    Heating = c.Heating
                };

            query = ApplyCommonRentalFilters(query, filter);

            if (filter.MinArea.HasValue)
                query = query.Where(x => x.GrossArea >= filter.MinArea.Value);

            if (filter.MaxArea.HasValue)
                query = query.Where(x => x.GrossArea <= filter.MaxArea.Value);

            if (filter.NumberOfBathRoom.HasValue)
                query = query.Where(x => x.NumberOfBathrooms == filter.NumberOfBathRoom.Value);

            if (filter.Floor.HasValue)
                query = query.Where(x => x.NumberOfFloors == filter.Floor.Value);

            if (filter.NumberOfSection.HasValue)
                query = query.Where(x => x.NumberOfSection == filter.NumberOfSection.Value);

            if (!string.IsNullOrWhiteSpace(filter.ZoningStatus))
                query = query.Where(x => false);

            if (!string.IsNullOrWhiteSpace(filter.BuildingAge))
                query = query.Where(x => x.BuildingAge == filter.BuildingAge);
            if (!string.IsNullOrWhiteSpace(filter.Heating))
                query = query.Where(x => x.Heating == filter.Heating);

            if (filter.HousingCategoryId.HasValue || filter.LandCategoryId.HasValue || !string.IsNullOrWhiteSpace(filter.TitleDeedStatus) || filter.Furnished.HasValue || filter.WithinTheComplex.HasValue || filter.LoanEligible.HasValue || filter.Transferable.HasValue)
                query = query.Where(x => false);

            return query;
        }
        private sealed class RentalListingSortKey
        {
            public int SourceType { get; set; }
            public int ListingId { get; set; }
            public decimal? Price { get; set; }
            public DateTime? CreatedDate { get; set; }
        }

        public async Task<RentalListingFilterResponseResult> GetFilteredForRentalPropertyForListingWithFacets(PropertyFilterRequest filter)
        {
            var selectedIds = (filter.SelectedAmenityIds ?? new List<int>()).Distinct().ToArray();
            var selectedCount = selectedIds.Length;
            var selectedSet = selectedIds.ToHashSet();

            var baseHousingQuery = BuildRentalHousingQuery(filter);
            var baseLandQuery = BuildRentalLandQuery(filter);
            var baseCommercialQuery = BuildRentalCommercialQuery(filter);

            if (selectedCount > 0)
            {
                var housingMatchedIds =
                    _context.RentalHousingListingAmenities
                        .Where(x => selectedIds.Contains(x.AmenityId))
                        .GroupBy(x => x.RentalHousingListId)
                        .Where(g => g.Select(x => x.AmenityId).Distinct().Count() == selectedCount)
                        .Select(g => g.Key);

                var landMatchedIds =
                    _context.RentalLandListingAmenities
                        .Where(x => selectedIds.Contains(x.AmenityId))
                        .GroupBy(x => x.RentalLandListingId)
                        .Where(g => g.Select(x => x.AmenityId).Distinct().Count() == selectedCount)
                        .Select(g => g.Key);

                var commercialMatchedIds =
                    _context.RentalCommercialListingAmenities
                        .Where(x => selectedIds.Contains(x.AmenityId))
                        .GroupBy(x => x.RentalCommercialListId)
                        .Where(g => g.Select(x => x.AmenityId).Distinct().Count() == selectedCount)
                        .Select(g => g.Key);

                baseHousingQuery = baseHousingQuery.Where(x => housingMatchedIds.Contains(x.ListingId));
                baseLandQuery = baseLandQuery.Where(x => landMatchedIds.Contains(x.ListingId));
                baseCommercialQuery = baseCommercialQuery.Where(x => commercialMatchedIds.Contains(x.ListingId));
            }

            var baseHousingIds = baseHousingQuery.Select(x => x.ListingId);
            var baseLandIds = baseLandQuery.Select(x => x.ListingId);
            var baseCommercialIds = baseCommercialQuery.Select(x => x.ListingId);

            var housingCounts = await _context.RentalHousingListingAmenities
                .Where(x => baseHousingIds.Contains(x.RentalHousingListId))
                .GroupBy(x => x.AmenityId)
                .Select(g => new { AmenityId = g.Key, Cnt = g.Select(x => x.RentalHousingListId).Distinct().Count() })
                .ToListAsync();

            var landCounts = await _context.RentalLandListingAmenities
                .Where(x => baseLandIds.Contains(x.RentalLandListingId))
                .GroupBy(x => x.AmenityId)
                .Select(g => new { AmenityId = g.Key, Cnt = g.Select(x => x.RentalLandListingId).Distinct().Count() })
                .ToListAsync();

            var commercialCounts = await _context.RentalCommercialListingAmenities
                .Where(x => baseCommercialIds.Contains(x.RentalCommercialListId))
                .GroupBy(x => x.AmenityId)
                .Select(g => new { AmenityId = g.Key, Cnt = g.Select(x => x.RentalCommercialListId).Distinct().Count() })
                .ToListAsync();

            var countMap = new Dictionary<int, int>();

            foreach (var x in housingCounts) countMap[x.AmenityId] = countMap.GetValueOrDefault(x.AmenityId) + x.Cnt;
            foreach (var x in landCounts) countMap[x.AmenityId] = countMap.GetValueOrDefault(x.AmenityId) + x.Cnt;
            foreach (var x in commercialCounts) countMap[x.AmenityId] = countMap.GetValueOrDefault(x.AmenityId) + x.Cnt;

            var total = await baseHousingQuery.CountAsync()
                + await baseLandQuery.CountAsync()
                + await baseCommercialQuery.CountAsync();

            var housingKeys = baseHousingQuery.Select(x => new RentalListingSortKey
            {
                SourceType = 1,
                ListingId = x.ListingId,
                Price = x.Price,
                CreatedDate = x.CreatedDate
            });

            var landKeys = baseLandQuery.Select(x => new RentalListingSortKey
            {
                SourceType = 2,
                ListingId = x.ListingId,
                Price = x.Price,
                CreatedDate = x.CreatedDate
            });

            var commercialKeys = baseCommercialQuery.Select(x => new RentalListingSortKey
            {
                SourceType = 3,
                ListingId = x.ListingId,
                Price = x.Price,
                CreatedDate = x.CreatedDate
            });

            var combinedKeys = housingKeys.Concat(landKeys).Concat(commercialKeys);

            var sortBy = (filter.SortBy ?? "CreatedDate").ToLowerInvariant();
            var sortDir = (filter.SortDir ?? "desc").ToLowerInvariant();

            IOrderedQueryable<RentalListingSortKey> sortedKeys = (sortBy, sortDir) switch
            {
                ("price", "asc") => combinedKeys.OrderBy(x => x.Price),
                ("price", "desc") => combinedKeys.OrderByDescending(x => x.Price),
                ("createddate", "asc") => combinedKeys.OrderBy(x => x.CreatedDate),
                _ => combinedKeys.OrderByDescending(x => x.CreatedDate),
            };

            var page = filter.Page < 1 ? 1 : filter.Page;
            var pageSize = filter.PageSize is < 1 or > 100 ? 20 : filter.PageSize;

            var pageKeys = await sortedKeys
                .ThenBy(x => x.SourceType)
                .ThenBy(x => x.ListingId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var housingIds = pageKeys.Where(x => x.SourceType == 1).Select(x => x.ListingId).ToList();
            var landIds = pageKeys.Where(x => x.SourceType == 2).Select(x => x.ListingId).ToList();
            var commercialIds = pageKeys.Where(x => x.SourceType == 3).Select(x => x.ListingId).ToList();

            var pageHousing = housingIds.Count > 0
                ? await baseHousingQuery.Where(x => housingIds.Contains(x.ListingId)).AsNoTracking().ToListAsync()
                : new List<RentalHousingListingResult>();

            var pageLand = landIds.Count > 0
                ? await baseLandQuery.Where(x => landIds.Contains(x.ListingId)).AsNoTracking().ToListAsync()
                : new List<RentalLandListingResult>();

            var pageCommercial = commercialIds.Count > 0
                ? await baseCommercialQuery.Where(x => commercialIds.Contains(x.ListingId)).AsNoTracking().ToListAsync()
                : new List<RentalCommercialListingResult>();

            var photoKeys = pageHousing.Select(x => (ListingTypeId: 8, x.ListingId))
                .Concat(pageLand.Select(x => (ListingTypeId: 7, x.ListingId)))
                .Concat(pageCommercial.Select(x => (ListingTypeId: 9, x.ListingId)));

            var photoMap = await ListingPhotoLookup.LoadAsync(_context, photoKeys);

            foreach (var h in pageHousing)
            {
                h.ImageUrls = photoMap.TryGetValue((8, h.ListingId), out var urls)
                    ? urls
                    : new List<string>();
            }

            foreach (var l in pageLand)
            {
                l.ImageUrls = photoMap.TryGetValue((7, l.ListingId), out var urls) && urls.Count > 0
                    ? new List<string> { urls[0] }
                    : new List<string>();
            }

            foreach (var c in pageCommercial)
            {
                c.ImageUrls = photoMap.TryGetValue((9, c.ListingId), out var urls) && urls.Count > 0
                    ? new List<string> { urls[0] }
                    : new List<string>();
            }

            var housingDict = pageHousing.ToDictionary(x => x.ListingId);
            var landDict = pageLand.ToDictionary(x => x.ListingId);
            var commercialDict = pageCommercial.ToDictionary(x => x.ListingId);

            var items = new List<RentalListingBaseResult>(pageKeys.Count);
            foreach (var key in pageKeys)
            {
                RentalListingBaseResult? item = key.SourceType switch
                {
                    1 => housingDict.GetValueOrDefault(key.ListingId),
                    2 => landDict.GetValueOrDefault(key.ListingId),
                    3 => commercialDict.GetValueOrDefault(key.ListingId),
                    _ => null
                };

                if (item != null)
                {
                    item.Highlights = BuildHighlightsForRental(item);
                    items.Add(item);
                }
            }

            var defs = await _context.AmenityDefinitions
                .AsNoTracking()
                .Where(d => d.isActive)
                .Select(d => new { d.AmenityId, d.Name, d.GroupName, d.SortOrder })
                .ToListAsync();
            var amenities = defs
                .Select(d =>
                {
                    var cnt = countMap.GetValueOrDefault(d.AmenityId);
                    return new AmenityFacetResult
                    {
                        Id = d.AmenityId,
                        Text = d.Name,
                        Group = d.GroupName,
                        SortOrder = d.SortOrder,
                        Count = cnt,
                        Selected = selectedSet.Contains(d.AmenityId),
                        Disabled = cnt == 0
                    };
                })
                .OrderBy(x => x.Group)
                .ThenBy(x => x.SortOrder)
                .ThenBy(x => x.Text)
                .ToList();
            return new RentalListingFilterResponseResult
            {
                Total = total,
                Items = items,
                Amenties = amenities
            };
        }

        private static List<CardHighlightViewModel> BuildHighlightsForRental(RentalListingBaseResult item)
        {
            if (item is RentalHousingListingResult h)
            {
                return ListingCardHighlightBuilder.Housing(
                    h.NumberOfRoom,
                    h.NetArea,
                    h.GrossArea,
                    h.NumberOfFloors,
                    h.NumberOfBathRoom,
                    h.BuildingAge);
            }

            if (item is RentalLandListingResult l)
            {
                return ListingCardHighlightBuilder.Land(
                    l.Area,
                    l.ZoningStatus,
                    null,
                    l.TitleDeedStatus);
            }

            if (item is RentalCommercialListingResult c)
            {
                return ListingCardHighlightBuilder.Commercial(
                    c.NetArea,
                    c.GrossArea,
                    null,
                    c.NumberOfSection,
                    c.NumberOfFloors,
                    c.BuildingAge);
            }

            return new List<CardHighlightViewModel>();
        }

        public async Task<ForRentalPropertForListingViewModel?> GetUnifiedForRentalPropertyById(int id, int? listingTypeId = null)
        {
            bool checkHousing = !listingTypeId.HasValue || listingTypeId == (int)PropertyListingType.RentalHousingListing;
            var vm = checkHousing ? await RentalHousingRows().FirstOrDefaultAsync(x => x.ListingId == id) : null;
            if (vm != null)
            {
                vm.Amenities = await LoadAmenitiesAsync(_context.RentalHousingListingAmenities.Where(x => x.RentalHousingListId == id).Select(x => x.AmenityDefinition));
                await FillPhotoSlotsAsync(new[] { vm }, x => (8, id));
                return vm;
            }

            bool checkLand = !listingTypeId.HasValue || listingTypeId == (int)PropertyListingType.RentalLandListing;
            vm = checkLand ? await RentalLandRows().FirstOrDefaultAsync(x => x.ListingId == id) : null;
            if (vm != null)
            {
                vm.Amenities = await LoadAmenitiesAsync(_context.RentalLandListingAmenities.Where(x => x.RentalLandListingId == id).Select(x => x.AmenityDefinition));
                await FillPhotoSlotsAsync(new[] { vm }, x => (7, id));
                return vm;
            }

            bool checkCommercial = !listingTypeId.HasValue || listingTypeId == (int)PropertyListingType.RentalCommercialPropertyListing;
            vm = checkCommercial ? await RentalCommercialRows().FirstOrDefaultAsync(x => x.ListingId == id) : null;
            if (vm != null)
            {
                vm.Amenities = await LoadAmenitiesAsync(_context.RentalCommercialListingAmenities.Where(x => x.RentalCommercialListId == id).Select(x => x.AmenityDefinition));
                await FillPhotoSlotsAsync(new[] { vm }, x => (9, id));
                return vm;
            }

            return null;
        }
    }
}
