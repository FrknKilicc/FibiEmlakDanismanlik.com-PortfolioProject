using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using FibiEmlakDanismanlik.AdminUI.Infrastructure;
using FibiEmlakDanismanlik.AdminUI.Models.ListingAlerts;
using FibiEmlakDanismanlik.AdminUI.Models.Listings;

namespace FibiEmlakDanismanlik.AdminUI.Controllers;

/// <summary>
/// Haber bekleyen müşteri talepleri listesi, detayları, durum güncelleme, danışman atama ve ilan eşleşme işlemlerini yöneten denetleyici.
/// </summary>
[Authorize]
public sealed class ListingAlertsController : Controller
{
    private const int PageSize = 25;
    private readonly ApiClient _apiClient;
    private readonly CurrentAdmin _currentAdmin;

    public ListingAlertsController(ApiClient apiClient, CurrentAdmin currentAdmin)
    {
        _apiClient = apiClient;
        _currentAdmin = currentAdmin;
    }

    /// <summary>
    /// Yönetim paneli haber bekleyenler listesi sayfası.
    /// </summary>
    [HttpGet("/haber-bekleyenler")]
    public async Task<IActionResult> Index(
        int? durum,
        int? danisman,
        bool? atanmamis,
        bool bekleyen = false,
        string? ara = null,
        int sayfa = 1)
    {
        var pageNumber = Math.Max(1, sayfa);

        // Durum filtresi varsayılan olarak 1 (Aktif). "tumu" için durum=0 gönderilir.
        var effectiveDurum = durum ?? 1;

        // API sorgu parametrelerini oluştur
        var queryParams = new Dictionary<string, string?>
        {
            ["page"] = pageNumber.ToString(),
            ["pageSize"] = PageSize.ToString()
        };

        // Durum: 1 (Aktif), 2 (Kapandı); 0 veya diğer ise filtre uygulanmaz (tümü)
        if (effectiveDurum == 1 || effectiveDurum == 2)
        {
            queryParams["status"] = effectiveDurum.ToString();
        }

        // Danışman filtresi
        if (danisman.HasValue && danisman.Value > 0)
        {
            queryParams["agentId"] = danisman.Value.ToString();
        }

        // Sadece atanmamış talepler filtresi
        if (atanmamis.HasValue && atanmamis.Value)
        {
            queryParams["unassigned"] = "true";
        }

        // Yalnızca eşleşen bekleyen ilanı olanlar filtresi
        if (bekleyen)
        {
            queryParams["onlyWithPending"] = "true";
        }

        // Metin arama filtresi
        if (!string.IsNullOrWhiteSpace(ara))
        {
            queryParams["search"] = ara.Trim();
        }

        var apiPath = QueryHelpers.AddQueryString("AdminListingAlerts", queryParams);

        // Haber taleplerini ve Danışman lookup verilerini paralel olarak çağır
        var alertsTask = _apiClient.GetAsync<ListingAlertListResponse>(apiPath);
        var lookupsTask = _apiClient.GetAsync<AdminListingLookupsDto>("AdminListings/lookups");

        await Task.WhenAll(alertsTask, lookupsTask);

        var alertsResult = await alertsTask;
        var lookupsResult = await lookupsTask;

        // Yetkisiz erişim (401) durumunda giriş sayfasına yönlendir
        if (alertsResult.StatusCode == StatusCodes.Status401Unauthorized ||
            lookupsResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }

        // 403 Forbidden durumunda yetkisiz sayfasına yönlendir
        if (alertsResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        var viewModel = new ListingAlertIndexViewModel
        {
            Durum = effectiveDurum,
            DanismanId = danisman,
            Atanmamis = atanmamis,
            Bekleyen = bekleyen,
            Ara = ara,
            Sayfa = pageNumber
        };

        if (alertsResult.Success && alertsResult.Data != null)
        {
            viewModel.Response = alertsResult.Data;
        }
        else
        {
            viewModel.ErrorMessage = alertsResult.ErrorMessage ?? "Haber bekleyen talepler listelenirken bir hata oluştu.";
        }

        if (lookupsResult.Success && lookupsResult.Data != null)
        {
            viewModel.Agents = lookupsResult.Data.Agents;
        }
        else if (string.IsNullOrWhiteSpace(viewModel.ErrorMessage))
        {
            viewModel.ErrorMessage = lookupsResult.ErrorMessage ?? "Filtre verileri yüklenirken bir hata oluştu.";
        }

        return View(viewModel);
    }

    /// <summary>
    /// Haber bekleyen talep ayrıntı sayfası.
    /// </summary>
    [HttpGet("/haber-bekleyenler/{id:int}")]
    public async Task<IActionResult> Detail(int id)
    {
        var detailTask = _apiClient.GetAsync<ListingAlertDetailDto>($"AdminListingAlerts/{id}");
        var lookupsTask = _apiClient.GetAsync<AdminListingLookupsDto>("AdminListings/lookups");

        await Task.WhenAll(detailTask, lookupsTask);

        var detailResult = await detailTask;
        var lookupsResult = await lookupsTask;

        if (detailResult.StatusCode == StatusCodes.Status401Unauthorized ||
            lookupsResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }

        if (detailResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        if (detailResult.StatusCode == StatusCodes.Status404NotFound || detailResult.Data == null)
        {
            return NotFound();
        }

        var viewModel = new ListingAlertDetailViewModel
        {
            Detail = detailResult.Data,
            Agents = lookupsResult.Data?.Agents ?? new List<AgentLookupDto>(),
            CanAssign = _currentAdmin.IsManagerOrAbove,
            ErrorMessage = detailResult.Success ? null : detailResult.ErrorMessage
        };

        return View(viewModel);
    }

    /// <summary>
    /// Haber talebinin takip durumunu günceller (1: Aktif, 2: Kapandı).
    /// </summary>
    [HttpPost("/haber-bekleyenler/{id:int}/durum")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStatus(int id, int status, string? returnUrl)
    {
        var result = await _apiClient.PutAsync<ListingAlertDetailDto>(
            $"AdminListingAlerts/{id}/status",
            new { status });

        if (result.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }

        if (result.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        if (result.Success)
        {
            TempData["Success"] = status == 2
                ? "Talep başarıyla kapatıldı."
                : "Talep başarıyla yeniden açıldı.";
        }
        else
        {
            TempData["Error"] = result.ErrorMessage ?? "Talep durumu güncellenirken bir hata oluştu.";
        }

        return SafeRedirect(returnUrl, id);
    }

    /// <summary>
    /// Haber talebini bir danışmana atar veya atamasını kaldırır (Yalnızca Sahip ve Yönetici).
    /// </summary>
    [HttpPost("/haber-bekleyenler/{id:int}/ata")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Assign(int id, int? agentId, string? returnUrl)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var targetAgentId = (agentId.HasValue && agentId.Value > 0) ? agentId : null;
        var result = await _apiClient.PutAsync<bool>(
            $"AdminListingAlerts/{id}/assign",
            new { agentId = targetAgentId });

        if (result.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }

        if (result.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        if (result.Success)
        {
            TempData["Success"] = targetAgentId.HasValue
                ? "Danışman ataması başarıyla yapıldı."
                : "Danışman ataması kaldırıldı.";
        }
        else
        {
            TempData["Error"] = result.ErrorMessage ?? "Danışman ataması yapılırken bir hata oluştu.";
        }

        return SafeRedirect(returnUrl, id);
    }

    /// <summary>
    /// Haber talebi ile bir ilanın eşleşme/bildirim durumunu işaretler (1: Haber verildi, 2: Uygun değil).
    /// </summary>
    [HttpPost("/haber-bekleyenler/{id:int}/eslesme")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkMatch(int id, int listingTypeId, int listingId, int status, string? returnUrl)
    {
        var result = await _apiClient.PostAsync<bool>(
            $"AdminListingAlerts/{id}/matches",
            new { listingTypeId, listingId, status });

        if (result.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }

        if (result.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        if (result.Success)
        {
            TempData["Success"] = status == 1
                ? "İlan 'Haber verildi' olarak işaretlendi."
                : "İlan 'Uygun değil' olarak işaretlendi.";
        }
        else
        {
            TempData["Error"] = result.ErrorMessage ?? "Eşleşme durumu güncellenirken bir hata oluştu.";
        }

        return SafeRedirect(returnUrl, id);
    }

    /// <summary>
    /// Güvenli yerel adres yönlendirmesi yapar veya haber bekleyenler detay sayfasına yönlendirir.
    /// </summary>
    private IActionResult SafeRedirect(string? returnUrl, int id)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) &&
            Url.IsLocalUrl(returnUrl) &&
            returnUrl.StartsWith('/') &&
            !returnUrl.StartsWith("//"))
        {
            return Redirect(returnUrl);
        }

        return Redirect($"/haber-bekleyenler/{id}");
    }
}
