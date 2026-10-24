namespace FibiEmlakDanismanlik.AdminUI.Models.Leads;

/// <summary>
/// Yönetim paneli müşteri talepleri listesi API yanıt modeli.
/// </summary>
public sealed class LeadListResponse
{
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<LeadRowDto> Items { get; set; } = new();
    public LeadSummaryDto Summary { get; set; } = new();
}

/// <summary>
/// Yönetim paneli müşteri talepleri özet sayaçları.
/// </summary>
public sealed class LeadSummaryDto
{
    public int Total { get; set; }
    public Dictionary<string, int> ByStatus { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public int Unassigned { get; set; }
    public int NewLast7Days { get; set; }
    public int AppointmentsUpcoming { get; set; }
}

/// <summary>
/// Yönetim paneli müşteri talepleri liste satır verisi.
/// </summary>
public class LeadRowDto
{
    public int Source { get; set; }
    public int LeadId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? MessagePreview { get; set; }
    public int Status { get; set; }
    public int? AssignedAgentId { get; set; }
    public string? AssignedAgentName { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? AppointmentDateTime { get; set; }
    public int? ListingTypeId { get; set; }
    public int? ListingId { get; set; }
    public string? ListingTitle { get; set; }
    public string? ServiceModule { get; set; }
    public string? NeighborhoodName { get; set; }
    public DateTime? LastActivityAtUtc { get; set; }
}

/// <summary>
/// Yönetim paneli müşteri talebi ayrıntılı görünüm verisi.
/// </summary>
public sealed class LeadDetailDto : LeadRowDto
{
    public string? Message { get; set; }
    public string? CustomerNote { get; set; }
    public string? CalculationSummaryJson { get; set; }
    public DateTime? KvkkApprovedAt { get; set; }
    public List<LeadActivityDto> Activities { get; set; } = new();
}

/// <summary>
/// Müşteri talebi hareket veya not geçmişi öğesi.
/// </summary>
public sealed class LeadActivityDto
{
    public int ActivityId { get; set; }
    public int Type { get; set; }
    public string? Text { get; set; }
    public int? OldStatus { get; set; }
    public int? NewStatus { get; set; }
    public int? AgentId { get; set; }
    public string? AgentName { get; set; }
    public string? AssignedAgentName { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

/// <summary>
/// Müşteri talepleri için durum, kaynak ve hareket etiketleri ile yardımcı biçimlendirme yöntemleri.
/// </summary>
public static class LeadLabels
{
    /// <summary>
    /// Talep durumu kimliğine göre Türkçe durum adını döndürür.
    /// </summary>
    public static string StatusName(int status) => status switch
    {
        1 => "Yeni",
        2 => "Arandı",
        3 => "Randevu",
        4 => "Teklif",
        5 => "Kapora",
        6 => "Kazanıldı",
        7 => "Kaybedildi",
        _ => "Bilinmiyor"
    };

    /// <summary>
    /// Talep durumu kimliğine göre Bootstrap rozet (badge) rengini döndürür.
    /// </summary>
    public static string StatusBadge(int status) => status switch
    {
        1 => "primary",
        2 => "info",
        3 => "warning",
        4 => "secondary",
        5 => "dark",
        6 => "success",
        7 => "danger",
        _ => "secondary"
    };

    /// <summary>
    /// Hareket türü kimliğine göre Türkçe etkinlik adını döndürür.
    /// </summary>
    public static string ActivityName(int type) => type switch
    {
        1 => "Not",
        2 => "Durum Değişikliği",
        3 => "Danışman Atama",
        4 => "Görüşme / Arama",
        _ => "Hareket"
    };

    /// <summary>
    /// Talep kaynağı kimliğine göre kullanıcı dostu Türkçe adı döndürür.
    /// </summary>
    public static string SourceName(int source) => source switch
    {
        1 => "İletişim / Randevu",
        2 => "FİBİ Hizmet",
        _ => "Diğer"
    };

    /// <summary>
    /// Talep kaynağı kimliğine göre URL yolu bileşenini (slug) döndürür.
    /// </summary>
    public static string SourceSlug(int source) => source switch
    {
        1 => "contact",
        2 => "service",
        _ => "contact"
    };

    /// <summary>
    /// Verilen UTC tarih-saat değerini Türkiye saat dilimine (Europe/Istanbul) dönüştürür.
    /// </summary>
    public static DateTime ToTurkeyTime(DateTime utc)
    {
        if (utc.Kind == DateTimeKind.Unspecified)
        {
            utc = DateTime.SpecifyKind(utc, DateTimeKind.Utc);
        }

        try
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Istanbul");
            return TimeZoneInfo.ConvertTimeFromUtc(utc, tz);
        }
        catch (TimeZoneNotFoundException)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(utc, tz);
        }
    }

    /// <summary>
    /// Verilen UTC tarih-saat değerini Türkiye saat dilimine çevirip dd.MM.yyyy HH:mm biçiminde metne dönüştürür.
    /// </summary>
    public static string FormatTurkeyTime(DateTime? utc)
    {
        if (!utc.HasValue)
        {
            return "-";
        }

        return ToTurkeyTime(utc.Value).ToString("dd.MM.yyyy HH:mm");
    }

    /// <summary>
    /// Telefon numarasını uluslararası WhatsApp arama bağlantısına dönüştürür (wa.me/90...).
    /// </summary>
    public static string ToWhatsAppLink(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            return string.Empty;
        }

        var digits = new string(phone.Where(char.IsDigit).ToArray());
        if (digits.StartsWith("0"))
        {
            digits = digits.Substring(1);
        }

        if (!digits.StartsWith("90"))
        {
            digits = "90" + digits;
        }

        return $"https://wa.me/{digits}";
    }

    /// <summary>
    /// Özet sözlüğünden (ByStatus) ilgili durum koduna ait sayı değerini bulur.
    /// </summary>
    public static int GetStatusCount(Dictionary<string, int>? byStatus, int status)
    {
        if (byStatus == null)
        {
            return 0;
        }

        var enumKey = status switch
        {
            1 => "New",
            2 => "Contacted",
            3 => "Appointment",
            4 => "Proposal",
            5 => "Deposit",
            6 => "Won",
            7 => "Lost",
            _ => string.Empty
        };

        if (!string.IsNullOrEmpty(enumKey) && byStatus.TryGetValue(enumKey, out var count))
        {
            return count;
        }

        if (byStatus.TryGetValue(status.ToString(), out count))
        {
            return count;
        }

        var trName = StatusName(status);
        if (byStatus.TryGetValue(trName, out count))
        {
            return count;
        }

        return 0;
    }
}
