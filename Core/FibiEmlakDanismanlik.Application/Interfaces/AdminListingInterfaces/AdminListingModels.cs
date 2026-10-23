using System;
using System.Collections.Generic;
using FibiEmlakDanismanlik.Application.Constants;

namespace FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces
{
    /// <summary>
    /// Yönetim paneli ilan listeleme filtre parametreleri.
    /// </summary>
    public class AdminListingFilter
    {
        public bool? IsRent { get; set; }
        public string? Category { get; set; }
        public string? Status { get; set; }
        public int? AgentId { get; set; }
        public string? Search { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = AdminListingConstants.DefaultPageSize;
        public string? Sort { get; set; }
    }

    /// <summary>
    /// Yönetim paneli ilan tablosu satır modeli.
    /// </summary>
    public class AdminListingRow
    {
        public int ListingTypeId { get; set; }
        public int ListingId { get; set; }
        public bool IsRent { get; set; }
        public string Category { get; set; } = string.Empty;
        public string CategoryDisplay { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Neighborhood { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Status { get; set; } = string.Empty;
        public int AgentId { get; set; }
        public string AgentName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public bool BestDeals { get; set; }
        public string? CoverThumbUrl { get; set; }
        public int ImageCount { get; set; }
        public bool CanEdit { get; set; }
    }

    /// <summary>
    /// Yönetim paneli ilan üst özet sayaçları.
    /// </summary>
    public class AdminListingSummary
    {
        public int Total { get; set; }
        public int SaleTotal { get; set; }
        public int RentTotal { get; set; }
        public Dictionary<string, int> ByCategorySale { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, int> ByCategoryRent { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, int> ByStatus { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public int BestDealsSale { get; set; }
        public int BestDealsRent { get; set; }
    }

    /// <summary>
    /// Yönetim paneli ilan detay modeli (tüm ilan türlerinin birleşik alanları).
    /// </summary>
    public class AdminListingDetail
    {
        // Kimlik ve Tür Bilgileri
        public int ListingTypeId { get; set; }
        public int ListingId { get; set; }
        public bool IsRent { get; set; }
        public string Category { get; set; } = string.Empty;
        public string CategoryDisplay { get; set; } = string.Empty;

        // Ortak ve Skaler Alanlar
        public int? PropertyNo { get; set; }
        public string? PropertyName { get; set; }
        public string? PropertyDescription { get; set; }
        public string? PropertyStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public decimal? Price { get; set; }
        public decimal? Rent { get; set; }
        public decimal? Deposit { get; set; }
        public decimal? PricePerSquareMeter { get; set; }
        public decimal? Dues { get; set; }

        // Konum ve Adres
        public string? City { get; set; }
        public string? District { get; set; }
        public int? CityId { get; set; }
        public int? DistrictId { get; set; }
        public int? NeighborhoodId { get; set; }
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

        // Kategori, Danışman ve Fırsat Bilgisi
        public bool? BestDeals { get; set; }
        public int? HousingCategoryId { get; set; }
        public int? LandCategoryId { get; set; }
        public int? AgentId { get; set; }
        public string? AgentName { get; set; }

        // Olanaklar ve Coğrafi Konum
        public List<int> AmenityIds { get; set; } = new();
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? GeoPrecision { get; set; }
        public string? GeoSource { get; set; }
    }
}
