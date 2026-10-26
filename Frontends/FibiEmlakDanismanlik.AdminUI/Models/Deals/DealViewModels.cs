using FibiEmlakDanismanlik.AdminUI.Models.Leads;
using FibiEmlakDanismanlik.AdminUI.Models.Listings;

namespace FibiEmlakDanismanlik.AdminUI.Models.Deals;

/// <summary>
/// İşlemler ve hak ediş listesi sayfası görünüm modeli.
/// </summary>
public sealed class DealIndexViewModel
{
    public DealListResponse Response { get; set; } = new();
    public int? Durum { get; set; }
    public int? Tur { get; set; }
    public int? Danisman { get; set; }
    public string? Baslangic { get; set; }
    public string? Bitis { get; set; }
    public string? Ara { get; set; }
    public int Sayfa { get; set; } = 1;
    public List<AgentLookupDto> Agents { get; set; } = new();
    public string? ErrorMessage { get; set; }

    public int TotalPages => Response.PageSize > 0
        ? (int)Math.Ceiling((double)Response.Total / Response.PageSize)
        : 0;
}

/// <summary>
/// İşlem oluşturma ve düzenleme form modeli.
/// </summary>
public class DealFormModel
{
    public int ListingTypeId { get; set; }
    public int ListingId { get; set; }
    public string? AgreedPrice { get; set; }
    public string? DepositAmount { get; set; }
    public string? DepositDate { get; set; }
    public string? BuyerCommissionRate { get; set; }
    public string? SellerCommissionRate { get; set; }
    public string? BuyerCommissionAmount { get; set; }
    public string? SellerCommissionAmount { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string? Notes { get; set; }
    public string? LinkedLead { get; set; }
}

/// <summary>
/// İşlem oluşturma sayfası görünüm modeli.
/// </summary>
public sealed class DealCreateViewModel
{
    public DealFormModel Form { get; set; } = new();
    public string? ListingTitle { get; set; }
    public string? ListingStatus { get; set; }
    public bool IsRent { get; set; }
    public decimal? ListingPrice { get; set; }
    public List<LeadRowDto> Leads { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public Dictionary<string, string[]> FieldErrors { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public string? GetFieldError(string key)
    {
        if (FieldErrors.TryGetValue(key, out var errors) && errors.Length > 0)
        {
            return errors[0];
        }
        return null;
    }
}

/// <summary>
/// İşlem detay sayfası görünüm modeli.
/// </summary>
public sealed class DealDetailViewModel
{
    public DealDetailDto Detail { get; set; } = new();
    public List<AgentLookupDto> Agents { get; set; } = new();
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// İlan düzenleme sayfasındaki İşlemler ve Hak Ediş kartı modeli.
/// </summary>
public sealed class ListingDealsCardModel
{
    public List<DealRowDto> Deals { get; set; } = new();
    public int ListingTypeId { get; set; }
    public int ListingId { get; set; }
    public string? ListingStatus { get; set; }

    public bool CanCreate
    {
        get
        {
            var hasActive = Deals != null && Deals.Any(d => d.Status == 1);
            var isSoldOrRented = string.Equals(ListingStatus, "Satıldı", StringComparison.OrdinalIgnoreCase) ||
                                 string.Equals(ListingStatus, "Kiralandı", StringComparison.OrdinalIgnoreCase);
            return !hasActive && !isSoldOrRented;
        }
    }
}
