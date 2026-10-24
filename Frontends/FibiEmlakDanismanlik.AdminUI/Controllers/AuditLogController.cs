using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using FibiEmlakDanismanlik.AdminUI.Infrastructure;
using FibiEmlakDanismanlik.AdminUI.Models.Users;

namespace FibiEmlakDanismanlik.AdminUI.Controllers;

/// <summary>
/// Sistem denetim kayıtlarını (Audit Logs) görüntüleyen denetleyici.
/// </summary>
[Authorize]
public sealed class AuditLogController : Controller
{
    private const int PageSize = 50;
    private readonly ApiClient _apiClient;
    private readonly CurrentAdmin _currentAdmin;

    public AuditLogController(ApiClient apiClient, CurrentAdmin currentAdmin)
    {
        _apiClient = apiClient;
        _currentAdmin = currentAdmin;
    }

    /// <summary>
    /// Denetim kayıtları listesi sayfası.
    /// </summary>
    [HttpGet("/denetim-kaydi")]
    public async Task<IActionResult> Index(string? tablo, int? kullanici, int sayfa = 1)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var pageNumber = Math.Max(1, sayfa);

        var queryParams = new Dictionary<string, string?>
        {
            ["page"] = pageNumber.ToString(CultureInfo.InvariantCulture),
            ["pageSize"] = PageSize.ToString(CultureInfo.InvariantCulture)
        };

        if (!string.IsNullOrWhiteSpace(tablo))
        {
            queryParams["entityName"] = tablo.Trim();
        }

        if (kullanici.HasValue && kullanici.Value > 0)
        {
            queryParams["agentId"] = kullanici.Value.ToString(CultureInfo.InvariantCulture);
        }

        var apiPath = QueryHelpers.AddQueryString("AuditLogs", queryParams);

        var auditTask = _apiClient.GetAsync<AuditLogResponse>(apiPath);
        var agentsTask = _apiClient.GetAsync<List<AgentAdminDto>>("Agent/admin");

        await Task.WhenAll(auditTask, agentsTask);

        var auditResult = await auditTask;
        var agentsResult = await agentsTask;

        if (auditResult.StatusCode == StatusCodes.Status401Unauthorized ||
            agentsResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }

        if (auditResult.StatusCode == StatusCodes.Status403Forbidden ||
            agentsResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        var vm = new AuditLogViewModel
        {
            Data = auditResult.Data,
            Tablo = tablo,
            Kullanici = kullanici,
            Sayfa = pageNumber,
            Agents = agentsResult.Data ?? new List<AgentAdminDto>(),
            ErrorMessage = auditResult.Success ? null : (auditResult.ErrorMessage ?? "Denetim kayıtları yüklenirken bir hata oluştu.")
        };

        if (auditResult.Data != null && auditResult.Data.PageSize > 0)
        {
            vm.TotalPages = (int)Math.Ceiling((double)auditResult.Data.TotalCount / auditResult.Data.PageSize);
        }

        return View("Index", vm);
    }
}
