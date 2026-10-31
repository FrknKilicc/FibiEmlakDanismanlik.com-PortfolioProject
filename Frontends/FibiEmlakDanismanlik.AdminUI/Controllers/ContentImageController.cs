using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.AdminUI.Infrastructure;
using FibiEmlakDanismanlik.AdminUI.Models.Content;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.AdminUI.Controllers;

/// <summary>
/// İçerik görsellerinin (blog, banner, hakkımızda, yazar, danışman) WebApi üzerinden yüklenmesini sağlayan denetleyici.
/// </summary>
[Authorize]
public sealed class ContentImageController : Controller
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp"
    };

    private static readonly HashSet<string> AllowedFolders = new(StringComparer.OrdinalIgnoreCase)
    {
        "blog", "banner", "about", "author", "agent"
    };

    private readonly ApiClient _apiClient;
    private readonly CurrentAdmin _currentAdmin;
    private readonly SiteImageUrl _siteImageUrl;

    public ContentImageController(
        ApiClient apiClient,
        CurrentAdmin currentAdmin,
        SiteImageUrl siteImageUrl)
    {
        _apiClient = apiClient;
        _currentAdmin = currentAdmin;
        _siteImageUrl = siteImageUrl;
    }

    [HttpPost("/icerik/gorsel-yukle")]
    [RequestSizeLimit(16 * 1024 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = 16 * 1024 * 1024)]
    public async Task<IActionResult> Upload(string? klasor, IFormFile? dosya)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { ok = false, message = "Bu işlem için yetkiniz bulunmamaktadır." });
        }

        if (dosya == null || dosya.Length == 0)
        {
            return BadRequest(new { ok = false, message = "Lütfen bir görsel seçin." });
        }

        var extension = Path.GetExtension(dosya.FileName);
        if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
        {
            return BadRequest(new { ok = false, message = $"Desteklenmeyen dosya türü: {dosya.FileName}. Sadece JPG, PNG veya WebP yükleyebilirsiniz." });
        }

        if (string.IsNullOrWhiteSpace(klasor) || !AllowedFolders.Contains(klasor))
        {
            return BadRequest(new { ok = false, message = "Geçersiz görsel klasörü." });
        }

        var result = await _apiClient.PostMultipartAsync<ContentImageUploadDto>(
            $"ContentImages?folder={Uri.EscapeDataString(klasor)}",
            new[] { dosya },
            "file");

        if (result.Success)
        {
            return Ok(new
            {
                ok = true,
                url = result.Data?.Url,
                previewUrl = _siteImageUrl.Preview(result.Data?.Url)
            });
        }

        return StatusCode(
            result.StatusCode == 0 ? 500 : result.StatusCode,
            new { ok = false, message = result.ErrorMessage ?? "Görsel yüklenemedi." });
    }
}
