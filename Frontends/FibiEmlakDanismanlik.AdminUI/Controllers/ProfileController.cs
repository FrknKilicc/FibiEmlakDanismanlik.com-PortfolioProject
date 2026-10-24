using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.AdminUI.Infrastructure;
using FibiEmlakDanismanlik.AdminUI.Models.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.AdminUI.Controllers;

/// <summary>
/// Giriş yapmış çalışanın kendi hesap bilgilerini görüntüleme ve güncelleme denetleyicisi.
/// </summary>
[Authorize]
public sealed class ProfileController : Controller
{
    private readonly ApiClient _apiClient;

    public ProfileController(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    /// <summary>
    /// Hesabım ana sayfası. Sekme parametresine göre Profil / Güvenlik / Giriş Geçmişi görüntülenir.
    /// </summary>
    [HttpGet("/hesabim")]
    public async Task<IActionResult> Index(string? sekme)
    {
        var validSekme = sekme switch
        {
            "guvenlik" => "guvenlik",
            "giris-gecmisi" => "giris-gecmisi",
            _ => "profil"
        };

        var profileResult = await _apiClient.GetAsync<MyProfileDto>("Me");

        if (profileResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }
        if (profileResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        var vm = new ProfilePageViewModel
        {
            Sekme = validSekme
        };

        if (profileResult.Success && profileResult.Data != null)
        {
            vm.Profile = profileResult.Data;
            vm.ProfileForm = new ProfileForm
            {
                AgentName = profileResult.Data.AgentName,
                AgentPhoneNumber = profileResult.Data.AgentPhoneNumber,
                AgentTitle = profileResult.Data.AgentTitle,
                AgentDescription = profileResult.Data.AgentDescription,
                AgentImgUrl = profileResult.Data.AgentImgUrl
            };
        }
        else
        {
            vm.ErrorMessage = profileResult.ErrorMessage ?? "Profil bilgileri yüklenemedi.";
        }

        // Giriş geçmişi sekmesinde login kayıtlarını da çek
        if (validSekme == "giris-gecmisi")
        {
            var loginsResult = await _apiClient.GetAsync<List<MyLoginDto>>("Me/logins");
            if (loginsResult.Success && loginsResult.Data != null)
            {
                vm.Logins = loginsResult.Data;
            }
        }

        return View("Index", vm);
    }

    /// <summary>
    /// Profil bilgilerini günceller.
    /// </summary>
    [HttpPost("/hesabim/profil")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(ProfileForm form)
    {
        var payload = new
        {
            agentName = form.AgentName ?? string.Empty,
            agentPhoneNumber = form.AgentPhoneNumber ?? string.Empty,
            agentTitle = form.AgentTitle ?? string.Empty,
            agentDescription = form.AgentDescription ?? string.Empty,
            agentImgUrl = form.AgentImgUrl ?? string.Empty
        };

        var result = await _apiClient.PutAsync<object>("Me/profile", payload);

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
            TempData["Success"] = "Profiliniz güncellendi. Üst menüdeki ad bir sonraki girişte yenilenir.";
            return Redirect("/hesabim");
        }

        // 400 — alan hatalarını göster
        var vm = await BuildViewModelAsync("profil");
        vm.ProfileForm = form;
        vm.FormKey = "profil";

        if (result.StatusCode == StatusCodes.Status400BadRequest && result.Errors != null)
        {
            vm.FieldErrors = new Dictionary<string, string[]>(result.Errors, StringComparer.OrdinalIgnoreCase);
        }

        vm.ErrorMessage = result.ErrorMessage ?? "Profil güncellenirken bir hata oluştu.";
        return View("Index", vm);
    }

    /// <summary>
    /// Şifre değiştirir. Başarıda oturumu kapatıp giriş sayfasına yönlendirir.
    /// </summary>
    [HttpPost("/hesabim/sifre")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(PasswordForm form)
    {
        // İstemci tarafı kontrol: şifreler eşleşiyor mu?
        if (!string.Equals(form.NewPassword, form.ConfirmPassword, StringComparison.Ordinal))
        {
            var vm = await BuildViewModelAsync("guvenlik");
            vm.FormKey = "sifre";
            vm.FieldErrors = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
            {
                ["ConfirmPassword"] = new[] { "Yeni şifreler eşleşmiyor." }
            };
            return View("Index", vm);
        }

        var payload = new
        {
            currentPassword = form.CurrentPassword,
            newPassword = form.NewPassword
        };

        var result = await _apiClient.PostAsync<object>("Me/password", payload);

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
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["SuccessMessage"] = "Şifreniz değiştirildi. Lütfen yeni şifrenizle giriş yapın.";
            return Redirect("/giris");
        }

        // 400 — alan hatalarını göster
        var errorVm = await BuildViewModelAsync("guvenlik");
        errorVm.FormKey = "sifre";

        if (result.StatusCode == StatusCodes.Status400BadRequest && result.Errors != null)
        {
            errorVm.FieldErrors = new Dictionary<string, string[]>(result.Errors, StringComparer.OrdinalIgnoreCase);
        }

        errorVm.ErrorMessage = result.ErrorMessage ?? "Şifre değiştirilemedi.";
        return View("Index", errorVm);
    }

    /// <summary>
    /// Yeni kurtarma kodları üretir, kodları TempData'ya yazıp kurtarma kodları sayfasına yönlendirir.
    /// </summary>
    [HttpPost("/hesabim/kurtarma-kodlari")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegenerateRecoveryCodes(RecoveryForm form)
    {
        var payload = new
        {
            currentPassword = form.CurrentPassword
        };

        var result = await _apiClient.PostAsync<List<string>>("Me/recovery-codes", payload);

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
            TempData["RecoveryCodes"] = JsonSerializer.Serialize(result.Data);
            return Redirect("/kurtarma-kodlari");
        }

        // 400 — alan hatalarını göster
        var vm = await BuildViewModelAsync("guvenlik");
        vm.FormKey = "kurtarma";

        if (result.StatusCode == StatusCodes.Status400BadRequest && result.Errors != null)
        {
            vm.FieldErrors = new Dictionary<string, string[]>(result.Errors, StringComparer.OrdinalIgnoreCase);
        }

        vm.ErrorMessage = result.ErrorMessage ?? "Kurtarma kodları üretilemedi.";
        return View("Index", vm);
    }

    /// <summary>
    /// Tüm cihazlardaki oturumları kapatır, çıkış yapar, giriş sayfasına yönlendirir.
    /// </summary>
    [HttpPost("/hesabim/tum-oturumlar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SignOutEverywhere()
    {
        var result = await _apiClient.PostAsync<object>("Me/sign-out-everywhere", new { });

        if (result.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }

        // Başarılı veya başarısız fark etmeksizin çıkış yap
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["SuccessMessage"] = "Tüm cihazlardaki oturumlarınız kapatıldı. Lütfen tekrar giriş yapın.";
        return Redirect("/giris");
    }

    /// <summary>
    /// Hata durumlarında profil verisini yeniden çekerek ViewModel oluşturur.
    /// </summary>
    private async Task<ProfilePageViewModel> BuildViewModelAsync(string sekme)
    {
        var vm = new ProfilePageViewModel { Sekme = sekme };

        var profileResult = await _apiClient.GetAsync<MyProfileDto>("Me");
        if (profileResult.Success && profileResult.Data != null)
        {
            vm.Profile = profileResult.Data;
            vm.ProfileForm = new ProfileForm
            {
                AgentName = profileResult.Data.AgentName,
                AgentPhoneNumber = profileResult.Data.AgentPhoneNumber,
                AgentTitle = profileResult.Data.AgentTitle,
                AgentDescription = profileResult.Data.AgentDescription,
                AgentImgUrl = profileResult.Data.AgentImgUrl
            };
        }

        return vm;
    }
}
