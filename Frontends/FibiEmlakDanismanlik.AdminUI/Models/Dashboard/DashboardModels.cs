namespace FibiEmlakDanismanlik.AdminUI.Models.Dashboard;

/// <summary>
/// Kontrol paneli ve raporlar API ana yanıt modeli.
/// </summary>
public sealed class DashboardResponse
{
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public bool CanSeeFinance { get; set; }
    public DashboardListingsDto Listings { get; set; } = new();
    public DashboardLeadsDto Leads { get; set; } = new();
    public DashboardLeadStatsDto LeadStats { get; set; } = new();
    public DashboardAlertsDto Alerts { get; set; } = new();
    public List<DashboardTopListingDto> TopListings { get; set; } = new();
    public DashboardFinanceDto? Finance { get; set; }
    public List<DashboardAgentDto> Agents { get; set; } = new();
}

/// <summary>
/// İlan sayaçları ve kategori/durum dağılımları.
/// </summary>
public sealed class DashboardListingsDto
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
/// Müşteri talepleri genel durum sayaçları.
/// </summary>
public sealed class DashboardLeadsDto
{
    public int Total { get; set; }
    public Dictionary<string, int> ByStatus { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public int Unassigned { get; set; }
    public int NewLast7Days { get; set; }
    public int AppointmentsUpcoming { get; set; }
}

/// <summary>
/// Seçili aralıktaki talep hareketleri ve randevu istatistikleri.
/// </summary>
public sealed class DashboardLeadStatsDto
{
    public int CreatedInRange { get; set; }
    public Dictionary<string, int> ByStatusInRange { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public int UpcomingAppointments7Days { get; set; }
    public int TodayAppointments { get; set; }
}

/// <summary>
/// Haber bekleyen ilan alarmları sayaçları.
/// </summary>
public sealed class DashboardAlertsDto
{
    public int Active { get; set; }
    public int ActiveWithPending { get; set; }
    public int Closed { get; set; }
    public int Unassigned { get; set; }
}

/// <summary>
/// En çok talep alan ilan özeti.
/// </summary>
public sealed class DashboardTopListingDto
{
    public int ListingTypeId { get; set; }
    public int ListingId { get; set; }
    public string ListingTitle { get; set; } = string.Empty;
    public int LeadCount { get; set; }
}

/// <summary>
/// Finansal özet ve aylık kırılımlar (yalnızca yetkili kullanıcılar görür).
/// </summary>
public sealed class DashboardFinanceDto
{
    public DashboardMonthDealsDto ThisMonth { get; set; } = new();
    public DashboardRangeDto Range { get; set; } = new();
    public List<DashboardMonthPointDto> Monthly { get; set; } = new();
}

/// <summary>
/// İçinde bulunulan ayın işlem ve kapora özeti.
/// </summary>
public sealed class DashboardMonthDealsDto
{
    public int DepositCount { get; set; }
    public decimal DepositTotal { get; set; }
    public int CompletedCount { get; set; }
    public decimal CompletedCommission { get; set; }
}

/// <summary>
/// Seçili tarih aralığındaki finansal toplamlar.
/// </summary>
public sealed class DashboardRangeDto
{
    public int DepositReceivedCount { get; set; }
    public int CompletedCount { get; set; }
    public int CancelledCount { get; set; }
    public decimal CompletedCommissionTotal { get; set; }
    public decimal CompletedSharesTotal { get; set; }
    public decimal CompanyShareTotal { get; set; }
    public decimal PendingSharesTotal { get; set; }
    public decimal KeptDepositTotal { get; set; }
}

/// <summary>
/// Son 12 ayın her bir ayı için finansal işlem noktası.
/// </summary>
public sealed class DashboardMonthPointDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int CompletedCount { get; set; }
    public decimal Commission { get; set; }
    public decimal Shares { get; set; }
    public int DepositCount { get; set; }
}

/// <summary>
/// Danışman/çalışan performansı ve hak ediş toplamları.
/// </summary>
public sealed class DashboardAgentDto
{
    public int AgentId { get; set; }
    public string AgentName { get; set; } = string.Empty;
    public int AssignedLeads { get; set; }
    public int WonLeads { get; set; }
    public int DealCount { get; set; }
    public decimal ShareTotal { get; set; }
    public decimal PaidTotal { get; set; }
    public decimal PendingTotal { get; set; }
}

/// <summary>
/// Kontrol paneli görünüm modeli.
/// </summary>
public sealed class DashboardViewModel
{
    public DashboardResponse? Data { get; set; }
    public string? Baslangic { get; set; }
    public string? Bitis { get; set; }
    public string? ErrorMessage { get; set; }
    public bool CanSeeFinance { get; set; }
    public List<SyncFailureItem> SyncFailures { get; set; } = new();
}

/// <summary>
/// Raporlar sayfası görünüm modeli.
/// </summary>
public sealed class ReportsViewModel
{
    public DashboardResponse? Data { get; set; }
    public string? Baslangic { get; set; }
    public string? Bitis { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Talep durum anahtarları ve kısa ay adları yardımcı sınıfı.
/// </summary>
public static class LeadStatusKeys
{
    public static readonly (string Key, string Name)[] Ordered =
    {
        ("New", "Yeni"),
        ("Contacted", "Arandı"),
        ("Appointment", "Randevu"),
        ("Offer", "Teklif"),
        ("Deposit", "Kapora"),
        ("Won", "Kazanıldı"),
        ("Lost", "Kaybedildi")
    };

    public static string MonthName(int month) => month switch
    {
        1 => "Oca",
        2 => "Şub",
        3 => "Mar",
        4 => "Nis",
        5 => "May",
        6 => "Haz",
        7 => "Tem",
        8 => "Ağu",
        9 => "Eyl",
        10 => "Eki",
        11 => "Kas",
        12 => "Ara",
        _ => month.ToString()
    };
}
