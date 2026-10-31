using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.AdminUI.Infrastructure;
using FibiEmlakDanismanlik.AdminUI.Models.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.AdminUI.Controllers;

/// <summary>
/// İş kuralı ayarlarını ve sistem bakım işlemlerini yöneten denetleyici (Yalnızca Sahip).
/// </summary>
[Authorize]
public sealed class SettingsController : Controller
{
    private readonly ApiClient _apiClient;
    private readonly CurrentAdmin _currentAdmin;

    public SettingsController(ApiClient apiClient, CurrentAdmin currentAdmin)
    {
        _apiClient = apiClient;
        _currentAdmin = currentAdmin;
    }

    /// <summary>
    /// Ayarlar sayfası.
    /// </summary>
    [HttpGet("/ayarlar")]
    public async Task<IActionResult> Index()
    {
        if (!_currentAdmin.IsOwner)
        {
            return Redirect("/yetkisiz");
        }

        var result = await _apiClient.GetAsync<BusinessSettingsDto>("AdminSettings");
        if (result.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }
        if (result.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        var vm = new SettingsViewModel();
        if (result.Success && result.Data != null)
        {
            vm.Form = new BusinessSettingsForm
            {
                SaleBuyerCommissionRate = result.Data.SaleBuyerCommissionRate,
                SaleSellerCommissionRate = result.Data.SaleSellerCommissionRate,
                RentCommissionMonths = result.Data.RentCommissionMonths,
                BestDealMaxPerUsageType = result.Data.BestDealMaxPerUsageType,
                ListingAlertMaxActivePerPhone = result.Data.ListingAlertMaxActivePerPhone
            };
            vm.UpdatedAtUtc = result.Data.UpdatedAtUtc;
        }
        else
        {
            vm.ErrorMessage = "Ayarlar yüklenemedi.";
        }

        return View("Index", vm);
    }

    /// <summary>
    /// Ayarları günceller.
    /// </summary>
    [HttpPost("/ayarlar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(BusinessSettingsForm form)
    {
        if (!_currentAdmin.IsOwner)
        {
            return Redirect("/yetkisiz");
        }

        if (!ModelState.IsValid)
        {
            var fieldErrors = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
            foreach (var (key, entry) in ModelState)
            {
                if (entry != null && entry.Errors.Count > 0)
                {
                    var errorMessages = entry.Errors
                        .Select(e => e.ErrorMessage)
                        .Where(m => !string.IsNullOrWhiteSpace(m))
                        .ToArray();

                    if (errorMessages.Length > 0)
                    {
                        fieldErrors[key] = errorMessages;
                    }
                }
            }

            var invalidVm = new SettingsViewModel
            {
                Form = form,
                FieldErrors = fieldErrors
            };
            return View("Index", invalidVm);
        }

        var payload = new
        {
            saleBuyerCommissionRate = form.SaleBuyerCommissionRate,
            saleSellerCommissionRate = form.SaleSellerCommissionRate,
            rentCommissionMonths = form.RentCommissionMonths,
            bestDealMaxPerUsageType = form.BestDealMaxPerUsageType,
            listingAlertMaxActivePerPhone = form.ListingAlertMaxActivePerPhone
        };

        var result = await _apiClient.PutAsync<string>("AdminSettings", payload);

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
            TempData["Success"] = "Ayarlar kaydedildi.";
            return Redirect("/ayarlar");
        }

        if (result.StatusCode == StatusCodes.Status400BadRequest)
        {
            var badRequestVm = new SettingsViewModel
            {
                Form = form,
                FieldErrors = result.Errors != null
                    ? new Dictionary<string, string[]>(result.Errors, StringComparer.OrdinalIgnoreCase)
                    : new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase),
                ErrorMessage = result.ErrorMessage ?? "Lütfen formdaki hataları kontrol ediniz."
            };
            return View("Index", badRequestVm);
        }

        var errorVm = new SettingsViewModel
        {
            Form = form,
            ErrorMessage = result.ErrorMessage ?? "Ayarlar kaydedilemedi."
        };
        return View("Index", errorVm);
    }

    /// <summary>
    /// Kullanılmayan içerik görsellerini listeler (Yalnızca Sahip).
    /// </summary>
    [HttpGet("/ayarlar/kullanilmayan-gorseller")]
    public async Task<IActionResult> OrphanImages()
    {
        if (!_currentAdmin.IsOwner)
        {
            return Redirect("/yetkisiz");
        }

        var result = await _apiClient.GetAsync<GetOrphanContentImagesResultDto>("ContentImages/orphans");
        if (result.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }
        if (result.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        var vm = new OrphanImagesViewModel();
        if (result.Success && result.Data != null)
        {
            vm.Items = result.Data.Items;
            vm.TotalBytes = result.Data.TotalBytes;
            vm.ScannedCount = result.Data.ScannedCount;
        }
        else
        {
            vm.ErrorMessage = result.ErrorMessage ?? "Kullanılmayan görseller yüklenemedi.";
        }

        return View("OrphanImages", vm);
    }

    /// <summary>
    /// Seçilen kullanılmayan içerik görsellerini siler (Yalnızca Sahip).
    /// </summary>
    [HttpPost("/ayarlar/kullanilmayan-gorseller/sil")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteOrphanImages([FromForm] string[]? paths)
    {
        if (!_currentAdmin.IsOwner)
        {
            return Redirect("/yetkisiz");
        }

        if (paths == null || paths.Length == 0)
        {
            TempData["Error"] = "Lütfen silinecek en az bir görsel seçin.";
            return Redirect("/ayarlar/kullanilmayan-gorseller");
        }

        var selectedPaths = paths.Where(p => !string.IsNullOrWhiteSpace(p)).ToList();
        if (selectedPaths.Count == 0)
        {
            TempData["Error"] = "Lütfen silinecek en az bir görsel seçin.";
            return Redirect("/ayarlar/kullanilmayan-gorseller");
        }

        var payload = new
        {
            relativePaths = selectedPaths
        };

        var result = await _apiClient.PostAsync<DeleteOrphansResponseDto>("ContentImages/orphans/delete", payload);
        if (result.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }
        if (result.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        if (result.Success && result.Data != null)
        {
            TempData["Success"] = $"{result.Data.Deleted} kullanılmayan görsel silindi.";
        }
        else
        {
            TempData["Error"] = result.ErrorMessage ?? "Görseller silinirken bir hata oluştu.";
        }

        return Redirect("/ayarlar/kullanilmayan-gorseller");
    }
}
