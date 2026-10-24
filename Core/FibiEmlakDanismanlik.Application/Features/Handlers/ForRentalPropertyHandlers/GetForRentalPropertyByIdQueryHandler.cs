using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Queries.ForRentalPropertyQueries;
using FibiEmlakDanismanlik.Application.Features.Results.ForRentalPropertyResults;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.ListingImageInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.PropertyInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ForRentalPropertyHandlers
{
    public class GetForRentalPropertyByIdQueryHandler : IRequestHandler<GetForRentalPropertyByIdQuery, GetForRentalPropertyDetailResult?>
    {
        private readonly IPropertyRepository _repository;
        private readonly IListingImageRepository _listingImageRepository;

        public GetForRentalPropertyByIdQueryHandler(
            IPropertyRepository repository,
            IListingImageRepository listingImageRepository)
        {
            _repository = repository;
            _listingImageRepository = listingImageRepository;
        }

        public async Task<GetForRentalPropertyDetailResult?> Handle(GetForRentalPropertyByIdQuery request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetUnifiedForRentalPropertyById(request.Id, request.ListingTypeId);

            // İlan bulunamadı, controller 404 döner
            if (value == null) return null;

            var result = new GetForRentalPropertyDetailResult
            {
                PropertyNo = value.PropertyNo,
                PropertyName = value.PropertyName,
                PropertyDescription = value.PropertyDescription,
                PropertyStatus = value.PropertyStatus,
                CreatedDate = value.CreatedDate,
                Rent = value.Rent,
                Deposit = value.Deposit,
                BestDeals = value.BestDeals,
                PropertyType = value.PropertyType,

                ListingId = value.ListingId,
                ListingType = value.ListingType,
                
                TitleDeedStatus = value.TitleDeedStatus,
                UsageTypeId = value.UsageTypeId,
                AgentTitle = value.AgentTitle,
                Mail = value.Mail,
                Amenities = value.Amenities,

                City = value.City,
                District = value.District,
                Neighborhood = value.Neighborhood,
                AddressDesc = value.AddressDesc,

                AgentId = value.AgentId,
                AgentName = value.AgentName,
                AgentImgUrl = value.AgentImgUrl,
                AgentDescription = value.AgentDescription,
                AgentPhoneNumber = value.AgentPhoneNumber,

                ListingTypeId = value.ListingTypeId,
                ListingTypeName = value.ListingTypeName,

                // Konut / işyeri / arsa alanları
                BuildingAge = value.BuildingAge,
                GrossArea = value.GrossArea,
                NetArea = value.NetArea,
                Heating = value.Heating,
                NumberOfFloors = value.NumberOfFloors,

                RentalHousingListId = value.RentalHousingListId,
                RentalCommercialListId = value.RentalCommercialListId,
                RentalLandListingId = value.RentalLandListingId,

                HousingCategoryId = value.HousingCategoryId,
                HousingCategoryName = value.HousingCategoryName,
                HousingCategory = value.HousingCategory,

                IsElevator = value.IsElevator,
                OpenArea = value.OpenArea,
                TotalNumberOfFloor = value.TotalNumberOfFloor,
                NumberOfBathRoom = value.NumberOfBathRoom,
                NumberOfRoom = value.NumberOfRoom,
                NumberOfBalconies = value.NumberOfBalconies,
                BlackBox = value.BlackBox,
                ParkingLot = value.ParkingLot,
                Furnished = value.Furnished,
                WithinTheComplex = value.WithinTheComplex,
                Dues = value.Dues,

                Facade = value.Facade,
                NumberOfSection = value.NumberOfSection,
                NumberOfKitchens = value.NumberOfKitchens,
                NumberOfBathrooms = value.NumberOfBathrooms,

                ZoningStatus = value.ZoningStatus,
                Area = value.Area,
                PricePerSquareMeter = value.PricePerSquareMeter,
                ParcelNumber = value.ParcelNumber,
                PlotNumber = value.PlotNumber,
                MapSheetNumber = value.MapSheetNumber,
                FloorAreaRatio = value.FloorAreaRatio,
                BaseAreaRatio = value.BaseAreaRatio,
                ZoningPlan = value.ZoningPlan,
                DevelopmentRight = value.DevelopmentRight,
                LandCategoryId = value.LandCategoryId,
                LandCategoryName = value.LandCategoryName,
                LandCategory = value.LandCategory,

                // Görseller
                PropImgUrl1 = value.PropImgUrl1,
                PropImgUrl2 = value.PropImgUrl2,
                PropImgUrl3 = value.PropImgUrl3,
                PropImgUrl4 = value.PropImgUrl4,
                PropImgUrl5 = value.PropImgUrl5,
                PropImgUrl6 = value.PropImgUrl6,
                PropImgUrl7 = value.PropImgUrl7,
                PropImgUrl8 = value.PropImgUrl8,
                PropImgUrl9 = value.PropImgUrl9,
                PropImgUrl10 = value.PropImgUrl10,
                PropImgUrl11 = value.PropImgUrl11,
                PropImgUrl12 = value.PropImgUrl12,
                PropImgUrl13 = value.PropImgUrl13,
                PropImgUrl14 = value.PropImgUrl14,
                PropImgUrl15 = value.PropImgUrl15,
                PropImgUrl16 = value.PropImgUrl16,
                PropImgUrl17 = value.PropImgUrl17,
                PropImgUrl18 = value.PropImgUrl18,
                PropImgUrl19 = value.PropImgUrl19,
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
                PropImgUrl30 = value.PropImgUrl30,
            };

            int listingTypeId = result.ListingTypeId ?? request.ListingTypeId ?? 0;
            if (listingTypeId == 0)
            {
                result.FloorPlanImageItems = new List<GetForRentalPropertyDetailResult.SelectedImageDto>();
            }
            else
            {
                var plans = await _listingImageRepository.GetPlanImagesAsync(listingTypeId, result.ListingId);
                result.FloorPlanImageItems = plans
                    .Select(x => new GetForRentalPropertyDetailResult.SelectedImageDto
                    {
                        SortOrder = x.SortOrder,
                        Title = string.IsNullOrWhiteSpace(x.Caption) ? ListingImageConstants.KindLabel(x.Kind) : x.Caption!,
                        Url = x.Url
                    })
                    .Take(10)
                    .ToList();
            }

            return result;
        }
    }
}
