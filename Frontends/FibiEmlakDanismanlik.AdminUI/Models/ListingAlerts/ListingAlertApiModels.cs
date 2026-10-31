using System.Globalization;

namespace FibiEmlakDanismanlik.AdminUI.Models.ListingAlerts;

/// <summary>
/// Yönetim paneli haber bekleyenler listesi API yanıt modeli.
/// </summary>
public sealed class ListingAlertListResponse
{
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<ListingAlertRowDto> Items { get; set; } = new();
    public ListingAlertSummaryDto Summary { get; set; } = new();
}

/// <summary>
/// Yönetim paneli haber bekleyenler durum ve sayaç özeti.
/// </summary>
public sealed class ListingAlertSummaryDto
{
    public int Active { get; set; }
    public int ActiveWithPending { get; set; }
    public int Closed { get; set; }
    public int Unassigned { get; set; }
}

/// <summary>
/// Yönetim paneli haber bekleyen talep listesi satır verisi.
/// </summary>
public class ListingAlertRowDto
{
    public int ListingAlertId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public int ListingTypeId { get; set; }
    public string TypeDisplay { get; set; } = string.Empty;
    public int CityId { get; set; }
    public string CityName { get; set; } = string.Empty;
    public int? DistrictId { get; set; }
    public string? DistrictName { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? RoomCounts { get; set; }
    public string? NotePreview { get; set; }
    public int Status { get; set; }
    public int? AssignedAgentId { get; set; }
    public string? AssignedAgentName { get; set; }
    public DateTime CreatedDate { get; set; }
    public int PendingMatchCount { get; set; }
    public int NotifiedCount { get; set; }
    public int? SourceListingTypeId { get; set; }
    public int? SourceListingId { get; set; }
    public string? SourceListingTitle { get; set; }
}

/// <summary>
/// Yönetim paneli haber bekleyen talep ayrıntılı görünüm verisi.
/// </summary>
public sealed class ListingAlertDetailDto : ListingAlertRowDto
{
    public string? Note { get; set; }
    public DateTime KvkkApprovedAt { get; set; }
    public DateTime? ClosedAtUtc { get; set; }
    public List<ListingAlertListingItemDto> PendingMatches { get; set; } = new();
    public List<ListingAlertMatchItemDto> History { get; set; } = new();
}

/// <summary>
/// Talep ile eşleşen veya bildirilen ilan bilgisi.
/// </summary>
public sealed class ListingAlertListingItemDto
{
    public int ListingTypeId { get; set; }
    public int ListingId { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string? NumberOfRoom { get; set; }
    public DateTime CreatedDate { get; set; }
    public int AgentId { get; set; }
    public string? AgentName { get; set; }
}

/// <summary>
/// Talep bildirim/eşleşme hareket geçmişi satırı.
/// </summary>
public sealed class ListingAlertMatchItemDto
{
    public int ListingAlertMatchId { get; set; }
    public int ListingTypeId { get; set; }
    public int ListingId { get; set; }
    public string? ListingTitle { get; set; }
    public int Status { get; set; }
    public string? AgentName { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

/// <summary>
/// Belirli bir ilan için eşleşen haber bekleyenler listesi ve kapsam dışı kayıt sayısı.
/// </summary>
public sealed class ListingAlertsForListingDto
{
    public List<ListingAlertForListingDto> Items { get; set; } = new();
    public int OtherCount { get; set; }
}

/// <summary>
/// Belirli bir ilanın kriterlerine uyan aktif haber talebi satırı.
/// </summary>
public sealed class ListingAlertForListingDto
{
    public int ListingAlertId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? AssignedAgentName { get; set; }
    public DateTime CreatedDate { get; set; }
    public int? MatchStatus { get; set; }
}

/// <summary>
/// İlan düzenleme ekranındaki "Bu İlanı Bekleyenler" kartı görünüm modeli.
/// </summary>
public sealed class ListingAlertsCardModel
{
    public ListingAlertsForListingDto Data { get; set; } = new();
    public int ListingTypeId { get; set; }
    public int ListingId { get; set; }
}

/// <summary>
/// Haber bekleyenler için durum ve eşleşme etiketleri ile yardımcı biçimlendirme yöntemleri.
/// </summary>
public static class ListingAlertLabels
{
    /// <summary>
    /// Talep durumu kimliğine göre Türkçe durum adını döndürür. (1: Aktif, 2: Kapandı)
    /// </summary>
    public static string StatusName(int status) => status switch
    {
        1 => "Aktif",
        2 => "Kapandı",
        _ => "Bilinmiyor"
    };

    /// <summary>
    /// Talep durumu kimliğine göre Bootstrap rozet rengini döndürür.
    /// </summary>
    public static string StatusBadge(int status) => status switch
    {
        1 => "success",
        2 => "secondary",
        _ => "secondary"
    };

    /// <summary>
    /// İlan eşleşme durumu kimliğine göre Türkçe durum adını döndürür. (1: Haber verildi, 2: Uygun değil)
    /// </summary>
    public static string MatchStatusName(int? status) => status switch
    {
        1 => "Haber verildi",
        2 => "Uygun değil",
        _ => "-"
    };

    /// <summary>
    /// İlan eşleşme durumu kimliğine göre Bootstrap rozet rengini döndürür.
    /// </summary>
    public static string MatchStatusBadge(int? status) => status switch
    {
        1 => "success",
        2 => "secondary",
        _ => "secondary"
    };

    /// <summary>
    /// Verilen taban ve tavan fiyat değerlerini kullanıcı dostu metin aralığına biçimlendirir.
    /// </summary>
    public static string PriceRange(decimal? minPrice, decimal? maxPrice)
    {
        var culture = CultureInfo.GetCultureInfo("tr-TR");
        if (minPrice.HasValue && maxPrice.HasValue)
        {
            return $"{minPrice.Value.ToString("N0", culture)} – {maxPrice.Value.ToString("N0", culture)} TL";
        }

        if (minPrice.HasValue)
        {
            return $"{minPrice.Value.ToString("N0", culture)} TL ve üzeri";
        }

        if (maxPrice.HasValue)
        {
            return $"{maxPrice.Value.ToString("N0", culture)} TL'ye kadar";
        }

        return "Fiyat fark etmez";
    }
}
