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
using FibiEmlakDanismanlik.Persistence.Migrations;
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
                                                  PropImgUrl1 = rentalCommercial.PropImgUrl1,
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
                                            PropImgUrl1 = rentalLand.PropImgUrl1,
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
                                               PropImgUrl1 = rentalHousing.PropImgUrl1,
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

        public List<ForRentalPropertForListingViewModel> GetAllForRentalPropertyForListing()
        {
            var rentalHousingList = from rentalHouse in _context.rentalHousingListings
                                    join agent in _context.Agents on rentalHouse.AgentId equals agent.AgentId
                                    select
                                    new ForRentalPropertForListingViewModel
                                    {
                                        RentalHousingListId = rentalHouse.RentalHousingListId,
                                        PropertyNo = rentalHouse.PropertyNo,
                                        PropertyName = rentalHouse.PropertyName,
                                        PropertyDescription = rentalHouse.PropertyDescription,
                                        PropertyStatus = rentalHouse.PropertyStatus,
                                        CreatedDate = rentalHouse.CreatedDate,
                                        Rent = rentalHouse.Rent,
                                        BuildingAge = rentalHouse.BuildingAge,
                                        GrossArea = rentalHouse.GrossArea,
                                        NetArea = rentalHouse.NetArea,
                                        Heating = rentalHouse.Heating,
                                        NumberOfFloors = rentalHouse.NumberOfFloors,
                                        HousingCategoryId = rentalHouse.HousingCategoryId,
                                        PropertyType = "Konut",
                                        Deposit = rentalHouse.Deposit,


                                        //adress
                                        City = rentalHouse.City,
                                        District = rentalHouse.District,
                                        Neighborhood = rentalHouse.Neighborhood,
                                        AddressDesc = rentalHouse.AddressDesc,



                                        IsElevator = rentalHouse.IsElevator,
                                        OpenArea = rentalHouse.OpenArea,
                                        TotalNumberOfFloor = rentalHouse.TotalNumberOfFloor,
                                        NumberOfBathRoom = rentalHouse.NumberOfBathRoom,
                                        NumberOfBalconies = rentalHouse.NumberOfBalconies,
                                        NumberOfRoom = rentalHouse.NumberOfRoom,
                                        BlackBox = rentalHouse.BlackBox,
                                        ParkingLot = rentalHouse.ParkingLot,
                                        Furnished = rentalHouse.Furnished,
                                        WithinTheComplex = rentalHouse.WithinTheComplex,
                                        Dues = rentalHouse.Dues,
                                        RentalCommercialListId = null,
                                        RentalLandListingId = null,

                                        // Arsa alanları boş
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
                                        BestDeals = rentalHouse.BestDeals,
                                        LandCategoryId = null,

                                        // // İşyeri alanlrı boş  
                                        Facade = null,
                                        NumberOfSection = null,
                                        NumberOfKitchens = null,
                                        NumberOfBathrooms = null,
                                        LandCategory = null,




                                        //relational

                                        AgentId = rentalHouse.AgentId,
                                        AgentImgUrl = agent.AgentImgUrl,
                                        AgentName = agent.AgentName,
                                        AgentDescription = agent.AgentDescription,
                                        AgentPhoneNumber = agent.AgentPhoneNumber,
                                        ListingTypeId = rentalHouse.ListingTypeId,



                                        //imgg
                                        PropImgUrl1 = rentalHouse.PropImgUrl1,
                                        PropImgUrl10 = rentalHouse.PropImgUrl10,
                                        PropImgUrl11 = rentalHouse.PropImgUrl11,
                                        PropImgUrl12 = rentalHouse.PropImgUrl12,
                                        PropImgUrl13 = rentalHouse.PropImgUrl13,
                                        PropImgUrl14 = rentalHouse.PropImgUrl14,
                                        PropImgUrl15 = rentalHouse.PropImgUrl15,
                                        PropImgUrl16 = rentalHouse.PropImgUrl16,
                                        PropImgUrl17 = rentalHouse.PropImgUrl17,
                                        PropImgUrl18 = rentalHouse.PropImgUrl18,
                                        PropImgUrl19 = rentalHouse.PropImgUrl19,
                                        PropImgUrl2 = rentalHouse.PropImgUrl2,
                                        PropImgUrl20 = rentalHouse.PropImgUrl20,
                                        PropImgUrl21 = rentalHouse.PropImgUrl21,
                                        PropImgUrl22 = rentalHouse.PropImgUrl22,
                                        PropImgUrl23 = rentalHouse.PropImgUrl23,
                                        PropImgUrl24 = rentalHouse.PropImgUrl24,
                                        PropImgUrl25 = rentalHouse.PropImgUrl25,
                                        PropImgUrl26 = rentalHouse.PropImgUrl26,
                                        PropImgUrl27 = rentalHouse.PropImgUrl27,
                                        PropImgUrl28 = rentalHouse.PropImgUrl28,
                                        PropImgUrl29 = rentalHouse.PropImgUrl29,
                                        PropImgUrl3 = rentalHouse.PropImgUrl3,
                                        PropImgUrl30 = rentalHouse.PropImgUrl30,
                                        PropImgUrl4 = rentalHouse.PropImgUrl4,
                                        PropImgUrl5 = rentalHouse.PropImgUrl5,
                                        PropImgUrl6 = rentalHouse.PropImgUrl6,
                                        PropImgUrl7 = rentalHouse.PropImgUrl7,
                                        PropImgUrl8 = rentalHouse.PropImgUrl8,
                                        PropImgUrl9 = rentalHouse.PropImgUrl9,

                                    };

            var rentalCommercialList = from rentalCommercial in _context.rentalCommercialPropertyListings
                                       join agent in _context.Agents on rentalCommercial.AgentId equals agent.AgentId
                                       select new ForRentalPropertForListingViewModel
                                       {
                                           //BaseModel

                                           PropertyNo = rentalCommercial.PropertyNo,
                                           PropertyName = rentalCommercial.PropertyName,
                                           PropertyStatus = rentalCommercial.PropertyStatus,
                                           PropertyDescription = rentalCommercial.PropertyDescription,
                                           CreatedDate = rentalCommercial.CreatedDate,
                                           Rent = rentalCommercial.Rent,
                                           BestDeals = rentalCommercial.BestDeals,
                                           PropertyType = "İşyeri",
                                           Deposit = rentalCommercial.Deposit,


                                           //BaseModel Adress

                                           City = rentalCommercial.City,
                                           District = rentalCommercial.District,
                                           Neighborhood = rentalCommercial.Neighborhood,
                                           AddressDesc = rentalCommercial.AddressDesc,

                                           //Relational

                                           AgentId = rentalCommercial.AgentId,
                                           AgentName = agent.AgentName,
                                           AgentDescription = agent.AgentDescription,
                                           AgentImgUrl = agent.AgentImgUrl,
                                           AgentPhoneNumber = agent.AgentPhoneNumber,

                                           ListingTypeId = rentalCommercial.ListingTypeId,
                                           HousingCategoryId = null,


                                           //ViewModelRental

                                           BuildingAge = rentalCommercial.BuildingAge,
                                           GrossArea = rentalCommercial.GrossArea,
                                           NetArea = rentalCommercial.NetArea,
                                           Heating = rentalCommercial.Heating,
                                           NumberOfFloors = rentalCommercial.NumberOfFloors,
                                           RentalCommercialListId = rentalCommercial.RentalCommercialListId,
                                           RentalHousingListId = null,
                                           RentalLandListingId = null,


                                           IsElevator = null,
                                           OpenArea = null,
                                           TotalNumberOfFloor = null,
                                           NumberOfBathRoom = null,
                                           NumberOfBalconies = null,
                                           NumberOfRoom = null,
                                           ParkingLot = null,
                                           Furnished = null,
                                           WithinTheComplex = null,
                                           Dues = null,
                                           BlackBox = null,


                                           Facade = rentalCommercial.Facade,
                                           NumberOfSection = rentalCommercial.NumberOfSection,
                                           NumberOfKitchens = rentalCommercial.NumberOfKitchens,
                                           NumberOfBathrooms = rentalCommercial.NumberOfBathrooms,


                                           ZoningStatus = null,
                                           Area = null,
                                           PricePerSquareMeter = null,
                                           ParcelNumber = null,
                                           PlotNumber = null,
                                           MapSheetNumber = null,
                                           FloorAreaRatio = null,
                                           BaseAreaRatio = null,
                                           ZoningPlan = null,
                                           DevelopmentRight = null,
                                           LandCategoryId = null,
                                           LandCategory = null,

                                           //img

                                           PropImgUrl1 = rentalCommercial.PropImgUrl1,
                                           PropImgUrl10 = rentalCommercial.PropImgUrl10,
                                           PropImgUrl11 = rentalCommercial.PropImgUrl11,
                                           PropImgUrl12 = rentalCommercial.PropImgUrl12,
                                           PropImgUrl13 = rentalCommercial.PropImgUrl13,
                                           PropImgUrl14 = rentalCommercial.PropImgUrl14,
                                           PropImgUrl15 = rentalCommercial.PropImgUrl15,
                                           PropImgUrl16 = rentalCommercial.PropImgUrl16,
                                           PropImgUrl17 = rentalCommercial.PropImgUrl17,
                                           PropImgUrl18 = rentalCommercial.PropImgUrl18,
                                           PropImgUrl19 = rentalCommercial.PropImgUrl19,
                                           PropImgUrl2 = rentalCommercial.PropImgUrl2,
                                           PropImgUrl20 = rentalCommercial.PropImgUrl20,
                                           PropImgUrl21 = rentalCommercial.PropImgUrl21,
                                           PropImgUrl22 = rentalCommercial.PropImgUrl22,
                                           PropImgUrl23 = rentalCommercial.PropImgUrl23,
                                           PropImgUrl24 = rentalCommercial.PropImgUrl24,
                                           PropImgUrl25 = rentalCommercial.PropImgUrl25,
                                           PropImgUrl26 = rentalCommercial.PropImgUrl26,
                                           PropImgUrl27 = rentalCommercial.PropImgUrl27,
                                           PropImgUrl28 = rentalCommercial.PropImgUrl28,
                                           PropImgUrl29 = rentalCommercial.PropImgUrl29,
                                           PropImgUrl3 = rentalCommercial.PropImgUrl3,
                                           PropImgUrl30 = rentalCommercial.PropImgUrl30,
                                           PropImgUrl4 = rentalCommercial.PropImgUrl4,
                                           PropImgUrl5 = rentalCommercial.PropImgUrl5,
                                           PropImgUrl6 = rentalCommercial.PropImgUrl6,
                                           PropImgUrl7 = rentalCommercial.PropImgUrl7,
                                           PropImgUrl8 = rentalCommercial.PropImgUrl8,
                                           PropImgUrl9 = rentalCommercial.PropImgUrl9,




                                       };
            var rentalLandList = from rentalLand in _context.rentalLandListings
                                 join agent in _context.Agents on rentalLand.AgentId equals agent.AgentId
                                 select new ForRentalPropertForListingViewModel
                                 {
                                     //baseModel
                                     PropertyNo = rentalLand.PropertyNo,
                                     PropertyName = rentalLand.PropertyName,
                                     PropertyDescription = rentalLand.PropertyDescription,
                                     PropertyStatus = rentalLand.PropertyStatus,
                                     CreatedDate = rentalLand.CreatedDate,
                                     Rent = rentalLand.Rent,
                                     BestDeals = rentalLand.BestDeals,
                                     PropertyType = "Arsa",

                                     //baseModel Adress

                                     City = rentalLand.City,
                                     District = rentalLand.District,
                                     Neighborhood = rentalLand.Neighborhood,
                                     AddressDesc = rentalLand.AddressDesc,



                                     //Relational

                                     AgentId = rentalLand.AgentId,
                                     AgentName = agent.AgentName,
                                     AgentDescription = agent.AgentDescription,
                                     AgentImgUrl = agent.AgentImgUrl,
                                     AgentPhoneNumber = agent.AgentPhoneNumber,

                                     ListingTypeId = rentalLand.ListingTypeId,


                                     Deposit = null,

                                     //ViewModel

                                     BuildingAge = null,
                                     GrossArea = null,
                                     NetArea = null,
                                     Heating = null,
                                     NumberOfFloors = null,

                                     RentalCommercialListId = null,
                                     RentalHousingListId = null,
                                     RentalLandListingId = rentalLand.RentalLandListingId,

                                     HousingCategoryId = null,
                                     IsElevator = null,
                                     OpenArea = null,
                                     TotalNumberOfFloor = null,
                                     NumberOfBathRoom = null,
                                     NumberOfRoom = null,
                                     NumberOfBalconies = null,
                                     BlackBox = null,
                                     ParkingLot = null,
                                     Furnished = null,
                                     WithinTheComplex = null,
                                     Dues = null,

                                     Facade = null,
                                     NumberOfSection = null,
                                     NumberOfKitchens = null,
                                     NumberOfBathrooms = null,
                                     LandCategory = null,

                                     ZoningStatus = rentalLand.ZoningStatus,
                                     Area = rentalLand.Area,
                                     PricePerSquareMeter = rentalLand.PricePerSquareMeter,
                                     ParcelNumber = rentalLand.ParcelNumber,
                                     PlotNumber = rentalLand.PlotNumber,
                                     MapSheetNumber = rentalLand.MapSheetNumber,
                                     FloorAreaRatio = rentalLand.FloorAreaRatio,
                                     BaseAreaRatio = rentalLand.BaseAreaRatio,
                                     ZoningPlan = rentalLand.ZoningPlan,
                                     DevelopmentRight = rentalLand.DevelopmentRight,
                                     LandCategoryId = rentalLand.LandCategoryId,

                                     //img

                                     PropImgUrl1 = rentalLand.PropImgUrl1,
                                     PropImgUrl10 = rentalLand.PropImgUrl10,
                                     PropImgUrl11 = rentalLand.PropImgUrl11,
                                     PropImgUrl12 = rentalLand.PropImgUrl12,
                                     PropImgUrl13 = rentalLand.PropImgUrl13,
                                     PropImgUrl14 = rentalLand.PropImgUrl14,
                                     PropImgUrl15 = rentalLand.PropImgUrl15,
                                     PropImgUrl16 = rentalLand.PropImgUrl16,
                                     PropImgUrl17 = rentalLand.PropImgUrl17,
                                     PropImgUrl18 = rentalLand.PropImgUrl18,
                                     PropImgUrl19 = rentalLand.PropImgUrl19,
                                     PropImgUrl2 = rentalLand.PropImgUrl2,
                                     PropImgUrl20 = rentalLand.PropImgUrl20,
                                     PropImgUrl21 = rentalLand.PropImgUrl21,
                                     PropImgUrl22 = rentalLand.PropImgUrl22,
                                     PropImgUrl23 = rentalLand.PropImgUrl23,
                                     PropImgUrl24 = rentalLand.PropImgUrl24,
                                     PropImgUrl25 = rentalLand.PropImgUrl25,
                                     PropImgUrl26 = rentalLand.PropImgUrl26,
                                     PropImgUrl27 = rentalLand.PropImgUrl27,
                                     PropImgUrl28 = rentalLand.PropImgUrl28,
                                     PropImgUrl29 = rentalLand.PropImgUrl29,
                                     PropImgUrl3 = rentalLand.PropImgUrl3,
                                     PropImgUrl30 = rentalLand.PropImgUrl30,
                                     PropImgUrl4 = rentalLand.PropImgUrl4,
                                     PropImgUrl5 = rentalLand.PropImgUrl5,
                                     PropImgUrl6 = rentalLand.PropImgUrl6,
                                     PropImgUrl7 = rentalLand.PropImgUrl7,
                                     PropImgUrl8 = rentalLand.PropImgUrl8,
                                     PropImgUrl9 = rentalLand.PropImgUrl9,

                                 };

            var listing = rentalHousingList.Concat(rentalLandList).Concat(rentalCommercialList).OrderByDescending(x => x.CreatedDate).ToList();

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

                    ImageUrls = CollectImages(
                        h.PropImgUrl1, h.PropImgUrl2, h.PropImgUrl3, h.PropImgUrl4, h.PropImgUrl5,
                        h.PropImgUrl6, h.PropImgUrl7, h.PropImgUrl8, h.PropImgUrl9, h.PropImgUrl10,
                        h.PropImgUrl11, h.PropImgUrl12, h.PropImgUrl13, h.PropImgUrl14, h.PropImgUrl15,
                        h.PropImgUrl16, h.PropImgUrl17, h.PropImgUrl18, h.PropImgUrl19, h.PropImgUrl20,
                        h.PropImgUrl21, h.PropImgUrl22, h.PropImgUrl23, h.PropImgUrl24, h.PropImgUrl25,
                        h.PropImgUrl26, h.PropImgUrl27, h.PropImgUrl28, h.PropImgUrl29, h.PropImgUrl30
                    ),

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

            if (filter.ListingTypeIds != null && filter.ListingTypeIds.Any())
                query = query.Where(x => filter.ListingTypeIds.Contains(x.ListingTypeId));

            if (filter.CityId.HasValue)
                query = query.Where(x => x.CityId == filter.CityId);
            else if (!string.IsNullOrWhiteSpace(filter.City))
                query = query.Where(x => x.City == filter.City);

            if (filter.DistrictId.HasValue)
                query = query.Where(x => x.DistrictId == filter.DistrictId);
            else if (!string.IsNullOrWhiteSpace(filter.District))
                query = query.Where(x => x.District == filter.District);

            if (filter.NeighborhoodId.HasValue)
                query = query.Where(x => x.NeighborhoodId == filter.NeighborhoodId);
            else if (!string.IsNullOrWhiteSpace(filter.Neighborhood))
                query = query.Where(x => x.Neighborhood == filter.Neighborhood);

            if (filter.MinPrice.HasValue)
                query = query.Where(x => x.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(x => x.Price <= filter.MaxPrice.Value);

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

            if (filter.AgentId.HasValue)
                query = query.Where(x => x.AgentId == filter.AgentId.Value);

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

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.Trim();
                var dashIdx = term.IndexOf(" - ", StringComparison.Ordinal);
                var namePart = dashIdx >= 0 ? term.Substring(dashIdx + 3).Trim() : term;
                query = query.Where(x =>
                    (x.PropertyName != null && (x.PropertyName.Contains(term) || (namePart.Length > 0 && x.PropertyName.Contains(namePart))))
                    || (x.PropertyNo != null && x.PropertyNo.ToString() == term)
                    || (x.City != null && x.City.Contains(term))
                    || (x.District != null && x.District.Contains(term))
                    || (x.Neighborhood != null && x.Neighborhood.Contains(term)));
            }

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

                    ImageUrls = CollectImages(l.PropImgUrl1),

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

            // Ortak filtreler
            if (filter.ListingTypeIds != null && filter.ListingTypeIds.Any())
                query = query.Where(x => filter.ListingTypeIds.Contains(x.ListingTypeId));

            if (filter.CityId.HasValue)
                query = query.Where(x => x.CityId == filter.CityId);
            else if (!string.IsNullOrWhiteSpace(filter.City))
                query = query.Where(x => x.City == filter.City);

            if (filter.DistrictId.HasValue)
                query = query.Where(x => x.DistrictId == filter.DistrictId);
            else if (!string.IsNullOrWhiteSpace(filter.District))
                query = query.Where(x => x.District == filter.District);

            if (filter.NeighborhoodId.HasValue)
                query = query.Where(x => x.NeighborhoodId == filter.NeighborhoodId);
            else if (!string.IsNullOrWhiteSpace(filter.Neighborhood))
                query = query.Where(x => x.Neighborhood == filter.Neighborhood);

            if (filter.MinPrice.HasValue)
                query = query.Where(x => x.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(x => x.Price <= filter.MaxPrice.Value);

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

            if (filter.AgentId.HasValue)
                query = query.Where(x => x.AgentId == filter.AgentId.Value);

            if (!string.IsNullOrWhiteSpace(filter.ZoningStatus))
                query = query.Where(x => x.ZoningStatus == filter.ZoningStatus);

            if (filter.LandCategoryId.HasValue)
                query = query.Where(x => x.LandCategoryId == filter.LandCategoryId.Value);
            if (!string.IsNullOrWhiteSpace(filter.TitleDeedStatus))
                query = query.Where(x => x.TitleDeedStatus == filter.TitleDeedStatus);

            if (filter.HousingCategoryId.HasValue || !string.IsNullOrWhiteSpace(filter.BuildingAge) || !string.IsNullOrWhiteSpace(filter.Heating) || filter.Furnished.HasValue || filter.WithinTheComplex.HasValue || filter.LoanEligible.HasValue || filter.Transferable.HasValue)
                query = query.Where(x => false);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.Trim();
                var dashIdx = term.IndexOf(" - ", StringComparison.Ordinal);
                var namePart = dashIdx >= 0 ? term.Substring(dashIdx + 3).Trim() : term;
                query = query.Where(x =>
                    (x.PropertyName != null && (x.PropertyName.Contains(term) || (namePart.Length > 0 && x.PropertyName.Contains(namePart))))
                    || (x.PropertyNo != null && x.PropertyNo.ToString() == term)
                    || (x.City != null && x.City.Contains(term))
                    || (x.District != null && x.District.Contains(term))
                    || (x.Neighborhood != null && x.Neighborhood.Contains(term)));
            }

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

                    ImageUrls = CollectImages(c.PropImgUrl1),

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

            // Ortak filtreler
            if (filter.ListingTypeIds != null && filter.ListingTypeIds.Any())
                query = query.Where(x => filter.ListingTypeIds.Contains(x.ListingTypeId));

            if (filter.CityId.HasValue)
                query = query.Where(x => x.CityId == filter.CityId);
            else if (!string.IsNullOrWhiteSpace(filter.City))
                query = query.Where(x => x.City == filter.City);

            if (filter.DistrictId.HasValue)
                query = query.Where(x => x.DistrictId == filter.DistrictId);
            else if (!string.IsNullOrWhiteSpace(filter.District))
                query = query.Where(x => x.District == filter.District);

            if (filter.NeighborhoodId.HasValue)
                query = query.Where(x => x.NeighborhoodId == filter.NeighborhoodId);
            else if (!string.IsNullOrWhiteSpace(filter.Neighborhood))
                query = query.Where(x => x.Neighborhood == filter.Neighborhood);

            if (filter.MinPrice.HasValue)
                query = query.Where(x => x.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(x => x.Price <= filter.MaxPrice.Value);

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

            if (filter.AgentId.HasValue)
                query = query.Where(x => x.AgentId == filter.AgentId.Value);

            if (!string.IsNullOrWhiteSpace(filter.ZoningStatus))
                query = query.Where(x => false);

            if (!string.IsNullOrWhiteSpace(filter.BuildingAge))
                query = query.Where(x => x.BuildingAge == filter.BuildingAge);
            if (!string.IsNullOrWhiteSpace(filter.Heating))
                query = query.Where(x => x.Heating == filter.Heating);

            if (filter.HousingCategoryId.HasValue || filter.LandCategoryId.HasValue || !string.IsNullOrWhiteSpace(filter.TitleDeedStatus) || filter.Furnished.HasValue || filter.WithinTheComplex.HasValue || filter.LoanEligible.HasValue || filter.Transferable.HasValue)
                query = query.Where(x => false);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.Trim();
                var dashIdx = term.IndexOf(" - ", StringComparison.Ordinal);
                var namePart = dashIdx >= 0 ? term.Substring(dashIdx + 3).Trim() : term;
                query = query.Where(x =>
                    (x.PropertyName != null && (x.PropertyName.Contains(term) || (namePart.Length > 0 && x.PropertyName.Contains(namePart))))
                    || (x.PropertyNo != null && x.PropertyNo.ToString() == term)
                    || (x.City != null && x.City.Contains(term))
                    || (x.District != null && x.District.Contains(term))
                    || (x.Neighborhood != null && x.Neighborhood.Contains(term)));
            }

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
            List<AmenityItemViewModel> amenities = new();

            bool checkHousing = !listingTypeId.HasValue || listingTypeId == (int)PropertyListingType.RentalHousingListing;
            var housingById = checkHousing
                ? await _context.rentalHousingListings
                    .Include(x => x.Agent)
                    .Include(x => x.ListingType)
                    .Include(x => x.HousingCategory)
                    .FirstOrDefaultAsync(x => x.RentalHousingListId == id)
                : null;

            if (housingById != null)
            {
                amenities = await _context.RentalHousingListingAmenities
            .Where(x => x.RentalHousingListId == id && x.AmenityDefinition.isActive == true)
            .Include(x => x.AmenityDefinition)
            .Select(x => new AmenityItemViewModel
            {
                Id = x.AmenityDefinition.AmenityId,
                Text = x.AmenityDefinition.Name,
                Group = x.AmenityDefinition.GroupName,
                SortOrder = x.AmenityDefinition.SortOrder
            })
            .OrderBy(x => x.Group)
            .ThenBy(x => x.SortOrder)
            .ToListAsync();
                return new ForRentalPropertForListingViewModel
                {
                    
                    ListingId = housingById.RentalHousingListId,
                    Deposit = housingById.Deposit,
                    HousingCategoryId=housingById.HousingCategory.HousingCategoryId,
                    HousingCategoryName=housingById.HousingCategory?.HousingCategoryName,
                    AgentDescription =housingById.Agent.AgentDescription,
                    ListingTypeName=housingById.ListingType.Name,
                    PropertyNo = housingById.PropertyNo,
                    PropertyName = housingById.PropertyName,
                    PropertyDescription = housingById.PropertyDescription,
                    PropertyStatus = housingById.PropertyStatus,
                    CreatedDate = housingById.CreatedDate,
                    City = housingById.City,
                    District = housingById.District,
                    Neighborhood = housingById.Neighborhood,
                    AddressDesc = housingById.AddressDesc,
                    Facade = housingById.Facade,
                    IsElevator = housingById.IsElevator,
                    GrossArea = housingById.GrossArea,
                    NetArea = housingById.NetArea,
                    OpenArea = housingById.OpenArea,
                    BuildingAge = housingById.BuildingAge,
                    TotalNumberOfFloor = housingById.TotalNumberOfFloor,
                    NumberOfFloors = housingById.NumberOfFloors,
                    NumberOfBathRoom = housingById.NumberOfBathRoom,
                    NumberOfRoom = housingById.NumberOfRoom,
                    Heating = housingById.Heating,
                    BlackBox = housingById.BlackBox,
                    NumberOfBalconies = housingById.NumberOfBalconies,
                    ParkingLot = housingById.ParkingLot,
                    Furnished = housingById.Furnished,
                    WithinTheComplex = housingById.WithinTheComplex,
                    Rent=housingById.Rent,
                    Dues = housingById.Dues,
                    AgentId = housingById.AgentId,
                    AgentName = housingById.Agent.AgentName,
                    AgentTitle = housingById.Agent.AgentTitle,
                    AgentImgUrl = housingById.Agent.AgentImgUrl,
                    BestDeals = housingById.BestDeals,
                    ListingType = "Konut",
                    PropertyType = "Kiralık",
                    ListingTypeId = housingById.ListingTypeId,
                    UsageTypeId = (int)housingById.ListingType.UsageType,
                    Amenities = amenities,
                    AgentPhoneNumber = housingById.Agent.AgentPhoneNumber,
                    Mail = housingById.Agent.Mail,
                    //Images
                    PropImgUrl1 = housingById.PropImgUrl1,
                    PropImgUrl10 = housingById.PropImgUrl10,
                    PropImgUrl11 = housingById.PropImgUrl11,
                    PropImgUrl12 = housingById.PropImgUrl12,
                    PropImgUrl13 = housingById.PropImgUrl13,
                    PropImgUrl14 = housingById.PropImgUrl14,
                    PropImgUrl15 = housingById.PropImgUrl15,
                    PropImgUrl16 = housingById.PropImgUrl16,
                    PropImgUrl17 = housingById.PropImgUrl17,
                    PropImgUrl18 = housingById.PropImgUrl18,
                    PropImgUrl19 = housingById.PropImgUrl19,
                    PropImgUrl2 = housingById.PropImgUrl2,
                    PropImgUrl20 = housingById.PropImgUrl20,
                    PropImgUrl21 = housingById.PropImgUrl21,
                    PropImgUrl22 = housingById.PropImgUrl22,
                    PropImgUrl23 = housingById.PropImgUrl23,
                    PropImgUrl24 = housingById.PropImgUrl24,
                    PropImgUrl25 = housingById.PropImgUrl25,
                    PropImgUrl26 = housingById.PropImgUrl26,
                    PropImgUrl27 = housingById.PropImgUrl27,
                    PropImgUrl28 = housingById.PropImgUrl28,
                    PropImgUrl29 = housingById.PropImgUrl29,
                    PropImgUrl3 = housingById.PropImgUrl3,
                    PropImgUrl30 = housingById.PropImgUrl30,
                    PropImgUrl4 = housingById.PropImgUrl4,
                    PropImgUrl5 = housingById.PropImgUrl5,
                    PropImgUrl6 = housingById.PropImgUrl6,
                    PropImgUrl7 = housingById.PropImgUrl7,
                    PropImgUrl8 = housingById.PropImgUrl8,
                    PropImgUrl9 = housingById.PropImgUrl9,
                    //Images

                };
            }
            bool checkLand = !listingTypeId.HasValue || listingTypeId == (int)PropertyListingType.RentalLandListing;
            var land = checkLand
                ? await _context.rentalLandListings.Include(x => x.Agent).Include(x => x.ListingType).Include(x => x.LandCategory).FirstOrDefaultAsync(x => x.RentalLandListingId == id)
                : null;
            if (land != null)
            {
                amenities = await _context.RentalLandListingAmenities
            .Where(x => x.RentalLandListingId == id && x.AmenityDefinition.isActive == true)
            .Include(x => x.AmenityDefinition)
            .Select(x => new AmenityItemViewModel
            {
                Id = x.AmenityDefinition.AmenityId,
                Text = x.AmenityDefinition.Name,
                Group = x.AmenityDefinition.GroupName,
                SortOrder = x.AmenityDefinition.SortOrder
            })
            .OrderBy(x => x.Group)
            .ThenBy(x => x.SortOrder)
            .ToListAsync();
                return new ForRentalPropertForListingViewModel
                {
                    ListingId = land.RentalLandListingId,
                    PropertyNo = land.PropertyNo,
                    PropertyName = land.PropertyName,
                    PropertyDescription = land.PropertyDescription,
                    PropertyStatus = land.PropertyStatus,
                    CreatedDate = land.CreatedDate,
                    City = land.City,
                    District = land.District,
                    Neighborhood = land.Neighborhood,
                    AddressDesc = land.AddressDesc,
                    ZoningStatus = land.ZoningStatus,
                    Area = land.Area,
                    PricePerSquareMeter = land.PricePerSquareMeter,
                    ParcelNumber = land.ParcelNumber,
                    PlotNumber = land.PlotNumber,
                    MapSheetNumber = land.MapSheetNumber,
                    FloorAreaRatio = land.FloorAreaRatio,
                    BaseAreaRatio = land.BaseAreaRatio,
                    ZoningPlan = land.ZoningPlan,
                    DevelopmentRight = land.DevelopmentRight,
                    LandCategoryId = land.LandCategory.LandCategoryId,
                    LandCategoryName = land.LandCategory.LandCategoryName,
                    ListingTypeName = land.ListingType.Name,
                    AgentId = land.AgentId,
                    AgentImgUrl = land.Agent.AgentImgUrl,
                    AgentName = land.Agent.AgentName,
                    AgentTitle = land.Agent.AgentTitle,
                    AgentDescription=land.Agent.AgentDescription,
                    BestDeals = land.BestDeals,
                    TitleDeedStatus=land.TitleDeedStatus,
                    ListingType = "Arsa",
                    PropertyType = "Kiralık",
                    ListingTypeId = land.ListingTypeId,
                    UsageTypeId = (int)land.ListingType.UsageType,
                    AgentPhoneNumber = land.Agent.AgentPhoneNumber,
                    Mail = land.Agent.Mail,
                    Rent=land.Rent,
                    Amenities = amenities,
                    //Images 
                    PropImgUrl1 = land.PropImgUrl1,
                    PropImgUrl10 = land.PropImgUrl10,
                    PropImgUrl11 = land.PropImgUrl11,
                    PropImgUrl12 = land.PropImgUrl12,
                    PropImgUrl13 = land.PropImgUrl13,
                    PropImgUrl14 = land.PropImgUrl14,
                    PropImgUrl15 = land.PropImgUrl15,
                    PropImgUrl16 = land.PropImgUrl16,
                    PropImgUrl17 = land.PropImgUrl17,
                    PropImgUrl18 = land.PropImgUrl18,
                    PropImgUrl19 = land.PropImgUrl19,
                    PropImgUrl2 = land.PropImgUrl2,
                    PropImgUrl20 = land.PropImgUrl20,
                    PropImgUrl21 = land.PropImgUrl21,
                    PropImgUrl22 = land.PropImgUrl22,
                    PropImgUrl23 = land.PropImgUrl23,
                    PropImgUrl24 = land.PropImgUrl24,
                    PropImgUrl25 = land.PropImgUrl25,
                    PropImgUrl26 = land.PropImgUrl26,
                    PropImgUrl27 = land.PropImgUrl27,
                    PropImgUrl28 = land.PropImgUrl28,
                    PropImgUrl29 = land.PropImgUrl29,
                    PropImgUrl3 = land.PropImgUrl3,
                    PropImgUrl30 = land.PropImgUrl30,
                    PropImgUrl4 = land.PropImgUrl4,
                    PropImgUrl5 = land.PropImgUrl5,
                    PropImgUrl6 = land.PropImgUrl6,
                    PropImgUrl7 = land.PropImgUrl7,
                    PropImgUrl8 = land.PropImgUrl8,
                    PropImgUrl9 = land.PropImgUrl9,
                    //Images
                };
            }
            bool checkCommercial = !listingTypeId.HasValue || listingTypeId == (int)PropertyListingType.RentalCommercialPropertyListing;
            var commercialById = checkCommercial
                ? await _context.rentalCommercialPropertyListings.Include(x => x.Agent).Include(x => x.ListingType).FirstOrDefaultAsync(x => x.RentalCommercialListId == id)
                : null;
            if (commercialById != null)
            {
                amenities = await _context.RentalCommercialListingAmenities
            .Where(x => x.RentalCommercialListId == id && x.AmenityDefinition.isActive == true)
            .Include(x => x.AmenityDefinition)
            .Select(x => new AmenityItemViewModel
            {
                Id = x.AmenityDefinition.AmenityId,
                Text = x.AmenityDefinition.Name,
                Group = x.AmenityDefinition.GroupName,
                SortOrder = x.AmenityDefinition.SortOrder
            })
            .OrderBy(x => x.Group)
            .ThenBy(x => x.SortOrder)
            .ToListAsync();
                return new ForRentalPropertForListingViewModel
                {
                    ListingId = commercialById.RentalCommercialListId,
                    PropertyNo = commercialById.PropertyNo,
                    PropertyName = commercialById.PropertyName,
                    PropertyDescription = commercialById.PropertyDescription,
                    PropertyStatus = commercialById.PropertyStatus,
                    CreatedDate = commercialById.CreatedDate,
                    City = commercialById.City,
                    District = commercialById.District,
                    Neighborhood = commercialById.Neighborhood,
                    AddressDesc = commercialById.AddressDesc,
                    Facade = commercialById.Facade,
                    NumberOfSection = commercialById.NumberOfSection,
                    NumberOfKitchens = commercialById.NumberOfKitchens,
                    NumberOfBathrooms = commercialById.NumberOfBathrooms,
                    NumberOfFloors = commercialById.NumberOfFloors,
                    GrossArea = commercialById.GrossArea,
                    NetArea = commercialById.NetArea,
                    BestDeals = commercialById.BestDeals,
                    AgentId = commercialById.AgentId,
                    AgentName = commercialById.Agent.AgentName,
                    AgentImgUrl = commercialById.Agent.AgentImgUrl,
                    AgentTitle = commercialById.Agent.AgentTitle,
                    AgentDescription=commercialById.Agent.AgentDescription,
                    BuildingAge=commercialById.BuildingAge,
                    Heating=commercialById.Heating,
                    TitleDeedStatus=commercialById.TitleDeedStatus,
                    Rent=commercialById.Rent,
                    ListingType = "İşyeri",
                    PropertyType = "Kiralık",
                    ListingTypeId = commercialById.ListingTypeId,
                    UsageTypeId = (int)commercialById.ListingType.UsageType,
                    ListingTypeName = commercialById.ListingType.Name,
                    Amenities = amenities,
                    Mail = commercialById.Agent.Mail,
                    AgentPhoneNumber = commercialById.Agent.AgentPhoneNumber,
                    Deposit=commercialById.Deposit,
                    //Images 
                    PropImgUrl1 = commercialById.PropImgUrl1,
                    PropImgUrl10 = commercialById.PropImgUrl10,
                    PropImgUrl11 = commercialById.PropImgUrl11,
                    PropImgUrl12 = commercialById.PropImgUrl12,
                    PropImgUrl13 = commercialById.PropImgUrl13,
                    PropImgUrl14 = commercialById.PropImgUrl14,
                    PropImgUrl15 = commercialById.PropImgUrl15,
                    PropImgUrl16 = commercialById.PropImgUrl16,
                    PropImgUrl17 = commercialById.PropImgUrl17,
                    PropImgUrl18 = commercialById.PropImgUrl18,
                    PropImgUrl19 = commercialById.PropImgUrl19,
                    PropImgUrl2 = commercialById.PropImgUrl2,
                    PropImgUrl20 = commercialById.PropImgUrl20,
                    PropImgUrl21 = commercialById.PropImgUrl21,
                    PropImgUrl22 = commercialById.PropImgUrl22,
                    PropImgUrl23 = commercialById.PropImgUrl23,
                    PropImgUrl24 = commercialById.PropImgUrl24,
                    PropImgUrl25 = commercialById.PropImgUrl25,
                    PropImgUrl26 = commercialById.PropImgUrl26,
                    PropImgUrl27 = commercialById.PropImgUrl27,
                    PropImgUrl28 = commercialById.PropImgUrl28,
                    PropImgUrl29 = commercialById.PropImgUrl29,
                    PropImgUrl3 = commercialById.PropImgUrl3,
                    PropImgUrl30 = commercialById.PropImgUrl30,
                    PropImgUrl4 = commercialById.PropImgUrl4,
                    PropImgUrl5 = commercialById.PropImgUrl5,
                    PropImgUrl6 = commercialById.PropImgUrl6,
                    PropImgUrl7 = commercialById.PropImgUrl7,
                    PropImgUrl8 = commercialById.PropImgUrl8,
                    PropImgUrl9 = commercialById.PropImgUrl9,
                    //Images 
                };
            }
            return null;
        }
    }
}
