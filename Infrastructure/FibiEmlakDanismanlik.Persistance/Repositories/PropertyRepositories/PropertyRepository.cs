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
            var housingList = from housing in _context.forSaleHousingPropertyListings
                              join agent in _context.Agents on housing.AgentId equals agent.AgentId
                              select new ForSalePropertyForListingViewModel
                              {
                                  ListingId = housing.ForSaleHousingListId,
                                  PropertyNo = housing.PropertyNo,
                                  PropertyName = housing.PropertyName,
                                  PropertyDescription = housing.PropertyDescription,
                                  PropertyStatus = housing.PropertyStatus,
                                  CreatedDate = housing.CreatedDate,
                                  Price = housing.Price,
                                  TitleDeedStatus = housing.TitleDeedStatus,
                                  City = housing.City,
                                  District = housing.District,
                                  Neighborhood = housing.Neighborhood,
                                  AddressDesc = housing.AddressDesc,
                                  Facade = housing.Facade,
                                  IsElevator = housing.IsElevator,
                                  GrossArea = housing.GrossArea,
                                  NetArea = housing.NetArea,
                                  OpenArea = housing.OpenArea,
                                  BuildingAge = housing.BuildingAge,
                                  TotalNumberOfFloor = housing.TotalNumberOfFloor,
                                  NumberOfFloors = housing.NumberOfFloors,
                                  NumberOfBathRoom = housing.NumberOfBathRoom,
                                  NumberOfRoom = housing.NumberOfRoom,
                                  Heating = housing.Heating,
                                  BlackBox = housing.BlackBox,
                                  NumberOfBalconies = housing.NumberOfBalconies,
                                  ParkingLot = housing.ParkingLot,
                                  Furnished = housing.Furnished,
                                  UsageStatus = housing.UsageStatus,
                                  WithinTheComplex = housing.WithinTheComplex,
                                  Dues = housing.Dues,
                                  HomeLoan = housing.HomeLoan,
                                  Exchange = housing.Exchange,
                                  AgentId = housing.AgentId,
                                  Agent = housing.Agent,
                                  AgentName = agent.AgentName,
                                  AgentImgUrl = agent.AgentImgUrl,
                                  AgentTitle = agent.AgentTitle,
                                  PropImgUrl1 = housing.PropImgUrl1,
                                  PropImgUrl10 = housing.PropImgUrl10,
                                  PropImgUrl11 = housing.PropImgUrl11,
                                  PropImgUrl12 = housing.PropImgUrl12,
                                  PropImgUrl13 = housing.PropImgUrl13,
                                  PropImgUrl14 = housing.PropImgUrl14,
                                  PropImgUrl15 = housing.PropImgUrl15,
                                  PropImgUrl16 = housing.PropImgUrl16,
                                  PropImgUrl17 = housing.PropImgUrl17,
                                  PropImgUrl18 = housing.PropImgUrl18,
                                  PropImgUrl19 = housing.PropImgUrl19,
                                  PropImgUrl2 = housing.PropImgUrl2,
                                  PropImgUrl20 = housing.PropImgUrl20,
                                  PropImgUrl21 = housing.PropImgUrl21,
                                  PropImgUrl22 = housing.PropImgUrl22,
                                  PropImgUrl23 = housing.PropImgUrl23,
                                  PropImgUrl24 = housing.PropImgUrl24,
                                  PropImgUrl25 = housing.PropImgUrl25,
                                  PropImgUrl26 = housing.PropImgUrl26,
                                  PropImgUrl27 = housing.PropImgUrl27,
                                  PropImgUrl28 = housing.PropImgUrl28,
                                  PropImgUrl29 = housing.PropImgUrl29,
                                  PropImgUrl3 = housing.PropImgUrl3,
                                  PropImgUrl30 = housing.PropImgUrl30,
                                  PropImgUrl4 = housing.PropImgUrl4,
                                  PropImgUrl5 = housing.PropImgUrl5,
                                  PropImgUrl6 = housing.PropImgUrl6,
                                  PropImgUrl7 = housing.PropImgUrl7,
                                  PropImgUrl8 = housing.PropImgUrl8,
                                  PropImgUrl9 = housing.PropImgUrl9,
                                  ListingType = "Arsa"

                              };
            var commercialList = from commercial in _context.forSaleCommercialPropertyListings
                                 join agent in _context.Agents on commercial.AgentId equals agent.AgentId
                                 select new ForSalePropertyForListingViewModel
                                 {
                                     PropertyNo = commercial.PropertyNo,
                                     PropertyName = commercial.PropertyName,
                                     PropertyDescription = commercial.PropertyDescription,
                                     PropertyStatus = commercial.PropertyStatus,
                                     CreatedDate = commercial.CreatedDate,
                                     Price = commercial.Price,
                                     TitleDeedStatus = commercial.TitleDeedStatus,
                                     District = commercial.District,
                                     City = commercial.City,
                                     Neighborhood = commercial.Neighborhood,
                                     AddressDesc = commercial.AddressDesc,
                                     Facade = commercial.Facade,
                                     GrossArea = commercial.GrossArea,
                                     Area = commercial.Area,
                                     NumberOfSection = commercial.NumberOfSection,
                                     NumberOfKitchens = commercial.NumberOfKitchens,
                                     NumberOfBathrooms = commercial.NumberOfBathrooms,
                                     NumberOfFloors = commercial.NumberOfFloors,
                                     SharePercentage = commercial.SharePercentage,
                                     LandLoan = commercial.LandLoan,
                                     Exchange = commercial.Exchange,
                                     Transferable = commercial.Transferable,
                                     BestDeals = commercial.BestDeals,
                                     AgentId = commercial.AgentId,
                                     Agent = agent,
                                     AgentName = agent.AgentName,
                                     AgentImgUrl = agent.AgentImgUrl,
                                     AgentTitle = agent.AgentTitle,
                                     PropImgUrl1 = commercial.PropImgUrl1,
                                     PropImgUrl10 = commercial.PropImgUrl10,
                                     PropImgUrl11 = commercial.PropImgUrl11,
                                     PropImgUrl12 = commercial.PropImgUrl12,
                                     PropImgUrl13 = commercial.PropImgUrl13,
                                     PropImgUrl14 = commercial.PropImgUrl14,
                                     PropImgUrl15 = commercial.PropImgUrl15,
                                     PropImgUrl16 = commercial.PropImgUrl16,
                                     PropImgUrl17 = commercial.PropImgUrl17,
                                     PropImgUrl18 = commercial.PropImgUrl18,
                                     PropImgUrl19 = commercial.PropImgUrl19,
                                     PropImgUrl2 = commercial.PropImgUrl2,
                                     PropImgUrl20 = commercial.PropImgUrl20,
                                     PropImgUrl21 = commercial.PropImgUrl21,
                                     PropImgUrl22 = commercial.PropImgUrl22,
                                     PropImgUrl23 = commercial.PropImgUrl23,
                                     PropImgUrl24 = commercial.PropImgUrl24,
                                     PropImgUrl25 = commercial.PropImgUrl25,
                                     PropImgUrl26 = commercial.PropImgUrl26,
                                     PropImgUrl27 = commercial.PropImgUrl27,
                                     PropImgUrl28 = commercial.PropImgUrl28,
                                     PropImgUrl29 = commercial.PropImgUrl29,
                                     PropImgUrl3 = commercial.PropImgUrl3,
                                     PropImgUrl30 = commercial.PropImgUrl30,
                                     PropImgUrl4 = commercial.PropImgUrl4,
                                     PropImgUrl5 = commercial.PropImgUrl5,
                                     PropImgUrl6 = commercial.PropImgUrl6,
                                     PropImgUrl7 = commercial.PropImgUrl7,
                                     PropImgUrl8 = commercial.PropImgUrl8,
                                     PropImgUrl9 = commercial.PropImgUrl9,
                                     ListingType = "İş Yeri"


                                 };
            var LandList = from land in _context.forSaleLandListings
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
                               AgentId = land.AgentId,
                               Agent = agent,
                               AgentName = agent.AgentName,
                               AgentImgUrl = agent.AgentImgUrl,
                               AgentTitle = agent.AgentTitle,
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
                               ListingType = "Arsa"

                           };
            var listing= housingList.Concat(LandList).Concat(commercialList).OrderByDescending(x=>x.CreatedDate).ToList();
            return listing;
                        
        }
    }
}
