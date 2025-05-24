using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.PropertyInterfaces;
using FibiEmlakDanismanlik.Application.ViewModels;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Persistence.Context;
using Microsoft.EntityFrameworkCore;
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

        /*
 
        */

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

                                     ListingType = "Ticari",
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
