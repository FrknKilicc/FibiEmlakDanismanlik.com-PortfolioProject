using FibiEmlakDanismanlik.Application.Features.Queries.ForSalePropertyQueries;
using FibiEmlakDanismanlik.Application.Features.Results.ForSalePropertyResults;
using FibiEmlakDanismanlik.Application.Interfaces.PropertyInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ForSalePropertyHandlers
{
    public class GetAllForSalePropertiesForListingQueryHandler : IRequestHandler<GetAllForSalePropertiesForListingQuery, List<GetAllForSalePropertiesForListingResult>>
    {
        private readonly IPropertyRepository _propertyRepository;

        public GetAllForSalePropertiesForListingQueryHandler(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }

        public async Task<List<GetAllForSalePropertiesForListingResult>> Handle(GetAllForSalePropertiesForListingQuery request, CancellationToken cancellationToken)
        {
            var value = await _propertyRepository.GetAllForSalePropertyForListing();
            return value.Select(x => new GetAllForSalePropertiesForListingResult
            {
                PropertyDescription=x.PropertyDescription,
                PropertyName=x.PropertyName,
                PropertyNo=x.PropertyNo,
                PropertyStatus=x.PropertyStatus,
                Price=x.Price,
                AgentId=x.AgentId,
                AgentTitle=x.AgentTitle,
                AddressDesc=x.AddressDesc,
                AgentImgUrl=x.AgentImgUrl,
                AgentName=x.AgentName,
                Area = x.Area,
                BuildingAge=x.BuildingAge,
                SharePercentage=x.SharePercentage,
                UsageStatus=x.UsageStatus,
                TitleDeedStatus=x.TitleDeedStatus,
                TotalNumberOfFloor = x.TotalNumberOfFloor,
                Transferable=x.Transferable,
                WithinTheComplex=x.WithinTheComplex,
                MapSheetNumber=x.MapSheetNumber,
                NumberOfSection=x.NumberOfSection,
                District=x.District,
                DevelopmentRight=x.DevelopmentRight,
                BestDeals=x.BestDeals,
                BaseAreaRatio=x.BaseAreaRatio,
                NumberOfBalconies=x.NumberOfBalconies,
                Neighborhood=x.Neighborhood,
                City=x.City,
                CreatedDate=x.CreatedDate,
                LandCategoryId=x.LandCategoryId,
                BlackBox=x.BlackBox,
                Dues=x.Dues,
                Exchange=x.Exchange,
                Facade=x.Facade,
                FloorAreaRatio=x.FloorAreaRatio,
                Furnished=x.Furnished,
                GrossArea=x.GrossArea,
                Heating=x.Heating,
                HomeLoan=x.HomeLoan,
                IsElevator=x.IsElevator,
                LandLoan=x.LandLoan,
                NumberOfBathrooms=x.NumberOfBathrooms,
                NetArea=x.NetArea,
                NumberOfFloors=x.NumberOfFloors,
                NumberOfBathRoom=x.NumberOfBathRoom,
                NumberOfKitchens=x.NumberOfKitchens,
                NumberOfRoom=x.NumberOfRoom,
                ParcelNumber=x.ParcelNumber,
                OpenArea=x.OpenArea,
                PricePerSquareMeter=x.PricePerSquareMeter,
                ListingType=x.ListingType,
                ParkingLot=x.ParkingLot,
                PlotNumber=x.PlotNumber,
                ListingId=x.ListingId,
                ZoningPlan=x.ZoningPlan,
                ZoningStatus=x.ZoningStatus,
                PropImgUrl1 =  x.PropImgUrl1,
                PropImgUrl10 = x.PropImgUrl10,
                PropImgUrl11 = x.PropImgUrl11,
                PropImgUrl12 = x.PropImgUrl12,
                PropImgUrl13 = x.PropImgUrl13,
                PropImgUrl14 = x.PropImgUrl14,
                PropImgUrl15 = x.PropImgUrl15,
                PropImgUrl16 = x.PropImgUrl16,
                PropImgUrl17 = x.PropImgUrl17,
                PropImgUrl18 = x.PropImgUrl18,
                PropImgUrl19 = x.PropImgUrl19,
                PropImgUrl2 =  x.PropImgUrl2,
                PropImgUrl20 = x.PropImgUrl20,
                PropImgUrl21 = x.PropImgUrl21,
                PropImgUrl22 = x.PropImgUrl22,
                PropImgUrl23 = x.PropImgUrl23,
                PropImgUrl24 = x.PropImgUrl24,
                PropImgUrl25 = x.PropImgUrl25,
                PropImgUrl26 = x.PropImgUrl26,
                PropImgUrl27 = x.PropImgUrl27,
                PropImgUrl28 = x.PropImgUrl28,
                PropImgUrl29 = x.PropImgUrl29,
                PropImgUrl3 =  x.PropImgUrl3,
                PropImgUrl30 = x.PropImgUrl30,
                PropImgUrl4 =  x.PropImgUrl4,
                PropImgUrl5 =  x.PropImgUrl5,
                PropImgUrl6 =  x.PropImgUrl6,
                PropImgUrl7 =  x.PropImgUrl7,
                PropImgUrl8 =  x.PropImgUrl8,
                PropImgUrl9 =  x.PropImgUrl9,
                
            }).ToList();
        }
    }
}
