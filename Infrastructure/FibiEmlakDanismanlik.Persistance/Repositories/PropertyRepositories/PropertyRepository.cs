using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Requests.PropertyRequests;
using FibiEmlakDanismanlik.Application.Features.Results.AmenityFacetResults;
using FibiEmlakDanismanlik.Application.Features.Results.ForRentalPropertyResults;
using FibiEmlakDanismanlik.Application.Features.Results.ForSalePropertyResults;
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
    public class PropertyRepository : IPropertyRepository
    {
        private readonly FibiEmlakDanismanlikContext _context;

        private IQueryable<ForSalePropertyForListingViewModel> BuildForSaleListingQuery()
        {
            var activeStatuses = PropertyStatuses.ActiveSet;

            var housingQuery =
    from house in _context.forSaleHousingPropertyListings
    join agent in _context.Agents on house.AgentId equals agent.AgentId
    join lt in _context.listingTypes on house.ListingTypeId equals lt.ListingTypeId
    where lt.UsageType == UsageType.ForSale
          && house.PropertyStatus != null
          && activeStatuses.Contains(house.PropertyStatus)
    select new ForSalePropertyForListingViewModel
    {


        ListingId = house.ForSaleHousingListId,

        ListingTypeId = house.ListingTypeId,
        PropertyNo = house.PropertyNo,
        PropertyName = house.PropertyName,
        PropertyDescription = house.PropertyDescription,
        PropertyStatus = house.PropertyStatus,
        CreatedDate = house.CreatedDate,
        Price = house.Price,
        TitleDeedStatus = house.TitleDeedStatus,

        City = house.City,
        District = house.District,
        Neighborhood = house.Neighborhood,
        AddressDesc = house.AddressDesc,
        SourceType = 1,  // usage type 1 satılık konut 3 ise kiralık konut 
        UsageTypeId = (int)lt.UsageType,



        // arsa boş
        Area = null,
        SharePercentage = null,
        PricePerSquareMeter = null,
        ParcelNumber = null,
        PlotNumber = null,
        MapSheetNumber = null,
        FloorAreaRatio = null,
        BaseAreaRatio = null,
        ZoningPlan = null,
        ZoningStatus = null,
        DevelopmentRight = null,
        LandLoan = null,
        Exchange = null,
        BestDeals = house.BestDeals,
        LandCategoryId = null,


        // konut dolu
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
        UsageStatus = house.UsageStatus,
        WithinTheComplex = house.WithinTheComplex,
        Dues = house.Dues,
        HomeLoan = house.HomeLoan,

        // ticari boş
        NumberOfSection = null,
        NumberOfKitchens = null,
        NumberOfBathrooms = null,
        Transferable = null,

        AgentId = house.AgentId,
        AgentName = agent.AgentName,
        AgentTitle = agent.AgentTitle,
        AgentImgUrl = agent.AgentImgUrl,

        ListingType = "Konut",


        PropImgUrl1 = house.PropImgUrl1,
        PropImgUrl2 = house.PropImgUrl2,
        PropImgUrl3 = house.PropImgUrl3,
        PropImgUrl4 = house.PropImgUrl4,
        PropImgUrl5 = house.PropImgUrl5,
        PropImgUrl6 = house.PropImgUrl6,
        PropImgUrl7 = house.PropImgUrl7,
        PropImgUrl8 = house.PropImgUrl8,
        PropImgUrl9 = house.PropImgUrl9,
        PropImgUrl10 = house.PropImgUrl10,
        PropImgUrl11 = house.PropImgUrl11,
        PropImgUrl12 = house.PropImgUrl12,
        PropImgUrl13 = house.PropImgUrl13,
        PropImgUrl14 = house.PropImgUrl14,
        PropImgUrl15 = house.PropImgUrl15,
        PropImgUrl16 = house.PropImgUrl16,
        PropImgUrl17 = house.PropImgUrl17,
        PropImgUrl18 = house.PropImgUrl18,
        PropImgUrl19 = house.PropImgUrl19,
        PropImgUrl20 = house.PropImgUrl20,
        PropImgUrl21 = house.PropImgUrl21,
        PropImgUrl22 = house.PropImgUrl22,
        PropImgUrl23 = house.PropImgUrl23,
        PropImgUrl24 = house.PropImgUrl24,
        PropImgUrl25 = house.PropImgUrl25,
        PropImgUrl26 = house.PropImgUrl26,
        PropImgUrl27 = house.PropImgUrl27,
        PropImgUrl28 = house.PropImgUrl28,
        PropImgUrl29 = house.PropImgUrl29,
        PropImgUrl30 = house.PropImgUrl30
    };

            var commercialQuery =
    from comm in _context.forSaleCommercialPropertyListings
    join agent in _context.Agents on comm.AgentId equals agent.AgentId
    join lt in _context.listingTypes on comm.ListingTypeId equals lt.ListingTypeId
    where lt.UsageType == UsageType.ForSale
          && comm.PropertyStatus != null
          && activeStatuses.Contains(comm.PropertyStatus)
    select new ForSalePropertyForListingViewModel
    {

        ListingId = comm.ForSaleCommercialListingId,
        ListingTypeId = comm.ListingTypeId,
        PropertyNo = comm.PropertyNo,
        PropertyName = comm.PropertyName,
        PropertyDescription = comm.PropertyDescription,
        PropertyStatus = comm.PropertyStatus,
        CreatedDate = comm.CreatedDate,
        Price = comm.Price,
        TitleDeedStatus = comm.TitleDeedStatus,
        SourceType = 3, // usage type 3 kullanarak satılık işyeri anlamına geliyor.
        UsageTypeId = (int)lt.UsageType,


        //        Highlights = new List<CardHighlightViewModel>
        //{
        //    new() { Icon = "icon-14", Text = $"{comm.Area} m²" },
        //    new() { Icon = "icon-15", Text = $"{comm.NumberOfFloors} Kat" },
        //    new() { Icon = "icon-16", Text = $"{comm.NumberOfSection} Bölüm" }
        //},

        City = comm.City,
        District = comm.District,
        Neighborhood = comm.Neighborhood,
        AddressDesc = comm.AddressDesc,

        // arsa boş
        Area = null,
        SharePercentage = null,
        PricePerSquareMeter = null,
        ParcelNumber = null,
        PlotNumber = null,
        MapSheetNumber = null,
        FloorAreaRatio = null,
        BaseAreaRatio = null,
        ZoningPlan = null,
        ZoningStatus = null,
        DevelopmentRight = null,
        LandLoan = null,
        Exchange = null,
        BestDeals = comm.BestDeals,
        LandCategoryId = null,


        // konut boş
        Facade = null,
        IsElevator = null,
        GrossArea = null,
        NetArea = null,
        OpenArea = null,
        BuildingAge = null,
        TotalNumberOfFloor = null,
        NumberOfFloors = null,
        NumberOfBathRoom = null,
        NumberOfRoom = null,
        Heating = null,
        BlackBox = null,
        NumberOfBalconies = null,
        ParkingLot = null,
        Furnished = null,
        UsageStatus = null,
        WithinTheComplex = null,
        Dues = null,
        HomeLoan = null,

        // ticari dolu
        NumberOfSection = comm.NumberOfSection,
        NumberOfKitchens = comm.NumberOfKitchens,
        NumberOfBathrooms = comm.NumberOfBathrooms,
        Transferable = comm.Transferable,

        AgentId = comm.AgentId,
        AgentName = agent.AgentName,
        AgentTitle = agent.AgentTitle,
        AgentImgUrl = agent.AgentImgUrl,

        ListingType = "İşyeri",

        PropImgUrl1 = comm.PropImgUrl1,
        PropImgUrl2 = comm.PropImgUrl2,
        PropImgUrl3 = comm.PropImgUrl3,
        PropImgUrl4 = comm.PropImgUrl4,
        PropImgUrl5 = comm.PropImgUrl5,
        PropImgUrl6 = comm.PropImgUrl6,
        PropImgUrl7 = comm.PropImgUrl7,
        PropImgUrl8 = comm.PropImgUrl8,
        PropImgUrl9 = comm.PropImgUrl9,
        PropImgUrl10 = comm.PropImgUrl10,
        PropImgUrl11 = comm.PropImgUrl11,
        PropImgUrl12 = comm.PropImgUrl12,
        PropImgUrl13 = comm.PropImgUrl13,
        PropImgUrl14 = comm.PropImgUrl14,
        PropImgUrl15 = comm.PropImgUrl15,
        PropImgUrl16 = comm.PropImgUrl16,
        PropImgUrl17 = comm.PropImgUrl17,
        PropImgUrl18 = comm.PropImgUrl18,
        PropImgUrl19 = comm.PropImgUrl19,
        PropImgUrl20 = comm.PropImgUrl20,
        PropImgUrl21 = comm.PropImgUrl21,
        PropImgUrl22 = comm.PropImgUrl22,
        PropImgUrl23 = comm.PropImgUrl23,
        PropImgUrl24 = comm.PropImgUrl24,
        PropImgUrl25 = comm.PropImgUrl25,
        PropImgUrl26 = comm.PropImgUrl26,
        PropImgUrl27 = comm.PropImgUrl27,
        PropImgUrl28 = comm.PropImgUrl28,
        PropImgUrl29 = comm.PropImgUrl29,
        PropImgUrl30 = comm.PropImgUrl30
    };

            var landQuery =
                from land in _context.forSaleLandListings
                join agent in _context.Agents on land.AgentId equals agent.AgentId
                join lt in _context.listingTypes on land.ListingTypeId equals lt.ListingTypeId
                where lt.UsageType == UsageType.ForSale
                      && land.PropertyStatus != null
                      && activeStatuses.Contains(land.PropertyStatus)
                select new ForSalePropertyForListingViewModel
                {

                    ListingId = land.ForSaleLandListingId,
                    ListingTypeId = land.ListingTypeId,
                    PropertyNo = land.PropertyNo,
                    PropertyName = land.PropertyName,
                    PropertyDescription = land.PropertyDescription,
                    PropertyStatus = land.PropertyStatus,
                    CreatedDate = land.CreatedDate,
                    Price = land.Price,
                    TitleDeedStatus = land.TitleDeedStatus,
                    SourceType = 2, //usage type 2 - > satılık arsa anlamına geliyor
                    UsageTypeId = (int)lt.UsageType,

                    //                    Highlights = new List<CardHighlightViewModel>
                    //{
                    //    new() { Icon = "icon-14", Text = $"{land.Area} m²" },
                    //    new() { Icon = "icon-15", Text = $"{land.ZoningStatus} " },
                    //    new() { Icon = "icon-16", Text = $"{land.PricePerSquareMeter} TL/m²" }
                    //},

                    City = land.City,
                    District = land.District,
                    Neighborhood = land.Neighborhood,
                    AddressDesc = land.AddressDesc,

                    // arsa dolu
                    Area = land.Area,
                    SharePercentage = land.SharePercentage,
                    PricePerSquareMeter = land.PricePerSquareMeter,
                    ParcelNumber = land.ParcelNumber,
                    PlotNumber = land.PlotNumber,
                    MapSheetNumber = land.MapSheetNumber,
                    FloorAreaRatio = land.FloorAreaRatio,
                    BaseAreaRatio = land.BaseAreaRatio,
                    ZoningPlan = land.ZoningPlan,
                    ZoningStatus = land.ZoningStatus,
                    DevelopmentRight = land.DevelopmentRight,
                    LandLoan = land.LandLoan,
                    Exchange = land.Exchange,
                    BestDeals = land.BestDeals,
                    LandCategoryId = land.LandCategoryId,
                    

                    // konut boş
                    Facade = null,
                    IsElevator = null,
                    GrossArea = null,
                    NetArea = null,
                    OpenArea = null,
                    BuildingAge = null,
                    TotalNumberOfFloor = null,
                    NumberOfFloors = null,
                    NumberOfBathRoom = null,
                    NumberOfRoom = null,
                    Heating = null,
                    BlackBox = null,
                    NumberOfBalconies = null,
                    ParkingLot = null,
                    Furnished = null,
                    UsageStatus = null,
                    WithinTheComplex = null,
                    Dues = null,
                    HomeLoan = null,

                    // ticari boş
                    NumberOfSection = null,
                    NumberOfKitchens = null,
                    NumberOfBathrooms = null,
                    Transferable = null,

                    AgentId = land.AgentId,
                    AgentName = agent.AgentName,
                    AgentTitle = agent.AgentTitle,
                    AgentImgUrl = agent.AgentImgUrl,

                    ListingType = "Arsa",

                    PropImgUrl1 = land.PropImgUrl1,
                    PropImgUrl2 = land.PropImgUrl2,
                    PropImgUrl3 = land.PropImgUrl3,
                    PropImgUrl4 = land.PropImgUrl4,
                    PropImgUrl5 = land.PropImgUrl5,
                    PropImgUrl6 = land.PropImgUrl6,
                    PropImgUrl7 = land.PropImgUrl7,
                    PropImgUrl8 = land.PropImgUrl8,
                    PropImgUrl9 = land.PropImgUrl9,
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
                    PropImgUrl30 = land.PropImgUrl30
                };

            return housingQuery.Concat(landQuery).Concat(commercialQuery);
        }


        public PropertyRepository(FibiEmlakDanismanlikContext context)
        {
            _context = context;
        }

        public List<ForSalePropertyViewModel> GetAllForSalePropertyWithAgent()
        {
            var housingList = from housing in _context.forSaleHousingPropertyListings
                              join agent in _context.Agents
                              on housing.AgentId equals agent.AgentId
                              select new ForSalePropertyViewModel
                              {
                                  PropertyType = "Konut",
                                  PropertyName = housing.PropertyName,
                                  Price = housing.Price,
                                  PropertyDescription = housing.PropertyDescription,
                                  PropertyStatus = housing.PropertyStatus,
                                  AgentName = agent.AgentName,
                                  AgentTitle = agent.AgentTitle,
                                  AgentImgUrl = agent.AgentImgUrl,
                                  PropImgUrl1 = housing.PropImgUrl1,
                                  CreatedDate = housing.CreatedDate,
                              };

            var landListings = from land in _context.forSaleLandListings
                               join agent in _context.Agents
                               on land.AgentId equals agent.AgentId
                               select new ForSalePropertyViewModel
                               {
                                   PropertyType = "Arsa",
                                   PropertyName = land.PropertyName,
                                   Price = land.Price,
                                   PropertyDescription = land.PropertyDescription,
                                   PropertyStatus = land.PropertyStatus,
                                   AgentName = agent.AgentName,
                                   AgentTitle = agent.AgentTitle,
                                   AgentImgUrl = agent.AgentImgUrl,
                                   PropImgUrl1 = land.PropImgUrl1,
                                   CreatedDate = land.CreatedDate,
                               };

            var commercialListing = from commercial in _context.forSaleCommercialPropertyListings
                                    join agent in _context.Agents on commercial.AgentId equals agent.AgentId
                                    select new ForSalePropertyViewModel
                                    {
                                        PropertyType = "İşyeri",
                                        PropertyName = commercial.PropertyName,
                                        Price = commercial.Price,
                                        PropertyDescription = commercial.PropertyDescription,
                                        PropertyStatus = commercial.PropertyStatus,
                                        AgentImgUrl = agent.AgentImgUrl,
                                        AgentName = agent.AgentName,
                                        AgentTitle = agent.AgentTitle,
                                        PropImgUrl1 = commercial.PropImgUrl1,
                                        CreatedDate = commercial.CreatedDate,
                                    };

            var listings = housingList
                            .Concat(landListings)
                            .Concat(commercialListing)
                            .OrderByDescending(x => x.CreatedDate)
                            .Take(10)
                            .ToList();

            return listings;
        }

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

        public async Task<ForSalePropertyForListingViewModel> GetUnifiedForSalePropertyById(int id)
        {
            List<AmenityItemViewModel> amenities = new();

            var housingById = await _context.forSaleHousingPropertyListings.Include(x => x.Agent).Include(x => x.ListingType).FirstOrDefaultAsync(x => x.ForSaleHousingListId == id);

            if (housingById != null)
            {
                amenities = await _context.ForSaleHousingListingAmenities
            .Where(x => x.ForSaleHousingListId == id && x.AmenityDefinition.isActive == true)
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
                return new ForSalePropertyForListingViewModel
                {
                    ListingId = housingById.ForSaleHousingListId,
                    PropertyNo = housingById.PropertyNo,
                    PropertyName = housingById.PropertyName,
                    PropertyDescription = housingById.PropertyDescription,
                    PropertyStatus = housingById.PropertyStatus,
                    CreatedDate = housingById.CreatedDate,
                    Price = housingById.Price,
                    TitleDeedStatus = housingById.TitleDeedStatus,
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
                    Heating = housingById.Heating,
                    BlackBox = housingById.BlackBox,
                    NumberOfBalconies = housingById.NumberOfBalconies,
                    ParkingLot = housingById.ParkingLot,
                    Furnished = housingById.Furnished,
                    UsageStatus = housingById.UsageStatus,
                    WithinTheComplex = housingById.WithinTheComplex,
                    Dues = housingById.Dues,
                    Exchange = housingById.Exchange,
                    HomeLoan = housingById.HomeLoan,
                    AgentId = housingById.AgentId,
                    AgentName = housingById.Agent.AgentName,
                    AgentTitle = housingById.Agent.AgentTitle,
                    AgentImgUrl = housingById.Agent.AgentImgUrl,
                    BestDeals = housingById.BestDeals,
                    ListingType = "Konut",
                    ListingTypeId=housingById.ListingTypeId,
                    UsageTypeId= (int)housingById.ListingType.UsageType,
                    Amenities = amenities,
                    AgentPhoneNumber =housingById.Agent.AgentPhoneNumber,
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
            var land = await _context.forSaleLandListings.Include(x => x.Agent).Include(x => x.ListingType).Include(x => x.LandCategory).FirstOrDefaultAsync(x => x.ForSaleLandListingId == id);
            

            if (land != null)
            {
                amenities = await _context.ForSaleLandListingAmenities
            .Where(x => x.ForSaleLandListId == id && x.AmenityDefinition.isActive == true)
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
                return new ForSalePropertyForListingViewModel
                {
                    ListingId = land.ForSaleLandListingId,
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
                    SharePercentage = land.SharePercentage,
                    Area = land.Area,
                    Price = land.Price,
                    PricePerSquareMeter = land.PricePerSquareMeter,
                    ParcelNumber = land.ParcelNumber,
                    PlotNumber = land.PlotNumber,
                    MapSheetNumber = land.MapSheetNumber,
                    FloorAreaRatio = land.FloorAreaRatio,
                    BaseAreaRatio = land.BaseAreaRatio,
                    ZoningPlan = land.ZoningPlan,
                    TitleDeedStatus = land.TitleDeedStatus,
                    DevelopmentRight = land.DevelopmentRight,
                    LandLoan = land.LandLoan,
                    Exchange = land.Exchange,
                    LandCategoryId = land.LandCategoryId,
                    AgentId = land.AgentId,
                    AgentImgUrl = land.Agent.AgentImgUrl,
                    AgentName = land.Agent.AgentName,
                    AgentTitle = land.Agent.AgentTitle,
                    BestDeals = land.BestDeals,
                    ListingType = "Arsa",
                    ListingTypeId =land.ListingTypeId,
                    UsageTypeId= (int)land.ListingType.UsageType,
                    AgentPhoneNumber = land.Agent.AgentPhoneNumber,
                    Mail = land.Agent.Mail,

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
            var commercialById = await _context.forSaleCommercialPropertyListings.Include(x => x.Agent).Include(x => x.ListingType).FirstOrDefaultAsync(x => x.ForSaleCommercialListingId == id);
            if (commercialById != null)
            {
                amenities = await _context.ForSaleCommercialListingAmenities
            .Where(x => x.ForSaleCommercialListingId == id && x.AmenityDefinition.isActive== true)
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
                return new ForSalePropertyForListingViewModel
                {
                    ListingId = commercialById.ForSaleCommercialListingId,
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
                    Area = commercialById.Area,
                    Price = commercialById.Price,
                    TitleDeedStatus = commercialById.TitleDeedStatus,
                    SharePercentage = commercialById.SharePercentage,
                    LandLoan = commercialById.LandLoan,
                    Exchange = commercialById.Exchange,
                    Transferable = commercialById.Transferable,
                    BestDeals = commercialById.BestDeals,
                    AgentId = commercialById.AgentId,
                    AgentName = commercialById.Agent.AgentName,
                    AgentImgUrl = commercialById.Agent.AgentImgUrl,
                    AgentTitle = commercialById.Agent.AgentTitle,
                    ListingType = "İşyeri",
                    ListingTypeId = commercialById.ListingTypeId,
                    UsageTypeId = (int)commercialById.ListingType.UsageType,
                    Amenities=amenities,
                    Mail =commercialById.Agent.Mail,
                    AgentPhoneNumber =commercialById.Agent.AgentPhoneNumber,
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


        public async Task<List<ListingTypeFacetDto>> GetForSaleListingTypeFacetsAsync()
        {
            const int ForSaleUsageType = 1;
            var activeStatuses = PropertyStatuses.ActiveSet;
            var listingTypes = await _context.listingTypes.AsNoTracking().Where(x => x.UsageType == UsageType.ForSale).Select(x => new { x.ListingTypeId, x.Name }).ToListAsync();
            var housingTypeIds = _context.forSaleHousingPropertyListings
     .AsNoTracking()
     .Where(x => (x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus)) || x.PropertyStatus == null)
     .Select(x => x.ListingTypeId);

            var landTypeIds = _context.forSaleLandListings
        .AsNoTracking()
        .Where(x => (x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus)) || x.PropertyStatus == null)
        .Select(x => x.ListingTypeId);
            var commercialTypeIds = _context.forSaleCommercialPropertyListings
        .AsNoTracking()
        .Where(x => (x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus)) || x.PropertyStatus == null)
        .Select(x => x.ListingTypeId);
            var groupedCounts = await housingTypeIds
        .Concat(landTypeIds)
        .Concat(commercialTypeIds)
        .Where(id => id != null)
        .GroupBy(id => id!)
        .Select(g => new { ListingTypeId = g.Key, Count = g.Count() })
        .ToListAsync();
            var result = listingTypes
       .Select(lt => new ListingTypeFacetDto
       {
           ListingTypeId = lt.ListingTypeId,
           Name = lt.Name,
           Count = groupedCounts.FirstOrDefault(x => x.ListingTypeId == lt.ListingTypeId)?.Count ?? 0
       })
       .OrderBy(x => x.ListingTypeId)
       .ToList();
            return result;
        }

        public async Task<List<ForSalePropertyForListingViewModel>> GetAllForSalePropertyForListing()
        {
            return await BuildForSaleListingQuery()
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }

        static string? Normalize(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;

            s = s.Trim();


            if (string.Equals(s, "string", StringComparison.OrdinalIgnoreCase))
                return null;

            return s;
        }
        public async Task<List<ForSalePropertyForListingViewModel>> GetFilteredForSalePropertyForListing(PropertyFilterRequest filter)
        {

            var query = BuildForSaleListingQuery().AsNoTracking();
            filter.City = Normalize(filter.City);
            filter.District = Normalize(filter.District);
            filter.Neighborhood = Normalize(filter.Neighborhood);
            filter.NumberOfRoom = Normalize(filter.NumberOfRoom);
            filter.SortBy = Normalize(filter.SortBy);
            filter.SortDir = Normalize(filter.SortDir);

            if (filter.ListingTypeIds != null && filter.ListingTypeIds.Any())
                query = query.Where(x => filter.ListingTypeIds.Contains(x.ListingTypeId));


            if (filter.City != null)
                query = query.Where(x => x.City == filter.City);

            if (filter.District != null)
                query = query.Where(x => x.District == filter.District);

            if (filter.Neighborhood != null)
                query = query.Where(x => x.Neighborhood == filter.Neighborhood);

            if (filter.NumberOfRoom != null)
                query = query.Where(x => x.NumberOfRoom == filter.NumberOfRoom);


            if (filter.MinPrice.HasValue)
                query = query.Where(x => x.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(x => x.Price <= filter.MaxPrice.Value);

            // Sort
            var sortBy = (filter.SortBy ?? "CreatedDate").ToLowerInvariant();
            var sortDir = (filter.SortDir ?? "desc").ToLowerInvariant();

            query = (sortBy, sortDir) switch
            {
                ("price", "asc") => query.OrderBy(x => x.Price),
                ("price", "desc") => query.OrderByDescending(x => x.Price),
                ("createddate", "asc") => query.OrderBy(x => x.CreatedDate),
                _ => query.OrderByDescending(x => x.CreatedDate),
            };

            // Pagination
            var page = filter.Page < 1 ? 1 : filter.Page;
            var pageSize = filter.PageSize is < 1 or > 100 ? 20 : filter.PageSize;

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        private static List<CardHighlightViewModel> BuildHighlights(ForSalePropertyForListingViewModel x)
        {
            var t = (x.ListingType ?? "").ToLowerInvariant();

            if (t.Contains("konut"))
            {
                return new List<CardHighlightViewModel>
        {
            new() { Icon = "icon-14", Text = $"{x.NumberOfRoom ?? "-"} Oda" },
            new() { Icon = "icon-15", Text = $"{(x.NumberOfBathRoom?.ToString() ?? "-")} Banyo" },
            new() { Icon = "icon-16", Text = $"{(x.NetArea?.ToString() ?? x.GrossArea?.ToString() ?? "-")} m²" }
        };
            }

            if (t.Contains("arsa"))
            {
                return new List<CardHighlightViewModel>
        {
            new() { Icon = "icon-14", Text = $"{(x.Area?.ToString() ?? "-")} m²" },
            new() { Icon = "icon-15", Text = $"{(x.ZoningStatus ?? "-")}" },
            new() { Icon = "icon-16", Text = $"{(x.PricePerSquareMeter?.ToString("N0") ?? "-")} TL/m²" }
        };
            }

            // İşyeri
            return new List<CardHighlightViewModel>
    {
        new() { Icon = "icon-14", Text = $"{(x.Area?.ToString() ?? x.GrossArea?.ToString() ?? "-")} m²" },
        new() { Icon = "icon-15", Text = $"{(x.NumberOfFloors?.ToString() ?? "-")} Kat" },
        new() { Icon = "icon-16", Text = $"{(x.NumberOfSection?.ToString() ?? "-")} Bölüm" }
    };
        }
        public async Task<ForSaleFilterResponseResult> GetFilteredForSalePropertyForListingWithFacets(PropertyFilterRequest filter)
        {
            // 1) Base query
            var baseQuery = BuildForSaleListingQuery().AsNoTracking();

            // 2) Normal filtreler
            if (filter.ListingTypeIds != null && filter.ListingTypeIds.Any())
                baseQuery = baseQuery.Where(x =>
                    filter.ListingTypeIds.Contains(x.ListingTypeId));

            if (!string.IsNullOrWhiteSpace(filter.City))
                baseQuery = baseQuery.Where(x => x.City == filter.City);

            if (!string.IsNullOrWhiteSpace(filter.District))
                baseQuery = baseQuery.Where(x => x.District == filter.District);

            if (!string.IsNullOrWhiteSpace(filter.Neighborhood))
                baseQuery = baseQuery.Where(x => x.Neighborhood == filter.Neighborhood);

            if (!string.IsNullOrWhiteSpace(filter.NumberOfRoom))
                baseQuery = baseQuery.Where(x => x.NumberOfRoom == filter.NumberOfRoom);

            if (filter.MinPrice.HasValue)
                baseQuery = baseQuery.Where(x => x.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                baseQuery = baseQuery.Where(x => x.Price <= filter.MaxPrice.Value);

            // 3) Amenities OR filtresi (ListingTypeId enum ile)
            if (filter.SelectedAmenityIds != null && filter.SelectedAmenityIds.Any())
            {
                var selected = filter.SelectedAmenityIds;

                baseQuery = baseQuery.Where(x =>
                    (x.ListingTypeId == (int)PropertyListingType.ForSaleHousingListing &&
                        _context.ForSaleHousingListingAmenities
                            .Any(ha => ha.ForSaleHousingListId == x.ListingId && selected.Contains(ha.AmenityId)))

                    ||

                    (x.ListingTypeId == (int)PropertyListingType.ForSaleLandListing &&
                        _context.ForSaleLandListingAmenities
                            .Any(la => la.ForSaleLandListId == x.ListingId && selected.Contains(la.AmenityId)))

                    ||

                    (x.ListingTypeId == (int)PropertyListingType.ForSaleCommercialPropertyListing &&
                        _context.ForSaleCommercialListingAmenities
                            .Any(ca => ca.ForSaleCommercialListingId == x.ListingId && selected.Contains(ca.AmenityId)))
                );
            }

            // 4) Total (pagination öncesi)
            var total = await baseQuery.CountAsync();

            // 5) Facet için ID’ler (pagination öncesi!)
            var filteredIds = baseQuery.Select(x => new { x.ListingId, x.SourceType });

            var housingIds = filteredIds.Where(x => x.SourceType == 1).Select(x => x.ListingId);
            var landIds = filteredIds.Where(x => x.SourceType == 2).Select(x => x.ListingId);
            var commIds = filteredIds.Where(x => x.SourceType == 3).Select(x => x.ListingId);

            var housingFacet = _context.ForSaleHousingListingAmenities
            .Where(ha => housingIds.Contains(ha.ForSaleHousingListId))
            .GroupBy(ha => ha.AmenityId)
            .Select(g => new { AmenityId = g.Key, Count = g.Count() });

            var landFacet = _context.ForSaleLandListingAmenities
                .Where(la => landIds.Contains(la.ForSaleLandListId))
                .GroupBy(la => la.AmenityId)
                .Select(g => new { AmenityId = g.Key, Count = g.Count() });

            var commFacet = _context.ForSaleCommercialListingAmenities
                .Where(ca => commIds.Contains(ca.ForSaleCommercialListingId))
                .GroupBy(ca => ca.AmenityId)
                .Select(g => new { AmenityId = g.Key, Count = g.Count() });

            var mergedFacet = housingFacet
                .Concat(landFacet)
                .Concat(commFacet)
                .GroupBy(x => x.AmenityId)
                .Select(g => new { AmenityId = g.Key, Count = g.Sum(x => x.Count) });

            var selectedSet = (filter.SelectedAmenityIds ?? new List<int>()).ToHashSet();

            var defs = await _context.AmenityDefinitions
    .AsNoTracking()
    .Where(d => d.isActive)
    .Select(d => new
    {
        d.AmenityId,
        d.Name,
        d.GroupName,
        d.SortOrder
    })
    .ToListAsync();

            var facetRows = await mergedFacet.ToListAsync();
            var facetDict = facetRows.ToDictionary(x => x.AmenityId, x => x.Count);
            var amenities = defs
                .Select(d =>
                {
                    var count = facetDict.TryGetValue(d.AmenityId, out var c) ? c : 0;

                    return new AmenityFacetResult
                    {
                        Id = d.AmenityId,
                        Text = d.Name,
                        Group = d.GroupName,
                        SortOrder = d.SortOrder,
                        Count = count,
                        Selected = selectedSet.Contains(d.AmenityId),
                        Disabled = count == 0
                    };
                })
                .OrderBy(x => x.Group)
                .ThenBy(x => x.SortOrder)
                .ThenBy(x => x.Text)
                .ToList();


            // 8) Sorting + Pagination (Items)
            var query = baseQuery; // baseQuery aynı filtre seti

            var sortBy = (filter.SortBy ?? "CreatedDate").ToLowerInvariant();
            var sortDir = (filter.SortDir ?? "desc").ToLowerInvariant();

            query = (sortBy, sortDir) switch
            {
                ("price", "asc") => query.OrderBy(x => x.Price),
                ("price", "desc") => query.OrderByDescending(x => x.Price),
                ("createddate", "asc") => query.OrderBy(x => x.CreatedDate),
                _ => query.OrderByDescending(x => x.CreatedDate),
            };

            var page = filter.Page < 1 ? 1 : filter.Page;
            var pageSize = filter.PageSize is < 1 or > 100 ? 20 : filter.PageSize;

            var itemsVm = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            foreach (var item in itemsVm)
            {
                item.Highlights = BuildHighlights(item);
            }

            // 9) Response (Items mapping Handler’da)
            return new ForSaleFilterResponseResult
            {
                Total = total,
                Items = itemsVm,
                Amenties = amenities
            };
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
.Where(x => (x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus)) || x.PropertyStatus == null).Select(x => x.ListingTypeId);

            var landTypeIds = _context.rentalLandListings.AsNoTracking()
.Where(x => (x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus)) || x.PropertyStatus == null).Select(x => x.ListingTypeId);

            var commTypeIds = _context.rentalCommercialPropertyListings.AsNoTracking()
.Where(x => (x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus)) || x.PropertyStatus == null).Select(x => x.ListingTypeId);

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
        private static List<string> CollectImages(params string?[] urls)
        {
            return urls.Where(u => !string.IsNullOrWhiteSpace(u)).ToList()!;
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
                    ListingTypeId = h.ListingTypeId ?? 0,
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

            if (!string.IsNullOrWhiteSpace(filter.City))
                query = query.Where(x => x.City == filter.City);

            if (!string.IsNullOrWhiteSpace(filter.District))
                query = query.Where(x => x.District == filter.District);

            if (!string.IsNullOrWhiteSpace(filter.Neighborhood))
                query = query.Where(x => x.Neighborhood == filter.Neighborhood);

            if (filter.MinPrice.HasValue)
                query = query.Where(x => x.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(x => x.Price <= filter.MaxPrice.Value);

            if (!string.IsNullOrWhiteSpace(filter.NumberOfRoom))
                query = query.Where(x => x.NumberOfRoom == filter.NumberOfRoom);

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
                    ListingTypeId = l.ListingTypeId ?? 0,
                    UsageTypeId = (int)lt.UsageType,
                    ListingType = l.ListingType.Name,

                    PropertyNo = l.PropertyNo,
                    PropertyName = l.PropertyName,
                    PropertyDescription = l.PropertyDescription,
                    PropertyStatus = l.PropertyStatus,
                    CreatedDate = l.CreatedDate,

                    Price = l.Rent,
                    BestDeals = l.BestDeals,

                    City = l.City,
                    District = l.District,
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

            if (!string.IsNullOrWhiteSpace(filter.City))
                query = query.Where(x => x.City == filter.City);

            if (!string.IsNullOrWhiteSpace(filter.District))
                query = query.Where(x => x.District == filter.District);

            if (!string.IsNullOrWhiteSpace(filter.Neighborhood))
                query = query.Where(x => x.Neighborhood == filter.Neighborhood);

            if (filter.MinPrice.HasValue)
                query = query.Where(x => x.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(x => x.Price <= filter.MaxPrice.Value);

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
                    ListingTypeId = c.ListingTypeId ?? 0,
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

            if (!string.IsNullOrWhiteSpace(filter.City))
                query = query.Where(x => x.City == filter.City);

            if (!string.IsNullOrWhiteSpace(filter.District))
                query = query.Where(x => x.District == filter.District);

            if (!string.IsNullOrWhiteSpace(filter.Neighborhood))
                query = query.Where(x => x.Neighborhood == filter.Neighborhood);

            if (filter.MinPrice.HasValue)
                query = query.Where(x => x.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(x => x.Price <= filter.MaxPrice.Value);

            return query;
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

            var housingList = await baseHousingQuery.AsNoTracking().ToListAsync();
            var landList = await baseLandQuery.AsNoTracking().ToListAsync();
            var commercialList = await baseCommercialQuery.AsNoTracking().ToListAsync();


            var all = new List<RentalListingBaseResult>();
            all.AddRange(housingList);
            all.AddRange(landList);
            all.AddRange(commercialList);


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

            var total = all.Count;

            // Sort 
            var sortBy = (filter.SortBy ?? "CreatedDate").ToLowerInvariant();
            var sortDir = (filter.SortDir ?? "desc").ToLowerInvariant();

            IEnumerable<RentalListingBaseResult> sorted = (sortBy, sortDir) switch
            {
                ("price", "asc") => all.OrderBy(x => x.Price),
                ("price", "desc") => all.OrderByDescending(x => x.Price),
                ("createddate", "asc") => all.OrderBy(x => x.CreatedDate),
                _ => all.OrderByDescending(x => x.CreatedDate),
            };

            // Pagination 
            var page = filter.Page < 1 ? 1 : filter.Page;
            var pageSize = filter.PageSize is < 1 or > 100 ? 20 : filter.PageSize;

            var items = sorted
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            foreach (var item in items)
                item.Highlights = BuildHighlightsForRental(item);
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

        private static List<string> BuildHighlightsForRental(RentalListingBaseResult item)
        {
            var list = new List<string>();

            if (item.Price.HasValue) list.Add($"{item.Price.Value:n0} ₺");

            if (item is RentalHousingListingResult h)
            {
                if (!string.IsNullOrWhiteSpace(h.NumberOfRoom)) list.Add(h.NumberOfRoom);
                if (h.GrossArea.HasValue) list.Add($"{h.GrossArea.Value:n0} m²");
            }
            else if (item is RentalLandListingResult l)
            {
                if (l.Area.HasValue) list.Add($"{l.Area.Value:n0} m²");
            }
            else if (item is RentalCommercialListingResult c)
            {
                if (c.GrossArea.HasValue) list.Add($"{c.GrossArea.Value:n0} m²");
            }

            return list;
        }

        public async Task<List<ListingSelectedImageRowDto>> GetSelectedImagesAsync(int listingId, string sectionKey)
        {
            return await _context.ListingImageSelections
        .Where(x => x.ListingId == listingId && x.SectionKey == sectionKey)
        .OrderBy(x => x.SortOrder)
        .Select(x => new ListingSelectedImageRowDto
        {
            ImageNo = x.ImageNo,
            SlotKey = x.SlotKey,
            SortOrder = x.SortOrder
        })
        .ToListAsync();
        }

        public async Task<ForRentalPropertForListingViewModel?> GetUnifiedForRentalPropertyById(int id)
        {
            List<AmenityItemViewModel> amenities = new();

            var housingById = await _context.rentalHousingListings
                .Include(x => x.Agent)
                .Include(x => x.ListingType)
                .Include(x => x.HousingCategory)
                .FirstOrDefaultAsync(x => x.RentalHousingListId == id);

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
            var land = await _context.rentalLandListings.Include(x => x.Agent).Include(x => x.ListingType).Include(x => x.LandCategory).FirstOrDefaultAsync(x => x.RentalLandListingId == id);
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
            var commercialById = await _context.rentalCommercialPropertyListings.Include(x => x.Agent).Include(x => x.ListingType).FirstOrDefaultAsync(x => x.RentalCommercialListId == id);
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


