using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;

namespace FibiEmlakDanismanlik.AdminUI.Infrastructure;

/// <summary>
/// Giriş akışındaki ara durumları (şifre değiştirme, 2FA kurulum/doğrulama) şifrelenmiş güvenli çerezde saklayan depo.
/// </summary>
public sealed class LoginChallengeStore
{
    public const string CookieName = "__Host-fibi-challenge";
    private const string Purpose = "FibiEmlak.AdminUI.Challenge";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IDataProtector _protector;

    public LoginChallengeStore(IDataProtectionProvider dataProtectionProvider)
    {
        _protector = dataProtectionProvider.CreateProtector(Purpose);
    }

    /// <summary>
    /// Ara durum bilgisini Data Protection ile şifreleyerek 10 dakikalık güvenli çereze kaydeder.
    /// </summary>
    public void Save(HttpContext context, LoginChallenge challenge)
    {
        if (context == null || challenge == null)
        {
            return;
        }

        var json = JsonSerializer.Serialize(challenge, JsonOptions);
        var encrypted = _protector.Protect(json);

        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddMinutes(10)
        };

        context.Response.Cookies.Append(CookieName, encrypted, options);
    }

    /// <summary>
    /// Güvenli çerezdeki ara durum bilgisini okur ve çözer. Çerez yoksa veya çözülemezse null döner.
    /// </summary>
    public LoginChallenge? Read(HttpContext context)
    {
        if (context == null)
        {
            return null;
        }

        if (!context.Request.Cookies.TryGetValue(CookieName, out var cookieValue) || string.IsNullOrWhiteSpace(cookieValue))
        {
            return null;
        }

        try
        {
            var json = _protector.Unprotect(cookieValue);
            return JsonSerializer.Deserialize<LoginChallenge>(json, JsonOptions);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Ara durum çerezini siler.
    /// </summary>
    public void Clear(HttpContext context)
    {
        if (context == null)
        {
            return;
        }

        context.Response.Cookies.Delete(CookieName, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/"
        });
    }
}

/// <summary>
/// Giriş adımları arasında taşınan geçici oturum ve doğrulama verisi.
/// </summary>
public sealed class LoginChallenge
{
    public string ChallengeToken { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? AgentName { get; set; }
    public string? ReturnUrl { get; set; }
    public string? TotpSecret { get; set; }
    public string? OtpAuthUri { get; set; }
}
