using FibiEmlakDanismanlik.Application.Features.Queries.ForSaleCommercialPropertyListingQueries;
using FibiEmlakDanismanlik.Application.Features.Queries.ForSalePropertyQueries;
using FibiEmlakDanismanlik.Application.Features.Results.ForSalePropertyResults;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.PropertyInterfaces;
using FibiEmlakDanismanlik.Application.ViewModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ForSalePropertyHandlers
{
    public class GetForSalesPropertyByIdQueryByIdHandler : IRequestHandler<GetForSalesPropertyByIdQueries, GetAllForSalePropertiesForListingResult>
    {
        private readonly IPropertyRepository _repository;

        public GetForSalesPropertyByIdQueryByIdHandler(IPropertyRepository repository)
        {
            _repository = repository;
        }

       
        public async Task<GetAllForSalePropertiesForListingResult> Handle(GetForSalesPropertyByIdQueries request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetUnifiedForSalePropertyById(request.Id);
            return new GetAllForSalePropertiesForListingResult
            {
                PropertyDescription = value.PropertyDescription,
                PropertyName = value.PropertyName,
                PropertyNo = value.PropertyNo,
                PropertyStatus = value.PropertyStatus,
                Price = value.Price,
                AgentId = value.AgentId,
                AgentTitle = value.AgentTitle,
                AddressDesc = value.AddressDesc,
                AgentImgUrl = value.AgentImgUrl,
                AgentName = value.AgentName,
                Area = value.Area,
                BuildingAge = value.BuildingAge,
                SharePercentage = value.SharePercentage,
                UsageStatus = value.UsageStatus,
                TitleDeedStatus = value.TitleDeedStatus,
                TotalNumberOfFloor = value.TotalNumberOfFloor,
                Transferable = value.Transferable,
                WithinTheComplex = value.WithinTheComplex,
                MapSheetNumber = value.MapSheetNumber,
                NumberOfSection = value.NumberOfSection,
                District = value.District,
                DevelopmentRight = value.DevelopmentRight,
                BestDeals = value.BestDeals,
                BaseAreaRatio = value.BaseAreaRatio,
                NumberOfBalconies = value.NumberOfBalconies,
                Neighborhood = value.Neighborhood,
                City = value.City,
                CreatedDate = value.CreatedDate,
                LandCategoryId = value.LandCategoryId,
                BlackBox = value.BlackBox,
                Dues = value.Dues,
                Exchange = value.Exchange,
                Facade = value.Facade,
                FloorAreaRatio = value.FloorAreaRatio,
                Furnished = value.Furnished,
                GrossArea = value.GrossArea,
                Heating = value.Heating,
                HomeLoan = value.HomeLoan,
                IsElevator = value.IsElevator,
                LandLoan = value.LandLoan,
                NumberOfBathrooms = value.NumberOfBathrooms,
                NetArea = value.NetArea,
                NumberOfFloors = value.NumberOfFloors,
                NumberOfBathRoom = value.NumberOfBathRoom,
                NumberOfKitchens = value.NumberOfKitchens,
                NumberOfRoom = value.NumberOfRoom,
                ParcelNumber = value.ParcelNumber,
                OpenArea = value.OpenArea,
                PricePerSquareMeter = value.PricePerSquareMeter,
                ListingType = value.ListingType,
                ParkingLot = value.ParkingLot,
                PlotNumber = value.PlotNumber,
                ListingId = value.ListingId,
                ZoningPlan = value.ZoningPlan,
                ZoningStatus = value.ZoningStatus,
                PropImgUrl1 = value.PropImgUrl1 ?? string.Empty,
                PropImgUrl10 = value.PropImgUrl10 ?? string.Empty,
                PropImgUrl11 = value.PropImgUrl11 ?? string.Empty,
                PropImgUrl12 = value.PropImgUrl12 ?? string.Empty,
                PropImgUrl13 = value.PropImgUrl13 ?? string.Empty,
                PropImgUrl14 = value.PropImgUrl14 ?? string.Empty,
                PropImgUrl15 = value.PropImgUrl15 ?? string.Empty,
                PropImgUrl16 = value.PropImgUrl16,
                PropImgUrl17 = value.PropImgUrl17,
                PropImgUrl18 = value.PropImgUrl18,
                PropImgUrl19 = value.PropImgUrl19,
                PropImgUrl2 = value.PropImgUrl2 ?? string.Empty,
                PropImgUrl20 = value.PropImgUrl20,
                PropImgUrl21 = value.PropImgUrl21,
                PropImgUrl22 = value.PropImgUrl22,
                PropImgUrl23 = value.PropImgUrl23,
                PropImgUrl24 = value.PropImgUrl24,
                PropImgUrl25 = value.PropImgUrl25,
                PropImgUrl26 = value.PropImgUrl26,
                PropImgUrl27 = value.PropImgUrl27,
                PropImgUrl28 = value.PropImgUrl28,
                PropImgUrl29 = value.PropImgUrl29,
                PropImgUrl3 = value.PropImgUrl3 ?? string.Empty,
                PropImgUrl30 = value.PropImgUrl30,
                PropImgUrl4 = value.PropImgUrl4 ?? string.Empty,
                PropImgUrl5 = value.PropImgUrl5 ?? string.Empty,
                PropImgUrl6 = value.PropImgUrl6 ?? string.Empty,
                PropImgUrl7 = value.PropImgUrl7 ?? string.Empty,
                PropImgUrl8 = value.PropImgUrl8 ?? string.Empty,
                PropImgUrl9 = value.PropImgUrl9 ?? string.Empty,
            };

        }


    }
}
