using System.Globalization;
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

        return View(viewModel);
    }
}
