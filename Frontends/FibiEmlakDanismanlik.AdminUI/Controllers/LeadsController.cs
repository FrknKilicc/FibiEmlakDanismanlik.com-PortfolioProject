using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using FibiEmlakDanismanlik.AdminUI.Infrastructure;
using FibiEmlakDanismanlik.AdminUI.Models.Leads;
using FibiEmlakDanismanlik.AdminUI.Models.Listings;

namespace FibiEmlakDanismanlik.AdminUI.Controllers;

/// <summary>
/// Müşteri talepleri listesi, detayları, durum güncelleme, danışman atama ve not işlemlerini yöneten denetleyici.
/// </summary>
[Authorize]
public sealed class LeadsController : Controller
{
    private const int PageSize = 25;
    private readonly ApiClient _apiClient;
    private readonly CurrentAdmin _currentAdmin;

    public LeadsController(ApiClient apiClient, CurrentAdmin currentAdmin)
    {
        _apiClient = apiClient;
        _currentAdmin = currentAdmin;
    }

    /// <summary>
    /// Yönetim paneli müşteri talepleri listesi sayfası.
    /// </summary>
    [HttpGet("/talepler")]
    public async Task<IActionResult> Index(
        string? kaynak,
        int? durum,
        int? danisman,
        bool? atanmamis,
        string? ara,
        int sayfa = 1)
    {
        var pageNumber = Math.Max(1, sayfa);

        // API sorgu parametrelerini oluştur
        var queryParams = new Dictionary<string, string?>
        {
            ["page"] = pageNumber.ToString(),
            ["pageSize"] = PageSize.ToString()
        };

        // Kaynak filtresi: "iletisim" -> 1, "fibi" -> 2
        if (string.Equals(kaynak, "iletisim", StringComparison.OrdinalIgnoreCase))
        {
            queryParams["source"] = "1";
        }
        else if (string.Equals(kaynak, "fibi", StringComparison.OrdinalIgnoreCase))
        {
            queryParams["source"] = "2";
        }

        // Durum filtresi (1-7)
        if (durum.HasValue && durum.Value >= 1 && durum.Value <= 7)
        {
            queryParams["status"] = durum.Value.ToString();
        }

        // Danışman filtresi (Yalnızca Sahip/Yönetici için veya geçerli id varsa)
        if (danisman.HasValue && danisman.Value > 0)
        {
            queryParams["agentId"] = danisman.Value.ToString();
        }

        // Sadece atanmamış talepler filtresi
        if (atanmamis.HasValue && atanmamis.Value)
        {
            queryParams["unassigned"] = "true";
        }

        // Metin arama filtresi
        if (!string.IsNullOrWhiteSpace(ara))
        {
            queryParams["search"] = ara.Trim();
        }

        var apiPath = QueryHelpers.AddQueryString("AdminLeads", queryParams);

        // Talepleri ve Danışman lookup verilerini paralel olarak çağır
        var leadsTask = _apiClient.GetAsync<LeadListResponse>(apiPath);
        var lookupsTask = _apiClient.GetAsync<AdminListingLookupsDto>("AdminListings/lookups");

        await Task.WhenAll(leadsTask, lookupsTask);

        var leadsResult = await leadsTask;
        var lookupsResult = await lookupsTask;

        // Yetkisiz erişim (401) durumunda giriş sayfasına yönlendir
        if (leadsResult.StatusCode == StatusCodes.Status401Unauthorized ||
            lookupsResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }

        // 403 Forbidden durumunda yetkisiz sayfasına yönlendir
        if (leadsResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        var viewModel = new LeadIndexViewModel
        {
            Kaynak = kaynak,
            Durum = durum,
            DanismanId = danisman,
            Atanmamis = atanmamis,
            Ara = ara,
            Sayfa = pageNumber
        };

        if (leadsResult.Success && leadsResult.Data != null)
        {
            viewModel.Response = leadsResult.Data;
        }
        else
        {
            viewModel.ErrorMessage = leadsResult.ErrorMessage ?? "Talepler listelenirken bir hata oluştu.";
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
    /// Müşteri talebi ayrıntı sayfası.
    /// </summary>
    [HttpGet("/talepler/{kaynak}/{id:int}")]
    public async Task<IActionResult> Detail(string kaynak, int id)
    {
        if (!IsValidSource(kaynak))
        {
            return NotFound();
        }

        var detailTask = _apiClient.GetAsync<LeadDetailDto>($"AdminLeads/{kaynak.ToLowerInvariant()}/{id}");
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

        var viewModel = new LeadDetailViewModel
        {
            Detail = detailResult.Data,
            Agents = lookupsResult.Data?.Agents ?? new List<AgentLookupDto>(),
            CanAssign = _currentAdmin.IsManagerOrAbove,
            ErrorMessage = detailResult.Success ? null : detailResult.ErrorMessage
        };

        return View(viewModel);
    }

    /// <summary>
    /// Müşteri talebinin durumunu günceller.
    /// </summary>
    [HttpPost("/talepler/{kaynak}/{id:int}/durum")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStatus(string kaynak, int id, int status, string? note, string? returnUrl)
    {
        if (!IsValidSource(kaynak))
        {
            return NotFound();
        }

        var result = await _apiClient.PutAsync<bool>(
            $"AdminLeads/{kaynak.ToLowerInvariant()}/{id}/status",
            new { status, note });

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
            TempData["Success"] = "Talep durumu başarıyla güncellendi.";
        }
        else
        {
            TempData["Error"] = result.ErrorMessage ?? "Talep durumu güncellenirken bir hata oluştu.";
        }

        return SafeRedirect(returnUrl, kaynak, id);
    }

    /// <summary>
    /// Müşteri talebine yeni danışman notu veya görüşme kaydı ekler.
    /// </summary>
    [HttpPost("/talepler/{kaynak}/{id:int}/not")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddNote(string kaynak, int id, string? text, bool isCall, string? returnUrl)
    {
        if (!IsValidSource(kaynak))
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            TempData["Error"] = "Lütfen bir not metni girin.";
            return SafeRedirect(returnUrl, kaynak, id);
        }

        var result = await _apiClient.PostAsync<bool>(
            $"AdminLeads/{kaynak.ToLowerInvariant()}/{id}/notes",
            new { text = text.Trim(), isCall });

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
            TempData["Success"] = isCall ? "Görüşme kaydı başarıyla eklendi." : "Not başarıyla eklendi.";
        }
        else
        {
            TempData["Error"] = result.ErrorMessage ?? "Not eklenirken bir hata oluştu.";
        }

        return SafeRedirect(returnUrl, kaynak, id);
    }

    /// <summary>
    /// Müşteri talebini bir danışmana atar veya atamasını kaldırır (Yalnızca Sahip ve Yönetici).
    /// </summary>
    [HttpPost("/talepler/{kaynak}/{id:int}/ata")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Assign(string kaynak, int id, int? agentId, string? returnUrl)
    {
        if (!IsValidSource(kaynak))
        {
            return NotFound();
        }

        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var targetAgentId = (agentId.HasValue && agentId.Value > 0) ? agentId : null;
        var result = await _apiClient.PutAsync<bool>(
            $"AdminLeads/{kaynak.ToLowerInvariant()}/{id}/assign",
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

        return SafeRedirect(returnUrl, kaynak, id);
    }

    /// <summary>
    /// Güvenli yerel adres yönlendirmesi yapar veya detay sayfasına yönlendirir.
    /// </summary>
    private IActionResult SafeRedirect(string? returnUrl, string kaynak, int id)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) &&
            Url.IsLocalUrl(returnUrl) &&
            returnUrl.StartsWith('/') &&
            !returnUrl.StartsWith("//"))
        {
            return Redirect(returnUrl);
        }

        return Redirect($"/talepler/{kaynak.ToLowerInvariant()}/{id}");
    }

    /// <summary>
    /// Kaynak parametresinin "contact" veya "service" olup olmadığını doğrular.
    /// </summary>
    private static bool IsValidSource(string source) =>
        string.Equals(source, "contact", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(source, "service", StringComparison.OrdinalIgnoreCase);
}
