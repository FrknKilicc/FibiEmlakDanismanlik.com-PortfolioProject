using System.Collections.Generic;
using FibiEmlakDanismanlik.Application.Features.Results.AdminListingResults;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.AdminListingCommands
{
    /// <summary>
    /// Yönetim panelinde ilan oluşturma veya mevcut ilanı güncelleme komutu.
    /// ListingId null veya 0 ise yeni ilan oluşturulur, aksi halde mevcut ilan güncellenir.
    /// </summary>
    public class SaveAdminListingCommand : IRequest<SaveAdminListingResult>
    {
        // Kimlik ve Tür
        public int ListingTypeId { get; set; }
        public int? ListingId { get; set; }

        // Ortak ve Skaler Alanlar
        public int? PropertyNo { get; set; }
        public string? PropertyName { get; set; }
        public string? PropertyDescription { get; set; }
        public decimal? Price { get; set; }
        public decimal? Rent { get; set; }
        public decimal? Deposit { get; set; }
        public decimal? PricePerSquareMeter { get; set; }
        public decimal? Dues { get; set; }

        // Konum ve Adres (CityId/DistrictId/NeighborhoodId DbContext tarafından adlardan çözülür)
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Neighborhood { get; set; }
        public string? AddressDesc { get; set; }

        // Yapı / Konut / İşyeri Alanları
        public string? Facade { get; set; }
        public bool? IsElevator { get; set; }
        public double? GrossArea { get; set; }
        public double? NetArea { get; set; }
        public double? OpenArea { get; set; }
        public double? Area { get; set; }
        public string? BuildingAge { get; set; }
        public int? TotalNumberOfFloor { get; set; }
        public int? NumberOfFloors { get; set; }
        public int? NumberOfBathRoom { get; set; }
        public int? NumberOfBathrooms { get; set; }
        public string? NumberOfRoom { get; set; }
        public int? NumberOfBalconies { get; set; }
        public int? NumberOfSection { get; set; }
        public int? NumberOfKitchens { get; set; }
        public string? Heating { get; set; }
        public bool? BlackBox { get; set; }
        public bool? ParkingLot { get; set; }
        public bool? Furnished { get; set; }
        public string? UsageStatus { get; set; }
        public bool? WithinTheComplex { get; set; }
        public bool? Exchange { get; set; }
        public string? TitleDeedStatus { get; set; }
        public bool? HomeLoan { get; set; }
        public bool? LandLoan { get; set; }
        public bool? Transferable { get; set; }

        // Arsa Alanları
        public double? SharePercentage { get; set; }
        public string? ZoningStatus { get; set; }
        public string? ParcelNumber { get; set; }
        public string? PlotNumber { get; set; }
        public string? MapSheetNumber { get; set; }
        public double? FloorAreaRatio { get; set; }
        public double? BaseAreaRatio { get; set; }
        public string? ZoningPlan { get; set; }
        public string? DevelopmentRight { get; set; }

        // Kategori ve Danışman
        public int? HousingCategoryId { get; set; }
        public int? LandCategoryId { get; set; }
        public int? AgentId { get; set; }

        // Olanaklar
        public List<int> AmenityIds { get; set; } = new();
    }
}
