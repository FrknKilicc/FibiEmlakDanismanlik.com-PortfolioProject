using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.AdminUI.Infrastructure;
using FibiEmlakDanismanlik.AdminUI.Models.Content;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.AdminUI.Controllers;

/// <summary>
/// Sitedeki sabit başlık/metin/sayıların düzenlendiği ekran (Sahip ve Yönetici).
/// </summary>
[Authorize]
public sealed class SiteTextsController : Controller
{
    private readonly ApiClient _apiClient;
    private readonly CurrentAdmin _currentAdmin;

    public SiteTextsController(ApiClient apiClient, CurrentAdmin currentAdmin)
    {
        _apiClient = apiClient;
        _currentAdmin = currentAdmin;
    }

    /// <summary>
    /// Site metinleri sayfası.
    /// </summary>
    [HttpGet("/icerik/site-metinleri")]
    public async Task<IActionResult> Index()
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var result = await _apiClient.GetAsync<List<SiteTextDto>>("SiteTexts/admin");
        if (result.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }
        if (result.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        return View("Index", new SiteTextsViewModel
        {
            Items = result.Data ?? new List<SiteTextDto>(),
            ErrorMessage = result.Success ? null : (result.ErrorMessage ?? "Site metinleri yüklenemedi.")
        });
    }

    /// <summary>
    /// Site metinlerini kaydeder; form alanları sıralı "keys" ve "vals" dizileri olarak gelir.
    /// </summary>
    [HttpPost("/icerik/site-metinleri")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(string[]? keys, string?[]? vals)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        keys ??= Array.Empty<string>();
        vals ??= Array.Empty<string?>();
        var values = new Dictionary<string, string?>();
        for (var i = 0; i < keys.Length && i < vals.Length; i++)
        {
            values[keys[i]] = vals[i] ?? string.Empty;
        }

        var result = await _apiClient.PutAsync<string>("SiteTexts", new { values });

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
            TempData["Success"] = "Site metinleri kaydedildi. Değişiklikler sitede en geç 30 saniye içinde görünür.";
            return Redirect("/icerik/site-metinleri");
        }

        // Hata: girilen değerleri koruyarak sayfayı yeniden göster
        var reload = await _apiClient.GetAsync<List<SiteTextDto>>("SiteTexts/admin");
        var items = reload.Data ?? new List<SiteTextDto>();
        foreach (var item in items)
        {
            if (values.TryGetValue(item.Key, out var posted))
            {
                item.Value = posted ?? string.Empty;
            }
        }

        return View("Index", new SiteTextsViewModel
        {
            Items = items,
            ErrorMessage = result.ErrorMessage ?? "Site metinleri kaydedilemedi.",
            FieldErrors = result.Errors != null
                ? new Dictionary<string, string[]>(result.Errors, StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        });
    }
}
