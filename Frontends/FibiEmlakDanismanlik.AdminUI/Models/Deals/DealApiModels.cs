using System.Globalization;

namespace FibiEmlakDanismanlik.AdminUI.Models.Deals;

/// <summary>
/// Yönetim paneli işlemler ve hak ediş listesi API yanıt modeli.
/// </summary>
public sealed class DealListResponse
{
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<DealRowDto> Rows { get; set; } = new();
    public DealSummaryDto Summary { get; set; } = new();
}

/// <summary>
/// Yönetim paneli işlemler durum ve ciro özeti.
/// </summary>
public sealed class DealSummaryDto
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
/// Yönetim paneli işlem listesi satır verisi.
/// </summary>
public class DealRowDto
{
    public int ListingDealId { get; set; }
    public int ListingTypeId { get; set; }
    public int ListingId { get; set; }
    public string ListingTitle { get; set; } = string.Empty;
    public int DealType { get; set; }
    public int Status { get; set; }
    public decimal AgreedPrice { get; set; }
    public decimal? DepositAmount { get; set; }
    public decimal TotalCommission { get; set; }
    public decimal TotalShares { get; set; }
    public decimal CompanyShare { get; set; }
    public string? CustomerName { get; set; }
    public DateTime? DealDate { get; set; }
    public List<string> ShareAgentNames { get; set; } = new();
}

/// <summary>
/// Yönetim paneli işlem ayrıntılı görünüm verisi.
/// </summary>
public sealed class DealDetailDto
{
    public int ListingDealId { get; set; }
    public int ListingTypeId { get; set; }
    public int ListingId { get; set; }
    public string ListingTitle { get; set; } = string.Empty;
    public string? ListingStatus { get; set; }
    public int DealType { get; set; }
    public int Status { get; set; }
    public decimal AgreedPrice { get; set; }
    public decimal? DepositAmount { get; set; }
    public DateTime? DepositDate { get; set; }
    public decimal? BuyerCommissionRate { get; set; }
    public decimal? SellerCommissionRate { get; set; }
    public decimal? BuyerCommissionAmount { get; set; }
    public decimal? SellerCommissionAmount { get; set; }
    public int? DepositOutcome { get; set; }
    public decimal? DepositRefundAmount { get; set; }
    public DateTime? DepositRefundDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public DateTime? CancelledDate { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string? Notes { get; set; }
    public int CreatedByAgentId { get; set; }
    public string? CreatedByAgentName { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public int? LinkedLeadSource { get; set; }
    public int? LinkedLeadId { get; set; }
    public string? LinkedLeadName { get; set; }
    public decimal TotalCommission { get; set; }
    public decimal TotalShares { get; set; }
    public decimal CompanyShare { get; set; }
    public List<DealShareDto> Shares { get; set; } = new();
}

/// <summary>
/// İşlem prim/hak ediş paylaşım satırı.
/// </summary>
public sealed class DealShareDto
{
    public int DealShareId { get; set; }
    public int AgentId { get; set; }
    public string AgentName { get; set; } = string.Empty;
    public int RoleAtDeal { get; set; }
    public decimal? SharePercent { get; set; }
    public decimal Amount { get; set; }
    public int PaymentStatus { get; set; }
    public DateTime? PaidDate { get; set; }
    public string? Note { get; set; }
}

/// <summary>
/// Yeni oluşturulan işlem veya prim kaydı yanıtı.
/// </summary>
public sealed class CreatedIdDto
{
    public int Id { get; set; }
}

/// <summary>
/// İşlemler için durum, tür, rol etiketleri ve sayı/tarih biçimlendirme yöntemleri.
/// </summary>
public static class DealLabels
{
    public static string StatusName(int status) => status switch
    {
        1 => "Kapora alındı",
        2 => "Tamamlandı",
        3 => "İptal",
        _ => "Bilinmiyor"
    };

    public static string StatusBadge(int status) => status switch
    {
        1 => "warning",
        2 => "success",
        3 => "secondary",
        _ => "secondary"
    };

    public static string DealTypeName(int dealType) => dealType switch
    {
        1 => "Satış",
        2 => "Kiralama",
        _ => "Bilinmiyor"
    };

    public static string RoleName(int role) => role switch
    {
        1 => "Sahip",
        2 => "Yönetici",
        3 => "Danışman",
        _ => "Bilinmiyor"
    };

    public static string PaymentName(int paymentStatus) => paymentStatus switch
    {
        1 => "Bekliyor",
        2 => "Ödendi",
        _ => "Bilinmiyor"
    };

    public static string PaymentBadge(int paymentStatus) => paymentStatus switch
    {
        1 => "warning",
        2 => "success",
        _ => "secondary"
    };

    public static string DepositOutcomeName(int? outcome) => outcome switch
    {
        1 => "Kapora geri verilmedi",
        2 => "Kapora iade edildi",
        _ => "—"
    };

    public static string Money(decimal? v) =>
        v.HasValue ? v.Value.ToString("#,##0.##", new CultureInfo("tr-TR")) + " ₺" : "—";

    public static string Date(DateTime? d) =>
        d.HasValue ? d.Value.ToString("dd.MM.yyyy") : "—";

    public static string InputDate(DateTime? d) =>
        d.HasValue ? d.Value.ToString("yyyy-MM-dd") : "";

    public static string InputNumber(decimal? v) =>
        v?.ToString(CultureInfo.InvariantCulture) ?? "";

    public static decimal? ParseDecimal(string? s)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            return null;
        }

        var clean = s.Replace(" ", "").Replace("₺", "").Trim();
        if (string.IsNullOrWhiteSpace(clean))
        {
            return null;
        }

        var lastComma = clean.LastIndexOf(',');
        var lastDot = clean.LastIndexOf('.');

        if (lastComma != -1 && lastDot != -1)
        {
            if (lastComma > lastDot)
            {
                clean = clean.Replace(".", "").Replace(',', '.');
            }
            else
            {
                clean = clean.Replace(",", "");
            }
        }
        else if (lastComma != -1)
        {
            clean = clean.Replace(',', '.');
        }
        else if (lastDot != -1)
        {
            var dotCount = clean.Count(c => c == '.');
            if (dotCount > 1)
            {
                clean = clean.Replace(".", "");
            }
            else if (dotCount == 1)
            {
                var digitsAfterDot = clean.Length - 1 - lastDot;
                if (digitsAfterDot == 3)
                {
                    clean = clean.Replace(".", "");
                }
            }
        }

        if (decimal.TryParse(clean, NumberStyles.Number, CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }

        return null;
    }
}
