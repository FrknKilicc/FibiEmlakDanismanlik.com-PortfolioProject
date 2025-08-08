using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.PropertyInterfaces;
using FibiEmlakDanismanlik.Application.ViewModels;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Persistence.Repositories.PropertyRepositories
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly FibiEmlakDanismanlikContext _context;

        
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
            var housingById = await _context.forSaleHousingPropertyListings.Include(x => x.Agent).Include(x => x.ListingType).FirstOrDefaultAsync(x => x.ForSaleHousingListId == id);
            if (housingById != null)
            {
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
            var land = await _context.forSaleLandListings.Include(x => x.Agent).Include(x => x.ListingType).Include(x=>x.LandCategory).FirstOrDefaultAsync(x => x.ForSaleLandListingId == id);

            if (land != null)
            {
                return new ForSalePropertyForListingViewModel
                {
                    ListingId = land.ForSaleLandListingId,
                    PropertyNo= land.PropertyNo,
                    PropertyName= land.PropertyName,
                    PropertyDescription= land.PropertyDescription,
                    PropertyStatus= land.PropertyStatus,
                    CreatedDate= land.CreatedDate,
                    City= land.City,
                    District= land.District,
                    Neighborhood= land.Neighborhood,
                    AddressDesc= land.AddressDesc,
                    ZoningStatus= land.ZoningStatus,
                    SharePercentage= land.SharePercentage,
                    Area= land.Area,
                    Price= land.Price,
                    PricePerSquareMeter= land.PricePerSquareMeter,
                    ParcelNumber= land.ParcelNumber,
                    PlotNumber= land.PlotNumber,
                    MapSheetNumber= land.MapSheetNumber,
                    FloorAreaRatio= land.FloorAreaRatio,
                    BaseAreaRatio= land.BaseAreaRatio,
                    ZoningPlan= land.ZoningPlan,
                    TitleDeedStatus= land.TitleDeedStatus,
                    DevelopmentRight= land.DevelopmentRight,
                    LandLoan    = land.LandLoan,
                    Exchange= land.Exchange,
                    LandCategoryId= land.LandCategoryId,
                    AgentId= land.AgentId,
                    AgentImgUrl=land.Agent.AgentImgUrl,
                    AgentName=land.Agent.AgentName,
                    AgentTitle=land.Agent.AgentTitle,
                    BestDeals= land.BestDeals,
                    ListingType="Arsa",

                      //Images 
                    PropImgUrl1 =  land.PropImgUrl1,
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
                    PropImgUrl2 =  land.PropImgUrl2,
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
                    PropImgUrl3 =  land.PropImgUrl3,
                    PropImgUrl30 = land.PropImgUrl30,
                    PropImgUrl4 =  land.PropImgUrl4,
                    PropImgUrl5 =  land.PropImgUrl5,
                    PropImgUrl6 =  land.PropImgUrl6,
                    PropImgUrl7 =  land.PropImgUrl7,
                    PropImgUrl8 =  land.PropImgUrl8,
                    PropImgUrl9 =  land.PropImgUrl9,
                    //Images


                };
            }
            var commercialById = await _context.forSaleCommercialPropertyListings.Include(x => x.Agent).Include(x => x.ListingType).FirstOrDefaultAsync(x => x.ForSaleCommercialListingId == id);
            if (commercialById!=null)
            {
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
                                           PropertyStatus=rentalCommercial.PropertyStatus,
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

        List<ForSalePropertyForListingViewModel> IPropertyRepository.GetAllForSalePropertyForListing()
        {
            var housingList = from house in _context.forSaleHousingPropertyListings
                              join agent in _context.Agents on house.AgentId equals agent.AgentId
                              select new ForSalePropertyForListingViewModel
                              {
                                  ListingId = house.ForSaleHousingListId,
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

                                  // Arsa alanları boş
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
                                  BestDeals = null,
                                  LandCategoryId = null,

                                  // ev için olan  alanlar dolu
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

                                  // ev için alanlar boş ancak ileride genişleyebilir 
                                  NumberOfSection = null,
                                  NumberOfKitchens = null,
                                  NumberOfBathrooms = null,
                                  Transferable = null,

                                  // çalışan  bilgileri
                                  AgentId = house.AgentId,
                                  Agent = agent,
                                  AgentName = agent.AgentName,
                                  AgentTitle = agent.AgentTitle,
                                  AgentImgUrl = agent.AgentImgUrl,

                                  ListingType = "Konut",

                                  PropImgUrl1 =  house.PropImgUrl1,
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
                                  PropImgUrl2 =  house.PropImgUrl2,
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
                                  PropImgUrl3 =  house.PropImgUrl3,
                                  PropImgUrl30 = house.PropImgUrl30,
                                  PropImgUrl4 = house.PropImgUrl4,
                                  PropImgUrl5 = house.PropImgUrl5,
                                  PropImgUrl6 = house.PropImgUrl6,
                                  PropImgUrl7 = house.PropImgUrl7,
                                  PropImgUrl8 = house.PropImgUrl8,
                                  PropImgUrl9 = house.PropImgUrl9,
                                 

                              };
            var commercialList = from comm in _context.forSaleCommercialPropertyListings
                                 join agent in _context.Agents on comm.AgentId equals agent.AgentId
                                 select new ForSalePropertyForListingViewModel
                                 {
                                     ListingId = comm.ForSaleCommercialListingId,
                                     PropertyNo = comm.PropertyNo,
                                     PropertyName = comm.PropertyName,
                                     PropertyDescription = comm.PropertyDescription,
                                     PropertyStatus = comm.PropertyStatus,
                                     CreatedDate = comm.CreatedDate,
                                     Price = comm.Price,
                                     TitleDeedStatus = comm.TitleDeedStatus,

                                     City = comm.City,
                                     District = comm.District,
                                     Neighborhood = comm.Neighborhood,
                                     AddressDesc = comm.AddressDesc,

                                     // Arsa alanları boş
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
                                     BestDeals = null,
                                     LandCategoryId = null,

                                     // Konut alanları boş olacak
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

                                     // işyeri modelinde dolu alanlar
                                     NumberOfSection = comm.NumberOfSection,
                                     NumberOfKitchens = comm.NumberOfKitchens,
                                     NumberOfBathrooms = comm.NumberOfBathrooms,
                                     Transferable = comm.Transferable,

                                     // çalışan bilgileri
                                     AgentId = comm.AgentId,
                                     Agent = agent,
                                     AgentName = agent.AgentName,
                                     AgentTitle = agent.AgentTitle,
                                     AgentImgUrl = agent.AgentImgUrl,

                                     ListingType = "İşyeri",
                                     PropImgUrl1 =  comm.PropImgUrl1,
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
                                     PropImgUrl2 =  comm.PropImgUrl2,
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
                                     PropImgUrl3 = comm.PropImgUrl3,
                                     PropImgUrl30 = comm.PropImgUrl30,
                                     PropImgUrl4 = comm.PropImgUrl4,
                                     PropImgUrl5 = comm.PropImgUrl5,
                                     PropImgUrl6 = comm.PropImgUrl6,
                                     PropImgUrl7 = comm.PropImgUrl7,
                                     PropImgUrl8 = comm.PropImgUrl8,
                                     PropImgUrl9 = comm.PropImgUrl9,
                                    


                                 };
            var landList = from land in _context.forSaleLandListings
                           join agent in _context.Agents on land.AgentId equals agent.AgentId
                           select new ForSalePropertyForListingViewModel
                           {
                               ListingId = land.ForSaleLandListingId,
                               PropertyNo = land.PropertyNo,
                               PropertyName = land.PropertyName,
                               PropertyDescription = land.PropertyDescription,
                               PropertyStatus = land.PropertyStatus,
                               CreatedDate = land.CreatedDate,
                               Price = land.Price,
                               TitleDeedStatus = land.TitleDeedStatus,

                               City = land.City,
                               District = land.District,
                               Neighborhood = land.Neighborhood,
                               AddressDesc = land.AddressDesc,

                               // Arsa için alanlar
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

                               // Konut propları boş olacak 
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

                               // Ticari alanlarda boşl
                               NumberOfSection = null,
                               NumberOfKitchens = null,
                               NumberOfBathrooms = null,
                               Transferable = null,

                               // çalışan bilgileri
                               AgentId = land.AgentId,
                               Agent = agent,
                               AgentName = agent.AgentName,
                               AgentTitle = agent.AgentTitle,
                               AgentImgUrl = agent.AgentImgUrl,

                               // İlan türü statikk "Arsa"
                               ListingType = "Arsa",

                               PropImgUrl1 =  land.PropImgUrl1,
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
                               PropImgUrl2 =  land.PropImgUrl2,
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
                               PropImgUrl3 =  land.PropImgUrl3,
                               PropImgUrl30 = land.PropImgUrl30,
                               PropImgUrl4 =  land.PropImgUrl4,
                               PropImgUrl5 =  land.PropImgUrl5,
                               PropImgUrl6 =  land.PropImgUrl6,
                               PropImgUrl7 =  land.PropImgUrl7,
                               PropImgUrl8 =  land.PropImgUrl8,
                               PropImgUrl9 =  land.PropImgUrl9,
                             

                           };
            var listing= housingList.Concat(landList).Concat(commercialList).OrderByDescending(x=>x.CreatedDate).ToList();
            return listing;
                        
        }

    }
}
