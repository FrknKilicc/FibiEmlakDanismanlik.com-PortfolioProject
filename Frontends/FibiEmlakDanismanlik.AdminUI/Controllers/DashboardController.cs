using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using FibiEmlakDanismanlik.AdminUI.Infrastructure;
using FibiEmlakDanismanlik.AdminUI.Models.Dashboard;

namespace FibiEmlakDanismanlik.AdminUI.Controllers;

/// <summary>
/// Yönetim paneli ana kontrol paneli (Dashboard) ekranını sunan denetleyici.
/// </summary>
[Authorize]
public sealed class DashboardController : Controller
{
    private readonly ApiClient _apiClient;
    private readonly CurrentAdmin _currentAdmin;

    public DashboardController(ApiClient apiClient, CurrentAdmin currentAdmin)
    {
        _apiClient = apiClient;
        _currentAdmin = currentAdmin;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? baslangic, string? bitis)
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

        var apiPath = QueryHelpers.AddQueryString("AdminDashboard", queryParams);
        var result = await _apiClient.GetAsync<DashboardResponse>(apiPath);

        if (result.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }

        var viewModel = new DashboardViewModel
        {
            Baslangic = baslangic,
            Bitis = bitis,
            CanSeeFinance = result.Data?.CanSeeFinance ?? _currentAdmin.IsManagerOrAbove
        };

        if (result.Success && result.Data != null)
        {
            viewModel.Data = result.Data;
            viewModel.CanSeeFinance = result.Data.CanSeeFinance;
        }
        else
        {
            viewModel.ErrorMessage = result.ErrorMessage ?? "Kontrol paneli verileri yüklenirken bir hata oluştu.";
        }

        if (_currentAdmin.IsManagerOrAbove)
        {
            try
            {
                var syncLogsResult = await _apiClient.GetAsync<List<SyncLogDto>>("FibiServices/sync/logs");
                if (syncLogsResult.Success && syncLogsResult.Data != null)
                {
                    viewModel.SyncFailures = syncLogsResult.Data
                        .Where(x => !string.IsNullOrWhiteSpace(x.ProviderKey))
                        .GroupBy(x => x.ProviderKey, StringComparer.OrdinalIgnoreCase)
                        .Select(g => g.OrderByDescending(x => x.FinishedAt).ThenByDescending(x => x.MarketDataSyncLogId).First())
                        .Where(latest => !latest.IsSuccess)
                        .Select(latest => new SyncFailureItem
                        {
                            ProviderKey = latest.ProviderKey,
                            FinishedAt = latest.FinishedAt,
                            Message = latest.Message ?? string.Empty
                        })
                        .ToList();
                }
            }
            catch
            {
                // Uç hata verirse sessizce geç (kontrol paneli bozulmasın)
            }
        }

        return View(viewModel);
    }
}
