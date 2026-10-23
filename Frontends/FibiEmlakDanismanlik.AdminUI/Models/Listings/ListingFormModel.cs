using System.ComponentModel.DataAnnotations;

namespace FibiEmlakDanismanlik.AdminUI.Models.Listings;

/// <summary>
/// İlan ekleme ve düzenleme formu veri modeli (tüm ilan türleri için birleşik alanlar).
/// </summary>
public sealed class ListingFormModel
{
    public int ListingTypeId { get; set; }
    public int? ListingId { get; set; }

    [Required(ErrorMessage = "İlan başlığı zorunludur.")]
    [StringLength(200, ErrorMessage = "İlan başlığı en fazla 200 karakter olabilir.")]
    public string? PropertyName { get; set; }

    [Required(ErrorMessage = "İlan açıklaması zorunludur.")]
    [StringLength(8000, ErrorMessage = "İlan açıklaması en fazla 8000 karakter olabilir.")]
    public string? PropertyDescription { get; set; }

    public string? PropertyStatus { get; set; }

    public decimal? Price { get; set; }
    public decimal? Rent { get; set; }
    public decimal? Deposit { get; set; }
    public decimal? PricePerSquareMeter { get; set; }
    public decimal? Dues { get; set; }

    [Required(ErrorMessage = "İl seçimi zorunludur.")]
    public string? City { get; set; }

    [Required(ErrorMessage = "İlçe seçimi zorunludur.")]
    public string? District { get; set; }

    public string? Neighborhood { get; set; }
    public int? CityId { get; set; }
    public int? DistrictId { get; set; }
    public int? NeighborhoodId { get; set; }
    public string? AddressDesc { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? GeoSource { get; set; }
    public bool? BestDeals { get; set; }

    public string? Facade { get; set; }
    public bool? IsElevator { get; set; }
    public decimal? GrossArea { get; set; }
    public decimal? NetArea { get; set; }
    public decimal? OpenArea { get; set; }
    public decimal? Area { get; set; }
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
    public decimal? SharePercentage { get; set; }
    public string? ZoningStatus { get; set; }
    public string? ParcelNumber { get; set; }
    public string? PlotNumber { get; set; }
    public string? MapSheetNumber { get; set; }
    public decimal? FloorAreaRatio { get; set; }
    public decimal? BaseAreaRatio { get; set; }
    public string? ZoningPlan { get; set; }
    public bool? DevelopmentRight { get; set; }
    public int? HousingCategoryId { get; set; }
    public int? LandCategoryId { get; set; }
    public int? AgentId { get; set; }
    public string? AgentName { get; set; }
    public List<int> AmenityIds { get; set; } = new();

    /// <summary>
    /// İlanın kiralık olup olmadığını belirtir.
    /// </summary>
    public bool IsRent => ListingTypeId is 7 or 8 or 9;

    /// <summary>
    /// İlanın ana kategorisi ("housing", "land", "commercial").
    /// </summary>
    public string Category => ListingTypeId switch
    {
        2 or 8 => "housing",
        1 or 7 => "land",
        3 or 9 => "commercial",
        _ => string.Empty
    };

    /// <summary>
    /// İlan türünün Türkçe başlığı.
    /// </summary>
    public string TypeTitle => ListingTypeId switch
    {
        1 => "Satılık Arsa",
        2 => "Satılık Konut",
        3 => "Satılık İşyeri",
        7 => "Kiralık Arsa",
        8 => "Kiralık Konut",
        9 => "Kiralık İşyeri",
        _ => "İlan"
    };

    private static readonly HashSet<string> CommonFields = new(StringComparer.OrdinalIgnoreCase)
    {
        nameof(PropertyName),
        nameof(PropertyDescription),
        nameof(City),
        nameof(District),
        nameof(Neighborhood),
        nameof(AddressDesc),
        nameof(AgentId),
        nameof(AmenityIds)
    };

    private static readonly Dictionary<int, HashSet<string>> FieldsByType = new()
    {
        // 2 satılık konut
        [2] = new(StringComparer.OrdinalIgnoreCase)
        {
            nameof(Price), nameof(Dues), nameof(HousingCategoryId), nameof(Facade), nameof(IsElevator),
            nameof(GrossArea), nameof(NetArea), nameof(OpenArea), nameof(BuildingAge), nameof(TotalNumberOfFloor),
            nameof(NumberOfFloors), nameof(NumberOfBathRoom), nameof(NumberOfRoom), nameof(NumberOfBalconies),
            nameof(Heating), nameof(BlackBox), nameof(ParkingLot), nameof(Furnished), nameof(UsageStatus),
            nameof(WithinTheComplex), nameof(Exchange), nameof(TitleDeedStatus), nameof(HomeLoan)
        },
        // 8 kiralık konut
        [8] = new(StringComparer.OrdinalIgnoreCase)
        {
            nameof(Rent), nameof(Deposit), nameof(Dues), nameof(HousingCategoryId), nameof(Facade), nameof(IsElevator),
            nameof(GrossArea), nameof(NetArea), nameof(OpenArea), nameof(BuildingAge), nameof(TotalNumberOfFloor),
            nameof(NumberOfFloors), nameof(NumberOfBathRoom), nameof(NumberOfRoom), nameof(NumberOfBalconies),
            nameof(Heating), nameof(BlackBox), nameof(ParkingLot), nameof(Furnished), nameof(WithinTheComplex)
        },
        // 1 satılık arsa
        [1] = new(StringComparer.OrdinalIgnoreCase)
        {
            nameof(Price), nameof(PricePerSquareMeter), nameof(LandCategoryId), nameof(ZoningStatus), nameof(SharePercentage),
            nameof(Area), nameof(ParcelNumber), nameof(PlotNumber), nameof(MapSheetNumber), nameof(FloorAreaRatio),
            nameof(BaseAreaRatio), nameof(ZoningPlan), nameof(TitleDeedStatus), nameof(DevelopmentRight), nameof(LandLoan), nameof(Exchange)
        },
        // 7 kiralık arsa
        [7] = new(StringComparer.OrdinalIgnoreCase)
        {
            nameof(Rent), nameof(PricePerSquareMeter), nameof(LandCategoryId), nameof(ZoningStatus), nameof(Area),
            nameof(ParcelNumber), nameof(PlotNumber), nameof(MapSheetNumber), nameof(FloorAreaRatio),
            nameof(BaseAreaRatio), nameof(ZoningPlan), nameof(TitleDeedStatus), nameof(DevelopmentRight)
        },
        // 3 satılık işyeri
        [3] = new(StringComparer.OrdinalIgnoreCase)
        {
            nameof(Price), nameof(Facade), nameof(NumberOfSection), nameof(NumberOfKitchens), nameof(NumberOfBathrooms),
            nameof(NumberOfFloors), nameof(GrossArea), nameof(Area), nameof(TitleDeedStatus), nameof(SharePercentage),
            nameof(LandLoan), nameof(Exchange), nameof(Transferable)
        },
        // 9 kiralık işyeri
        [9] = new(StringComparer.OrdinalIgnoreCase)
        {
            nameof(Rent), nameof(Deposit), nameof(Facade), nameof(NumberOfSection), nameof(NumberOfKitchens),
            nameof(NumberOfBathrooms), nameof(NumberOfFloors), nameof(GrossArea), nameof(NetArea),
            nameof(BuildingAge), nameof(Heating), nameof(TitleDeedStatus)
        }
    };

    /// <summary>
    /// Belirtilen alanın geçerli ilan türünde gösterilip gösterilmeyeceğini kontrol eder.
    /// </summary>
    /// <param name="fieldName">Kontrol edilecek alan adı.</param>
    /// <returns>Alan geçerli türe aitse true, aksi halde false.</returns>
    public bool Has(string fieldName)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
        {
            return false;
        }

        if (CommonFields.Contains(fieldName))
        {
            return true;
        }

        return FieldsByType.TryGetValue(ListingTypeId, out var set) && set.Contains(fieldName);
    }
}
