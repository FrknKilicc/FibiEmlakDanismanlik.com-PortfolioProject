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
        CityId = house.CityId,
        DistrictId = house.DistrictId,
        NeighborhoodId = house.NeighborhoodId,
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
        HousingCategoryId = house.HousingCategoryId,


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
        CityId = comm.CityId,
        DistrictId = comm.DistrictId,
        NeighborhoodId = comm.NeighborhoodId,
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
        HousingCategoryId = null,


        // konut boş
        Facade = null,
        IsElevator = null,
        GrossArea = comm.GrossArea,
        NetArea = null,
        OpenArea = null,
        BuildingAge = null,
        TotalNumberOfFloor = null,
        NumberOfFloors = comm.NumberOfFloors,
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
                    CityId = land.CityId,
                    DistrictId = land.DistrictId,
                    NeighborhoodId = land.NeighborhoodId,
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
                    HousingCategoryId = null,
                    

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

        public async Task<ForSalePropertyForListingViewModel> GetUnifiedForSalePropertyById(int id, int? listingTypeId = null)
        {
            List<AmenityItemViewModel> amenities = new();

            bool checkHousing = !listingTypeId.HasValue || listingTypeId == (int)PropertyListingType.ForSaleHousingListing;
            var housingById = checkHousing
                ? await _context.forSaleHousingPropertyListings.Include(x => x.Agent).Include(x => x.ListingType).FirstOrDefaultAsync(x => x.ForSaleHousingListId == id)
                : null;

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
                    NumberOfRoom = housingById.NumberOfRoom,
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
            bool checkLand = !listingTypeId.HasValue || listingTypeId == (int)PropertyListingType.ForSaleLandListing;
            var land = checkLand
                ? await _context.forSaleLandListings.Include(x => x.Agent).Include(x => x.ListingType).Include(x => x.LandCategory).FirstOrDefaultAsync(x => x.ForSaleLandListingId == id)
                : null;
            

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
            bool checkCommercial = !listingTypeId.HasValue || listingTypeId == (int)PropertyListingType.ForSaleCommercialPropertyListing;
            var commercialById = checkCommercial
                ? await _context.forSaleCommercialPropertyListings.Include(x => x.Agent).Include(x => x.ListingType).FirstOrDefaultAsync(x => x.ForSaleCommercialListingId == id)
                : null;
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

        public async Task<List<ListingTypeFacetDto>> GetForSaleListingTypeFacetsAsync()
        {
            const int ForSaleUsageType = 1;
            var activeStatuses = PropertyStatuses.ActiveSet;
            var listingTypes = await _context.listingTypes.AsNoTracking().Where(x => x.UsageType == UsageType.ForSale).Select(x => new { x.ListingTypeId, x.Name }).ToListAsync();
            var housingTypeIds = _context.forSaleHousingPropertyListings
     .AsNoTracking()
     .Where(x => x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus))
     .Select(x => x.ListingTypeId);

            var landTypeIds = _context.forSaleLandListings
        .AsNoTracking()
        .Where(x => x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus))
        .Select(x => x.ListingTypeId);
            var commercialTypeIds = _context.forSaleCommercialPropertyListings
        .AsNoTracking()
        .Where(x => x.PropertyStatus != null && activeStatuses.Contains(x.PropertyStatus))
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


            if (filter.CityId.HasValue)
                query = query.Where(x => x.CityId == filter.CityId);
            else if (filter.City != null)
                query = query.Where(x => x.City == filter.City);

            if (filter.DistrictId.HasValue)
                query = query.Where(x => x.DistrictId == filter.DistrictId);
            else if (filter.District != null)
                query = query.Where(x => x.District == filter.District);

            if (filter.NeighborhoodId.HasValue)
                query = query.Where(x => x.NeighborhoodId == filter.NeighborhoodId);
            else if (filter.Neighborhood != null)
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
                return ListingCardHighlightBuilder.Housing(
                    x.NumberOfRoom,
                    x.NetArea,
                    x.GrossArea,
                    x.NumberOfFloors,
                    x.NumberOfBathRoom ?? x.NumberOfBathrooms,
                    x.BuildingAge);
            }

            if (t.Contains("arsa"))
            {
                return ListingCardHighlightBuilder.Land(
                    x.Area,
                    x.ZoningStatus,
                    x.PricePerSquareMeter,
                    x.TitleDeedStatus);
            }

            // İşyeri
            return ListingCardHighlightBuilder.Commercial(
                x.NetArea,
                x.GrossArea,
                x.Area,
                x.NumberOfSection,
                x.NumberOfFloors,
                x.BuildingAge);
        }
        public async Task<ForSaleFilterResponseResult> GetFilteredForSalePropertyForListingWithFacets(PropertyFilterRequest filter)
        {
            // 1) Base query
            var baseQuery = BuildForSaleListingQuery().AsNoTracking();

            // 2) Normal filtreler
            if (filter.ListingTypeIds != null && filter.ListingTypeIds.Any())
                baseQuery = baseQuery.Where(x =>
                    filter.ListingTypeIds.Contains(x.ListingTypeId));

            if (filter.CityId.HasValue)
                baseQuery = baseQuery.Where(x => x.CityId == filter.CityId);
            else if (!string.IsNullOrWhiteSpace(filter.City))
                baseQuery = baseQuery.Where(x => x.City == filter.City);

            if (filter.DistrictId.HasValue)
                baseQuery = baseQuery.Where(x => x.DistrictId == filter.DistrictId);
            else if (!string.IsNullOrWhiteSpace(filter.District))
                baseQuery = baseQuery.Where(x => x.District == filter.District);

            if (filter.NeighborhoodId.HasValue)
                baseQuery = baseQuery.Where(x => x.NeighborhoodId == filter.NeighborhoodId);
            else if (!string.IsNullOrWhiteSpace(filter.Neighborhood))
                baseQuery = baseQuery.Where(x => x.Neighborhood == filter.Neighborhood);

            if (!string.IsNullOrWhiteSpace(filter.NumberOfRoom))
                baseQuery = baseQuery.Where(x => x.NumberOfRoom == filter.NumberOfRoom);

            if (filter.MinPrice.HasValue)
                baseQuery = baseQuery.Where(x => x.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                baseQuery = baseQuery.Where(x => x.Price <= filter.MaxPrice.Value);

            if (filter.MinArea.HasValue)
                baseQuery = baseQuery.Where(x => (x.Area ?? x.GrossArea) >= filter.MinArea.Value);

            if (filter.MaxArea.HasValue)
                baseQuery = baseQuery.Where(x => (x.Area ?? x.GrossArea) <= filter.MaxArea.Value);

            if (filter.NumberOfBathRoom.HasValue)
                baseQuery = baseQuery.Where(x => (x.NumberOfBathRoom ?? x.NumberOfBathrooms) == filter.NumberOfBathRoom.Value);

            if (filter.Floor.HasValue)
                baseQuery = baseQuery.Where(x => x.NumberOfFloors == filter.Floor.Value);

            if (filter.NumberOfSection.HasValue)
                baseQuery = baseQuery.Where(x => x.NumberOfSection == filter.NumberOfSection.Value);

            if (filter.AgentId.HasValue)
                baseQuery = baseQuery.Where(x => x.AgentId == filter.AgentId.Value);

            if (!string.IsNullOrWhiteSpace(filter.ZoningStatus))
                baseQuery = baseQuery.Where(x => x.ZoningStatus == filter.ZoningStatus);

            if (filter.HousingCategoryId.HasValue)
                baseQuery = baseQuery.Where(x => x.HousingCategoryId == filter.HousingCategoryId.Value);
            if (filter.LandCategoryId.HasValue)
                baseQuery = baseQuery.Where(x => x.LandCategoryId == filter.LandCategoryId.Value);
            if (!string.IsNullOrWhiteSpace(filter.BuildingAge))
                baseQuery = baseQuery.Where(x => x.BuildingAge == filter.BuildingAge);
            if (!string.IsNullOrWhiteSpace(filter.Heating))
                baseQuery = baseQuery.Where(x => x.Heating == filter.Heating);
            if (!string.IsNullOrWhiteSpace(filter.TitleDeedStatus))
                baseQuery = baseQuery.Where(x => x.TitleDeedStatus == filter.TitleDeedStatus);
            if (filter.Furnished.HasValue)
                baseQuery = baseQuery.Where(x => (x.Furnished ?? false) == filter.Furnished.Value);
            if (filter.WithinTheComplex.HasValue)
                baseQuery = baseQuery.Where(x => (x.WithinTheComplex ?? false) == filter.WithinTheComplex.Value);
            if (filter.LoanEligible.HasValue)
                baseQuery = baseQuery.Where(x => ((x.HomeLoan ?? false) || (x.LandLoan ?? false)) == filter.LoanEligible.Value);
            if (filter.Transferable.HasValue)
                baseQuery = baseQuery.Where(x => (x.Transferable ?? false) == filter.Transferable.Value);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.Trim();
                var dashIdx = term.IndexOf(" - ", StringComparison.Ordinal);
                var namePart = dashIdx >= 0 ? term.Substring(dashIdx + 3).Trim() : term;
                baseQuery = baseQuery.Where(x =>
                    (x.PropertyName != null && (x.PropertyName.Contains(term) || (namePart.Length > 0 && x.PropertyName.Contains(namePart))))
                    || (x.PropertyNo != null && x.PropertyNo.ToString() == term)
                    || (x.City != null && x.City.Contains(term))
                    || (x.District != null && x.District.Contains(term))
                    || (x.Neighborhood != null && x.Neighborhood.Contains(term)));
            }

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
    }
}
