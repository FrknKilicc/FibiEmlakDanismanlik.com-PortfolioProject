using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using FibiEmlakDanismanlik.AdminUI.Infrastructure;
using FibiEmlakDanismanlik.AdminUI.Models.Dashboard;
using FibiEmlakDanismanlik.AdminUI.Models.Deals;

namespace FibiEmlakDanismanlik.AdminUI.Controllers;

/// <summary>
/// Yönetim paneli raporlar ve CSV dışa aktarma denetleyicisi.
/// </summary>
[Authorize]
public sealed class ReportsController : Controller
{
    private static readonly CultureInfo TrCulture = new("tr-TR");
    private readonly ApiClient _apiClient;
    private readonly CurrentAdmin _currentAdmin;

    public ReportsController(ApiClient apiClient, CurrentAdmin currentAdmin)
    {
        _apiClient = apiClient;
        _currentAdmin = currentAdmin;
    }

    /// <summary>
    /// Raporlar ana sayfası.
    /// </summary>
    [HttpGet("/raporlar")]
    public async Task<IActionResult> Index(string? baslangic, string? bitis)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var queryParams = BuildRangeQueryParams(baslangic, bitis);
        var apiPath = QueryHelpers.AddQueryString("AdminDashboard", queryParams);
        var result = await _apiClient.GetAsync<DashboardResponse>(apiPath);

        if (result.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }

        var viewModel = new ReportsViewModel
        {
            Baslangic = baslangic,
            Bitis = bitis
        };

        if (result.Success && result.Data != null)
        {
            viewModel.Data = result.Data;
        }
        else
        {
            viewModel.ErrorMessage = result.ErrorMessage ?? "Rapor verileri yüklenirken bir hata oluştu.";
        }

        return View(viewModel);
    }

    /// <summary>
    /// Aylık özet raporunu CSV olarak dışa aktarır.
    /// </summary>
    [HttpGet("/raporlar/aylik.csv")]
    public async Task<IActionResult> MonthlyCsv(string? baslangic, string? bitis)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var queryParams = BuildRangeQueryParams(baslangic, bitis);
        var apiPath = QueryHelpers.AddQueryString("AdminDashboard", queryParams);
        var result = await _apiClient.GetAsync<DashboardResponse>(apiPath);

        if (result.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }

        var rows = new List<string[]>
        {
            new[] { "Ay", "Kapora Sayısı", "Tamamlanan İşlem", "Komisyon", "Prim", "Şirket Payı" }
        };

        var monthlyList = result.Data?.Finance?.Monthly ?? new List<DashboardMonthPointDto>();
        foreach (var m in monthlyList)
        {
            var monthLabel = $"{LeadStatusKeys.MonthName(m.Month)} {m.Year}";
            var companyShare = m.Commission - m.Shares;

            rows.Add(new[]
            {
                monthLabel,
                m.DepositCount.ToString(CultureInfo.InvariantCulture),
                m.CompletedCount.ToString(CultureInfo.InvariantCulture),
                FormatDecimal(m.Commission),
                FormatDecimal(m.Shares),
                FormatDecimal(companyShare)
            });
        }

        var bytes = BuildCsv(rows);
        var fileName = $"fibi-aylik-{SafeRangeSegment(baslangic)}-{SafeRangeSegment(bitis)}.csv";
        return File(bytes, "text/csv; charset=utf-8", fileName);
    }

    /// <summary>
    /// Çalışan performans raporunu CSV olarak dışa aktarır.
    /// </summary>
    [HttpGet("/raporlar/calisanlar.csv")]
    public async Task<IActionResult> AgentsCsv(string? baslangic, string? bitis)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var queryParams = BuildRangeQueryParams(baslangic, bitis);
        var apiPath = QueryHelpers.AddQueryString("AdminDashboard", queryParams);
        var result = await _apiClient.GetAsync<DashboardResponse>(apiPath);

        if (result.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }

        var rows = new List<string[]>
        {
            new[] { "Çalışan", "Atanan Talep", "Kazanılan Talep", "Tamamlanan İşlem", "Prim", "Ödenen", "Bekleyen" }
        };

        var agentList = result.Data?.Agents ?? new List<DashboardAgentDto>();
        foreach (var a in agentList)
        {
            rows.Add(new[]
            {
                a.AgentName,
                a.AssignedLeads.ToString(CultureInfo.InvariantCulture),
                a.WonLeads.ToString(CultureInfo.InvariantCulture),
                a.DealCount.ToString(CultureInfo.InvariantCulture),
                FormatDecimal(a.ShareTotal),
                FormatDecimal(a.PaidTotal),
                FormatDecimal(a.PendingTotal)
            });
        }

        var bytes = BuildCsv(rows);
        var fileName = $"fibi-calisanlar-{SafeRangeSegment(baslangic)}-{SafeRangeSegment(bitis)}.csv";
        return File(bytes, "text/csv; charset=utf-8", fileName);
    }

    /// <summary>
    /// İşlemler listesini CSV olarak dışa aktarır.
    /// </summary>
    [HttpGet("/raporlar/islemler.csv")]
    public async Task<IActionResult> DealsCsv(string? baslangic, string? bitis)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var allRows = new List<DealRowDto>();
        var page = 1;
        const int pageSize = 100;
        const int maxPages = 50;

        while (page <= maxPages)
        {
            var queryParams = new Dictionary<string, string?>
            {
                ["page"] = page.ToString(CultureInfo.InvariantCulture),
                ["pageSize"] = pageSize.ToString(CultureInfo.InvariantCulture)
            };

            if (!string.IsNullOrWhiteSpace(baslangic) &&
                DateTime.TryParseExact(baslangic.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                queryParams["from"] = baslangic.Trim();
            }

            if (!string.IsNullOrWhiteSpace(bitis) &&
                DateTime.TryParseExact(bitis.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                queryParams["to"] = bitis.Trim();
            }

            var apiPath = QueryHelpers.AddQueryString("AdminDeals", queryParams);
            var dealsResult = await _apiClient.GetAsync<DealListResponse>(apiPath);

            if (dealsResult.StatusCode == StatusCodes.Status401Unauthorized)
            {
                return Redirect("/giris");
            }

            if (dealsResult.StatusCode == StatusCodes.Status403Forbidden)
            {
                return Redirect("/yetkisiz");
            }

            if (!dealsResult.Success || dealsResult.Data == null)
            {
                break;
            }

            if (dealsResult.Data.Rows != null && dealsResult.Data.Rows.Count > 0)
            {
                allRows.AddRange(dealsResult.Data.Rows);
            }

            if (allRows.Count >= dealsResult.Data.Total || dealsResult.Data.Rows == null || dealsResult.Data.Rows.Count < pageSize)
            {
                break;
            }

            page++;
        }

        var rows = new List<string[]>
        {
            new[]
            {
                "İşlem No",
                "Tarih",
                "İlan No",
                "İlan",
                "Tür",
                "Durum",
                "Müşteri",
                "Anlaşılan Bedel",
                "Kapora",
                "Komisyon",
                "Prim",
                "Şirket Payı",
                "Prim Alanlar"
            }
        };

        foreach (var deal in allRows)
        {
            var shareAgents = deal.ShareAgentNames != null && deal.ShareAgentNames.Count > 0
                ? string.Join(", ", deal.ShareAgentNames)
                : "";

            rows.Add(new[]
            {
                deal.ListingDealId.ToString(CultureInfo.InvariantCulture),
                deal.DealDate.HasValue ? deal.DealDate.Value.ToString("dd.MM.yyyy") : "",
                deal.ListingId.ToString(CultureInfo.InvariantCulture),
                deal.ListingTitle,
                DealLabels.DealTypeName(deal.DealType),
                DealLabels.StatusName(deal.Status),
                deal.CustomerName ?? "",
                FormatDecimal(deal.AgreedPrice),
                FormatDecimal(deal.DepositAmount),
                FormatDecimal(deal.TotalCommission),
                FormatDecimal(deal.TotalShares),
                FormatDecimal(deal.CompanyShare),
                shareAgents
            });
        }

        var bytes = BuildCsv(rows);
        var fileName = $"fibi-islemler-{SafeRangeSegment(baslangic)}-{SafeRangeSegment(bitis)}.csv";
        return File(bytes, "text/csv; charset=utf-8", fileName);
    }

    private static Dictionary<string, string?> BuildRangeQueryParams(string? baslangic, string? bitis)
    {
        var queryParams = new Dictionary<string, string?>();

        if (!string.IsNullOrWhiteSpace(baslangic) &&
            DateTime.TryParseExact(baslangic.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
        {
            queryParams["from"] = baslangic.Trim();
        }

        if (!string.IsNullOrWhiteSpace(bitis) &&
            DateTime.TryParseExact(bitis.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
        {
            queryParams["to"] = bitis.Trim();
        }

        return queryParams;
    }

    private static string SafeRangeSegment(string? dateStr)
    {
        if (!string.IsNullOrWhiteSpace(dateStr) &&
            DateTime.TryParseExact(dateStr.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
        {
            return dateStr.Trim();
        }
        return "tum";
    }

    private static string FormatDecimal(decimal? value)
    {
        if (!value.HasValue)
        {
            return "";
        }

        var nfi = (NumberFormatInfo)TrCulture.NumberFormat.Clone();
        nfi.NumberGroupSeparator = "";
        return value.Value.ToString("0.##", nfi);
    }

    private static string EscapeCsvCell(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "\"\"";
        }

        var text = value;
        var isNumeric = decimal.TryParse(text, NumberStyles.Number, TrCulture, out _) ||
                        decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out _);

        if (!isNumeric && text.Length > 0)
        {
            var first = text[0];
            if (first is '=' or '+' or '-' or '@' or '\t' or '\r')
            {
                text = "'" + text;
            }
        }

        return "\"" + text.Replace("\"", "\"\"") + "\"";
    }

    private static byte[] BuildCsv(IEnumerable<string[]> rows)
    {
        var sb = new StringBuilder();
        foreach (var row in rows)
        {
            var escapedCells = row.Select(EscapeCsvCell);
            sb.Append(string.Join(";", escapedCells));
            sb.Append("\r\n");
        }

        var utf8Bom = Encoding.UTF8.GetPreamble();
        var contentBytes = Encoding.UTF8.GetBytes(sb.ToString());
        var result = new byte[utf8Bom.Length + contentBytes.Length];
        Buffer.BlockCopy(utf8Bom, 0, result, 0, utf8Bom.Length);
        Buffer.BlockCopy(contentBytes, 0, result, utf8Bom.Length, contentBytes.Length);
        return result;
    }
}
