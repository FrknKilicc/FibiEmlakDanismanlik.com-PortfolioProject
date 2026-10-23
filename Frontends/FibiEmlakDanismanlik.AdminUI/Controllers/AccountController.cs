using System.Text.Json;
using FibiEmlakDanismanlik.AdminUI.Infrastructure;
using FibiEmlakDanismanlik.AdminUI.Models.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCoder;

namespace FibiEmlakDanismanlik.AdminUI.Controllers;

/// <summary>
/// Yönetici kimlik doğrulama, şifre değiştirme ve iki adımlı doğrulama (2FA) akışlarını yöneten denetleyici.
/// </summary>
[AllowAnonymous]
public sealed class AccountController : Controller
{
    private readonly ApiClient _apiClient;
    private readonly LoginChallengeStore _challengeStore;
    private readonly AdminSignInService _signInService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        ApiClient apiClient,
        LoginChallengeStore challengeStore,
        AdminSignInService signInService,
        ILogger<AccountController> logger)
    {
        _apiClient = apiClient;
        _challengeStore = challengeStore;
        _signInService = signInService;
        _logger = logger;
    }

    /// <summary>
    /// Yönetici giriş sayfasını görüntüler. Oturum açıksa ana sayfaya yönlendirir.
    /// </summary>
    [HttpGet("/giris")]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToLocal(returnUrl);
        }

        if (TempData["ErrorMessage"] is string errorMessage)
        {
            ModelState.AddModelError(string.Empty, errorMessage);
        }

        var model = new LoginViewModel
        {
            ReturnUrl = returnUrl
        };

        return View(model);
    }

    /// <summary>
    /// Giriş formunu işler. Cloudflare Turnstile token'ını ve kimlik bilgilerini WebApi'ye iletir.
    /// </summary>
    [HttpPost("/giris")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var captchaToken = Request.Form["cf-turnstile-response"].ToString();

        var requestBody = new
        {
            mail = model.Mail.Trim(),
            password = model.Password,
            captchaToken = string.IsNullOrWhiteSpace(captchaToken) ? null : captchaToken
        };

        var response = await _apiClient.PostAnonymousAsync<LoginResponse>("Auth/login", requestBody);

        if (!response.Success)
        {
            if (response.StatusCode == StatusCodes.Status429TooManyRequests)
            {
                ModelState.AddModelError(string.Empty, "Çok fazla deneme yaptınız. Lütfen bir dakika sonra tekrar deneyin.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "E-posta veya şifre hatalı ya da doğrulama tamamlanamadı.");
            }

            return View(model);
        }

        var data = response.Data;
        if (data == null)
        {
            ModelState.AddModelError(string.Empty, "Giriş işlemi sırasında beklenmeyen bir hata oluştu.");
            return View(model);
        }

        return await ProcessLoginResultAsync(data, model.ReturnUrl);
    }

    /// <summary>
    /// İlk girişte veya süresi dolduğunda zorunlu şifre değiştirme sayfasını görüntüler.
    /// </summary>
    [HttpGet("/sifre-degistir")]
    public IActionResult ChangePassword()
    {
        var challenge = _challengeStore.Read(HttpContext);
        if (challenge == null || challenge.Status != "PasswordChangeRequired" || string.IsNullOrWhiteSpace(challenge.ChallengeToken))
        {
            return RedirectToAction(nameof(Login));
        }

        var model = new ChangePasswordViewModel
        {
            ReturnUrl = challenge.ReturnUrl
        };

        return View(model);
    }

    /// <summary>
    /// Zorunlu şifre değişikliğini WebApi'ye iletir ve oturum akışını devam ettirir.
    /// </summary>
    [HttpPost("/sifre-degistir")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        var challenge = _challengeStore.Read(HttpContext);
        if (challenge == null || challenge.Status != "PasswordChangeRequired" || string.IsNullOrWhiteSpace(challenge.ChallengeToken))
        {
            TempData["ErrorMessage"] = "Oturum süresi doldu. Lütfen tekrar giriş yapın.";
            return RedirectToAction(nameof(Login));
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var requestBody = new
        {
            challengeToken = challenge.ChallengeToken,
            newPassword = model.NewPassword
        };

        var response = await _apiClient.PostAnonymousAsync<LoginResponse>("Auth/change-password", requestBody);

        if (!response.Success)
        {
            if (response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                _challengeStore.Clear(HttpContext);
                TempData["ErrorMessage"] = "Oturum süresi doldu. Lütfen tekrar giriş yapın.";
                return RedirectToAction(nameof(Login));
            }

            ModelState.AddModelError(string.Empty, response.ErrorMessage ?? "Şifre değiştirme işlemi başarısız oldu.");
            return View(model);
        }

        var data = response.Data;
        if (data == null)
        {
            ModelState.AddModelError(string.Empty, "Şifre değiştirme yanıtı geçersiz.");
            return View(model);
        }

        return await ProcessLoginResultAsync(data, challenge.ReturnUrl);
    }

    /// <summary>
    /// İki adımlı doğrulama ilk kurulum ekranını gösterir. QR kod ve elle giriş anahtarı üretir.
    /// </summary>
    [HttpGet("/iki-adimli-kurulum")]
    public async Task<IActionResult> TwoFactorSetup()
    {
        var challenge = _challengeStore.Read(HttpContext);
        if (challenge == null || challenge.Status != "TwoFactorSetupRequired" || string.IsNullOrWhiteSpace(challenge.ChallengeToken))
        {
            return RedirectToAction(nameof(Login));
        }

        // Eğer daha önce üretilmiş secret varsa yeniden isteme, aynı secret'ı göster
        if (string.IsNullOrWhiteSpace(challenge.TotpSecret) || string.IsNullOrWhiteSpace(challenge.OtpAuthUri))
        {
            var setupResponse = await _apiClient.PostAnonymousAsync<TwoFactorSetupResponse>(
                "Auth/2fa/setup",
                new { challengeToken = challenge.ChallengeToken });

            if (!setupResponse.Success || setupResponse.Data == null ||
                string.IsNullOrWhiteSpace(setupResponse.Data.Secret) ||
                string.IsNullOrWhiteSpace(setupResponse.Data.OtpAuthUri))
            {
                _challengeStore.Clear(HttpContext);
                TempData["ErrorMessage"] = setupResponse.ErrorMessage ?? "İki adımlı doğrulama kurulumu başlatılamadı.";
                return RedirectToAction(nameof(Login));
            }

            challenge.TotpSecret = setupResponse.Data.Secret;
            challenge.OtpAuthUri = setupResponse.Data.OtpAuthUri;
            _challengeStore.Save(HttpContext, challenge);
        }

        var model = new TwoFactorSetupViewModel
        {
            QrCodeBase64 = GenerateQrCodeDataUrl(challenge.OtpAuthUri),
            ManualSecret = FormatSecret(challenge.TotpSecret),
            ReturnUrl = challenge.ReturnUrl
        };

        return View(model);
    }

    /// <summary>
    /// Authenticator uygulamasından alınan 6 haneli TOTP kodunu doğrulayarak 2FA'yı aktif eder.
    /// </summary>
    [HttpPost("/iki-adimli-kurulum")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TwoFactorSetup(TwoFactorSetupViewModel model)
    {
        var challenge = _challengeStore.Read(HttpContext);
        if (challenge == null || challenge.Status != "TwoFactorSetupRequired" || string.IsNullOrWhiteSpace(challenge.ChallengeToken))
        {
            TempData["ErrorMessage"] = "Oturum süresi doldu. Lütfen tekrar giriş yapın.";
            return RedirectToAction(nameof(Login));
        }

        if (!ModelState.IsValid)
        {
            PopulateSetupViewModel(model, challenge);
            return View(model);
        }

        var requestBody = new
        {
            challengeToken = challenge.ChallengeToken,
            code = model.Code.Trim()
        };

        var response = await _apiClient.PostAnonymousAsync<TwoFactorEnableResponse>("Auth/2fa/enable", requestBody);

        if (!response.Success)
        {
            PopulateSetupViewModel(model, challenge);
            ModelState.AddModelError(nameof(model.Code), "Kod hatalı, uygulamadaki güncel kodu girin.");
            return View(model);
        }

        var data = response.Data;
        var login = data?.Login;
        if (login == null || string.IsNullOrWhiteSpace(login.Token))
        {
            PopulateSetupViewModel(model, challenge);
            ModelState.AddModelError(string.Empty, "İki adımlı doğrulama onaylandı fakat oturum başlatılamadı.");
            return View(model);
        }

        // Başarılı: Giriş yap, challenge'ı temizle ve kurtarma kodlarını TempData'ya aktar
        var expireDate = login.ExpireDate ?? DateTime.UtcNow.AddHours(1);
        var agentName = login.AgentName ?? challenge.AgentName ?? "Yönetici";
        var role = login.Role ?? "Agent";

        await _signInService.SignInAsync(HttpContext, login.Token, expireDate, agentName, role);
        _challengeStore.Clear(HttpContext);

        TempData["RecoveryCodes"] = JsonSerializer.Serialize(data?.RecoveryCodes ?? new List<string>());
        return RedirectToAction(nameof(RecoveryCodes));
    }

    /// <summary>
    /// 2FA etkinleştirildiğinde tek seferlik gösterilen kurtarma kodları ekranı.
    /// </summary>
    [Authorize]
    [HttpGet("/kurtarma-kodlari")]
    public IActionResult RecoveryCodes()
    {
        var codesJson = TempData["RecoveryCodes"] as string;
        if (string.IsNullOrWhiteSpace(codesJson))
        {
            return Redirect("~/");
        }

        List<string>? codes;
        try
        {
            codes = JsonSerializer.Deserialize<List<string>>(codesJson);
        }
        catch (JsonException)
        {
            codes = null;
        }

        if (codes == null || codes.Count == 0)
        {
            return Redirect("~/");
        }

        return View(codes);
    }

    /// <summary>
    /// İki adımlı doğrulama zorunlu olan yöneticiler için 6 haneli kod veya kurtarma kodu giriş ekranı.
    /// </summary>
    [HttpGet("/iki-adimli-dogrulama")]
    public IActionResult TwoFactorVerify()
    {
        var challenge = _challengeStore.Read(HttpContext);
        if (challenge == null || challenge.Status != "TwoFactorRequired" || string.IsNullOrWhiteSpace(challenge.ChallengeToken))
        {
            return RedirectToAction(nameof(Login));
        }

        var model = new TwoFactorVerifyViewModel
        {
            ReturnUrl = challenge.ReturnUrl
        };

        return View(model);
    }

    /// <summary>
    /// TOTP kodunu veya kurtarma kodunu doğrular.
    /// </summary>
    [HttpPost("/iki-adimli-dogrulama")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TwoFactorVerify(TwoFactorVerifyViewModel model)
    {
        var challenge = _challengeStore.Read(HttpContext);
        if (challenge == null || challenge.Status != "TwoFactorRequired" || string.IsNullOrWhiteSpace(challenge.ChallengeToken))
        {
            TempData["ErrorMessage"] = "Oturum süresi doldu. Lütfen tekrar giriş yapın.";
            return RedirectToAction(nameof(Login));
        }

        if (model.UseRecoveryCode)
        {
            ModelState.Remove(nameof(model.Code));
            if (string.IsNullOrWhiteSpace(model.RecoveryCode))
            {
                ModelState.AddModelError(nameof(model.RecoveryCode), "Kurtarma kodu zorunludur.");
                return View(model);
            }
        }
        else
        {
            ModelState.Remove(nameof(model.RecoveryCode));
            if (string.IsNullOrWhiteSpace(model.Code) || model.Code.Trim().Length != 6)
            {
                ModelState.AddModelError(nameof(model.Code), "Doğrulama kodu 6 rakamdan oluşmalıdır.");
                return View(model);
            }
        }

        object requestBody = model.UseRecoveryCode
            ? new { challengeToken = challenge.ChallengeToken, recoveryCode = model.RecoveryCode?.Trim() }
            : new { challengeToken = challenge.ChallengeToken, code = model.Code?.Trim() };

        var response = await _apiClient.PostAnonymousAsync<LoginResponse>("Auth/2fa/verify", requestBody);

        if (!response.Success)
        {
            ModelState.AddModelError(string.Empty, "Kod hatalı. Üst üste 3 hatalı denemeden sonra hesabınız geçici olarak kilitlenebilir.");
            return View(model);
        }

        var data = response.Data;
        if (data == null || string.IsNullOrWhiteSpace(data.Token))
        {
            ModelState.AddModelError(string.Empty, "Oturum bilgileri alınamadı.");
            return View(model);
        }

        _challengeStore.Clear(HttpContext);

        var expireDate = data.ExpireDate ?? DateTime.UtcNow.AddHours(1);
        var agentName = data.AgentName ?? challenge.AgentName ?? "Yönetici";
        var role = data.Role ?? "Agent";

        await _signInService.SignInAsync(HttpContext, data.Token, expireDate, agentName, role);

        return RedirectToLocal(challenge.ReturnUrl);
    }

    /// <summary>
    /// Güvenli çıkış yapar ve oturum çerezlerini temizler.
    /// </summary>
    [Authorize]
    [HttpPost("/cikis")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        _challengeStore.Clear(HttpContext);
        return RedirectToAction(nameof(Login));
    }

    /// <summary>
    /// API yanıtındaki Status durumuna göre oturum açar veya ilgili ara adıma yönlendirir.
    /// </summary>
    private async Task<IActionResult> ProcessLoginResultAsync(LoginResponse data, string? returnUrl)
    {
        switch (data.Status)
        {
            case "Success":
            {
                if (string.IsNullOrWhiteSpace(data.Token))
                {
                    ModelState.AddModelError(string.Empty, "Yetkilendirme belirteci alınamadı.");
                    return View(new LoginViewModel { ReturnUrl = returnUrl });
                }

                _challengeStore.Clear(HttpContext);
                var expireDate = data.ExpireDate ?? DateTime.UtcNow.AddHours(1);
                var agentName = data.AgentName ?? "Yönetici";
                var role = data.Role ?? "Agent";

                await _signInService.SignInAsync(HttpContext, data.Token, expireDate, agentName, role);
                return RedirectToLocal(returnUrl);
            }

            case "PasswordChangeRequired":
            {
                _challengeStore.Save(HttpContext, new LoginChallenge
                {
                    ChallengeToken = data.ChallengeToken ?? string.Empty,
                    Status = data.Status,
                    AgentName = data.AgentName,
                    ReturnUrl = returnUrl
                });
                return RedirectToAction(nameof(ChangePassword));
            }

            case "TwoFactorSetupRequired":
            {
                _challengeStore.Save(HttpContext, new LoginChallenge
                {
                    ChallengeToken = data.ChallengeToken ?? string.Empty,
                    Status = data.Status,
                    AgentName = data.AgentName,
                    ReturnUrl = returnUrl
                });
                return RedirectToAction(nameof(TwoFactorSetup));
            }

            case "TwoFactorRequired":
            {
                _challengeStore.Save(HttpContext, new LoginChallenge
                {
                    ChallengeToken = data.ChallengeToken ?? string.Empty,
                    Status = data.Status,
                    AgentName = data.AgentName,
                    ReturnUrl = returnUrl
                });
                return RedirectToAction(nameof(TwoFactorVerify));
            }

            default:
            {
                _logger.LogWarning("Bilinmeyen oturum durumu alındı: {Status}", data.Status);
                ModelState.AddModelError(string.Empty, "Tanımlanamayan oturum durumu.");
                return View(new LoginViewModel { ReturnUrl = returnUrl });
            }
        }
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return Redirect("~/");
    }

    private static void PopulateSetupViewModel(TwoFactorSetupViewModel model, LoginChallenge challenge)
    {
        if (!string.IsNullOrWhiteSpace(challenge.OtpAuthUri))
        {
            model.QrCodeBase64 = GenerateQrCodeDataUrl(challenge.OtpAuthUri);
        }

        if (!string.IsNullOrWhiteSpace(challenge.TotpSecret))
        {
            model.ManualSecret = FormatSecret(challenge.TotpSecret);
        }

        model.ReturnUrl = challenge.ReturnUrl;
    }

    private static string GenerateQrCodeDataUrl(string otpAuthUri)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(otpAuthUri, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        var bytes = qrCode.GetGraphic(20);
        return $"data:image/png;base64,{Convert.ToBase64String(bytes)}";
    }

    private static string FormatSecret(string? secret)
    {
        if (string.IsNullOrWhiteSpace(secret))
        {
            return string.Empty;
        }

        var clean = secret.Replace(" ", "").ToUpperInvariant();
        var chunks = new List<string>();
        for (var i = 0; i < clean.Length; i += 4)
        {
            var length = Math.Min(4, clean.Length - i);
            chunks.Add(clean.Substring(i, length));
        }

        return string.Join(" ", chunks);
    }
}
