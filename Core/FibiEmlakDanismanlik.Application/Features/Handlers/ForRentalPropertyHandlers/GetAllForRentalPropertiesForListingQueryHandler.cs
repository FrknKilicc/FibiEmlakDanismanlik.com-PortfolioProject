using FibiEmlakDanismanlik.Application.Features.Queries.ForRentalPropertyQueries;
using FibiEmlakDanismanlik.Application.Features.Results.ForRentalResults;
using FibiEmlakDanismanlik.Application.Interfaces.PropertyInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ForRentalPropertyHandlers
{
    public class GetAllForRentalPropertiesForListingQueryHandler : IRequestHandler<GetAllForRentalPropertiesForListingQuery, List<GetAllForRentalPropertiesForListingResult>>
    {
        private readonly IPropertyRepository _propertyRepository;

        public GetAllForRentalPropertiesForListingQueryHandler(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }

        public async Task<List<GetAllForRentalPropertiesForListingResult>> Handle(GetAllForRentalPropertiesForListingQuery request, CancellationToken cancellationToken)
        {
            var value = _propertyRepository.GetAllForRentalPropertyForListing();
            return value.Select(x => new GetAllForRentalPropertiesForListingResult
            {
                BuildingAge = x.BuildingAge,
                GrossArea = x.GrossArea,
                NetArea = x.NetArea,
                Heating = x.Heating,
                NumberOfFloors = x.NumberOfFloors,
                RentalHousingListId = x.RentalHousingListId,
                RentalCommercialListId = x.RentalCommercialListId,
                RentalLandListingId = x.RentalLandListingId,
                HousingCategoryId = x.HousingCategoryId,
                IsElevator = x.IsElevator,
                OpenArea = x.OpenArea,
                TotalNumberOfFloor = x.TotalNumberOfFloor,
                NumberOfBathRoom = x.NumberOfBathRoom,
                NumberOfBalconies = x.NumberOfBalconies,
                NumberOfRoom = x.NumberOfRoom,
                BlackBox = x.BlackBox,
                ParkingLot = x.ParkingLot,
                Furnished = x.Furnished,
                WithinTheComplex = x.WithinTheComplex,
                Dues = x.Dues,
                Facade = x.Facade,
                NumberOfSection = x.NumberOfSection,
                NumberOfKitchens = x.NumberOfKitchens,
                NumberOfBathrooms = x.NumberOfBathrooms,

                ZoningStatus = x.ZoningStatus,
                Area = x.Area,
                PricePerSquareMeter = x.PricePerSquareMeter,
                ParcelNumber = x.ParcelNumber,
                PlotNumber = x.PlotNumber,
                MapSheetNumber = x.MapSheetNumber,
                FloorAreaRatio = x.FloorAreaRatio,
                BaseAreaRatio = x.BaseAreaRatio,
                ZoningPlan = x.ZoningPlan,
                DevelopmentRight = x.DevelopmentRight,

                LandCategoryId = x.LandCategoryId,


            }).ToList();

        }
    }
}
