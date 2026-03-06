using FibiEmlakDanismanlik.Application.Features.Queries.ForRentalPropertyQueries;
using FibiEmlakDanismanlik.Application.Features.Results.ForRentalPropertyResults;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.PropertyInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ForRentalPropertyHandlers
{
    public class GetForRentalPropertyByIdQueryHandler : IRequestHandler<GetForRentalPropertyByIdQuery, GetForRentalPropertyDetailResult>
    {
        private readonly IPropertyRepository _repository;

        public GetForRentalPropertyByIdQueryHandler(IPropertyRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetForRentalPropertyDetailResult> Handle(GetForRentalPropertyByIdQuery request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetUnifiedForRentalPropertyById(request.Id);

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

            // Floorplan selection (satılıktakiyle aynı mantık)
            var selected = await _repository.GetSelectedImagesAsync(request.Id, "floorplan");

            result.FloorPlanImageItems = selected
                .OrderBy(x => x.SortOrder)
                .Select(x => new GetForRentalPropertyDetailResult.SelectedImageDto
                {
                    SortOrder = x.SortOrder,
                    Title = MapSlotToTitle(x.SlotKey),
                    Url = GetPropImgUrlByNo(result, x.ImageNo) ?? ""
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.Url))
                .Take(3)
                .ToList();

            return result;
        }
        private static string? GetPropImgUrlByNo(GetForRentalPropertyDetailResult v, int no) => no switch
        {
            1 => v.PropImgUrl1,
            2 => v.PropImgUrl2,
            3 => v.PropImgUrl3,
            4 => v.PropImgUrl4,
            5 => v.PropImgUrl5,
            6 => v.PropImgUrl6,
            7 => v.PropImgUrl7,
            8 => v.PropImgUrl8,
            9 => v.PropImgUrl9,
            10 => v.PropImgUrl10,
            11 => v.PropImgUrl11,
            12 => v.PropImgUrl12,
            13 => v.PropImgUrl13,
            14 => v.PropImgUrl14,
            15 => v.PropImgUrl15,
            16 => v.PropImgUrl16,
            17 => v.PropImgUrl17,
            18 => v.PropImgUrl18,
            19 => v.PropImgUrl19,
            20 => v.PropImgUrl20,
            21 => v.PropImgUrl21,
            22 => v.PropImgUrl22,
            23 => v.PropImgUrl23,
            24 => v.PropImgUrl24,
            25 => v.PropImgUrl25,
            26 => v.PropImgUrl26,
            27 => v.PropImgUrl27,
            28 => v.PropImgUrl28,
            29 => v.PropImgUrl29,
            30 => v.PropImgUrl30,
            _ => null
        };

        private static string MapSlotToTitle(string slotKey) => (slotKey ?? "").ToLower() switch
        {
            "kitchen" => "Mutfak",
            "bathroom" => "Banyo",
            "livingroom" => "Salon",
            "exterior" => "Dış Cephe",
            "street" => "Sokak",
            "interior" => "İç Mekan",
            "zoningplan" => "İmar Planı",
            "parcelsketch" => "Kroki",
            _ => "Görsel"
        };
    }
}
