using FibiEmlakDanismanlik.AdminUI.Models.Leads;
using FibiEmlakDanismanlik.AdminUI.Models.ListingAlerts;

namespace FibiEmlakDanismanlik.AdminUI.Models.Listings;

/// <summary>
/// İlan ekleme ve düzenleme sayfası görünüm modeli.
/// </summary>
public sealed class ListingFormViewModel
{
    public ListingFormModel Form { get; set; } = new();
    public AdminListingLookupsDto Lookups { get; set; } = new();
    public bool CanEdit { get; set; } = true;
    public bool IsNew => !Form.ListingId.HasValue || Form.ListingId.Value <= 0;
    public string? ErrorMessage { get; set; }
    public Dictionary<string, string[]> FieldErrors { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public List<ListingImageDto> Images { get; set; } = new();
    public List<ListingAccessGrantDto> Grants { get; set; } = new();
    public List<LeadRowDto> Leads { get; set; } = new();
    public ListingAlertsForListingDto Alerts { get; set; } = new();
    public List<FibiEmlakDanismanlik.AdminUI.Models.Deals.DealRowDto> Deals { get; set; } = new();
    public string? CurrentStatus { get; set; }
    public bool BestDeals { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? GeoSource { get; set; }

    // Statik Seçenek Listeleri
    public static readonly IReadOnlyList<string> BuildingAgeOptions = new[]
    {
        "0", "1-5", "6-10", "11-15", "16-20", "21 ve üzeri"
    };

    public static readonly IReadOnlyList<string> HeatingOptions = new[]
    {
        "Doğalgaz (Kombi)", "Merkezi (Pay Ölçer)", "Merkezi (Klima)", "Klima", "Yerden Isıtma", "Yok"
    };

    public static readonly IReadOnlyList<string> UsageStatusOptions = new[]
    {
        "Boş", "Kiracılı", "Mülk Sahibi"
    };

    public static readonly IReadOnlyList<string> TitleDeedStatusOptions = new[]
    {
        "Kat Mülkiyetli", "Kat İrtifaklı", "Müstakil Tapulu", "Hisseli Tapulu"
    };

    public static readonly IReadOnlyList<string> FacadeOptions = new[]
    {
        "Kuzey", "Güney", "Doğu", "Batı", "Kuzey-Doğu", "Kuzey-Batı", "Güney-Doğu", "Güney-Batı", "Caddeye Bakan"
    };

    public static readonly IReadOnlyList<string> NumberOfRoomOptions = new[]
    {
        "Stüdyo (1+0)", "1+1", "2+1", "3+1", "4+1", "5+1", "6+1 ve üzeri"
    };

    public static readonly IReadOnlyList<string> ZoningStatusOptions = new[]
    {
        "Konut İmarlı", "Ticari İmarlı", "Konut + Ticari İmarlı", "Sanayi İmarlı", "Turizm İmarlı", "Tarla", "Bağ & Bahçe", "Zeytinlik"
    };

    /// <summary>
    /// Statik listede mevcut olmayan bir değer varsa veri kaybını önlemek için onu da listeye dahil eder.
    /// </summary>
    public static IEnumerable<string> GetOptionsWithCustom(IReadOnlyList<string> standardOptions, string? currentValue)
    {
        var list = new List<string>(standardOptions);
        if (!string.IsNullOrWhiteSpace(currentValue) &&
            !list.Any(o => string.Equals(o, currentValue, StringComparison.OrdinalIgnoreCase)))
        {
            list.Add(currentValue);
        }
        return list;
    }

    /// <summary>
    /// Belirtilen alana ait ilk hata mesajını döndürür.
    /// </summary>
    public string? GetFieldError(string fieldName)
    {
        if (FieldErrors.TryGetValue(fieldName, out var errors) && errors.Length > 0)
        {
            return errors[0];
        }
        return null;
    }

    /// <summary>
    /// İlan kategorisine uygun olanakları filtreler (konutta appliesToHousing, arsada appliesToLand, işyerinde appliesToCommercial).
    /// </summary>
    public IEnumerable<AmenityLookupDto> ApplicableAmenities
    {
        get
        {
            if (Lookups?.Amenities == null)
            {
                return Enumerable.Empty<AmenityLookupDto>();
            }

            var cat = Form.Category;
            return Lookups.Amenities.Where(a =>
                (cat == "housing" && a.AppliesToHousing) ||
                (cat == "land" && a.AppliesToLand) ||
                (cat == "commercial" && a.AppliesToCommercial));
        }
    }

    /// <summary>
    /// İlan durumu için Bootstrap rozet sınıfını döndürür.
    /// </summary>
    public string StatusBadgeClass(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return "secondary";
        }

        var normalized = status.Trim();

        if (string.Equals(normalized, "Aktif", StringComparison.OrdinalIgnoreCase))
        {
            return "success";
        }

        if (string.Equals(normalized, "Opsiyonlu", StringComparison.OrdinalIgnoreCase))
        {
            return "warning";
        }

        if (string.Equals(normalized, "Pasif", StringComparison.OrdinalIgnoreCase))
        {
            return "secondary";
        }

        if (string.Equals(normalized, "Satıldı", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(normalized, "Satildi", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(normalized, "Kiralandı", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(normalized, "Kiralandi", StringComparison.OrdinalIgnoreCase))
        {
            return "dark";
        }

        return "secondary";
    }
}
