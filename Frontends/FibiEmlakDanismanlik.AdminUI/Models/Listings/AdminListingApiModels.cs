namespace FibiEmlakDanismanlik.AdminUI.Models.Listings;

/// <summary>
/// Yönetim paneli ilan listesi API yanıt modeli.
/// </summary>
public sealed class AdminListingListResponse
{
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<AdminListingRowDto> Items { get; set; } = new();
    public AdminListingSummaryDto Summary { get; set; } = new();
}

/// <summary>
/// Yönetim paneli ilan listesi satır verisi.
/// </summary>
public sealed class AdminListingRowDto
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
/// İlan listesi özet istatistik verileri.
/// </summary>
public sealed class AdminListingSummaryDto
{
    public int Total { get; set; }
    public int SaleTotal { get; set; }
    public int RentTotal { get; set; }
    public AdminListingCategoryCountDto ByCategorySale { get; set; } = new();
    public AdminListingCategoryCountDto ByCategoryRent { get; set; } = new();
    public Dictionary<string, int> ByStatus { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public int BestDealsSale { get; set; }
    public int BestDealsRent { get; set; }
}

/// <summary>
/// Kategori bazlı ilan adetleri.
/// </summary>
public sealed class AdminListingCategoryCountDto
{
    public int Housing { get; set; }
    public int Land { get; set; }
    public int Commercial { get; set; }
}

/// <summary>
/// İlan filtreleme ve form lookup verileri.
/// </summary>
public sealed class AdminListingLookupsDto
{
    public List<AmenityLookupDto> Amenities { get; set; } = new();
    public List<HousingCategoryLookupDto> HousingCategories { get; set; } = new();
    public List<LandCategoryLookupDto> LandCategories { get; set; } = new();
    public List<AgentLookupDto> Agents { get; set; } = new();
    public List<string> Statuses { get; set; } = new();
}

/// <summary>
/// Danışman lookup öğesi.
/// </summary>
public sealed class AgentLookupDto
{
    public int AgentId { get; set; }
    public string AgentName { get; set; } = string.Empty;
}

/// <summary>
/// Özellik / Donatı lookup öğesi.
/// </summary>
public sealed class AmenityLookupDto
{
    public int Id { get; set; }
    public int AmenityId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? AmenityName { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public bool AppliesToHousing { get; set; }
    public bool AppliesToLand { get; set; }
    public bool AppliesToCommercial { get; set; }

    public int Key => Id > 0 ? Id : AmenityId;
    public string DisplayName => !string.IsNullOrWhiteSpace(Name) ? Name : (AmenityName ?? string.Empty);
}

/// <summary>
/// Konut alt kategorisi lookup öğesi.
/// </summary>
public sealed class HousingCategoryLookupDto
{
    public int HousingCategoryId { get; set; }
    public string HousingCategoryName { get; set; } = string.Empty;
}

/// <summary>
/// Arsa alt kategorisi lookup öğesi.
/// </summary>
public sealed class LandCategoryLookupDto
{
    public int LandCategoryId { get; set; }
    public string LandCategoryName { get; set; } = string.Empty;
}

/// <summary>
/// Genel kategori lookup öğesi.
/// </summary>
public sealed class CategoryLookupDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// İlan detay API yanıt modeli.
/// </summary>
public sealed class AdminListingDetailResponse
{
    public ListingFormModel? Listing { get; set; }
    public bool CanEdit { get; set; }
}

/// <summary>
/// İlan kaydetme (POST/PUT) API yanıt modeli.
/// </summary>
public sealed class SaveListingResponse
{
    public int ListingTypeId { get; set; }
    public int ListingId { get; set; }
}

/// <summary>
/// Konum seçim API öğesi (İl, İlçe, Mahalle).
/// </summary>
public sealed class LocationOptionDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
}
