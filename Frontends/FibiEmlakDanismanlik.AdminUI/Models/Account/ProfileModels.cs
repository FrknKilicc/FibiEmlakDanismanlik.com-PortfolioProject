using System;
using System.Collections.Generic;

namespace FibiEmlakDanismanlik.AdminUI.Models.Account;

/// <summary>
/// GET Me API yanıtının panel tarafı DTO'su.
/// </summary>
public sealed class MyProfileDto
{
    public int AgentId { get; set; }
    public string AgentName { get; set; } = string.Empty;
    public string Mail { get; set; } = string.Empty;
    public string AgentPhoneNumber { get; set; } = string.Empty;
    public string? AgentTitle { get; set; }
    public string? AgentDescription { get; set; }
    public string? AgentImgUrl { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool TwoFactorEnabled { get; set; }
    public int UnusedRecoveryCodeCount { get; set; }
    public DateTime? LastLoginAtUtc { get; set; }
    public DateTime? PasswordChangedAtUtc { get; set; }
}

/// <summary>
/// GET Me/logins API yanıtının panel tarafı DTO'su.
/// </summary>
public sealed class MyLoginDto
{
    public DateTime CreatedAtUtc { get; set; }
    public bool Succeeded { get; set; }
    public string? FailureReason { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}

/// <summary>
/// Profil güncelleme form modeli.
/// </summary>
public sealed class ProfileForm
{
    public string AgentName { get; set; } = string.Empty;
    public string AgentPhoneNumber { get; set; } = string.Empty;
    public string? AgentTitle { get; set; }
    public string? AgentDescription { get; set; }
    public string? AgentImgUrl { get; set; }
}

/// <summary>
/// Şifre değiştirme form modeli.
/// </summary>
public sealed class PasswordForm
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

/// <summary>
/// Kurtarma kodları üretme form modeli.
/// </summary>
public sealed class RecoveryForm
{
    public string CurrentPassword { get; set; } = string.Empty;
}

/// <summary>
/// Hesabım sayfası birleşik görünüm modeli.
/// </summary>
public sealed class ProfilePageViewModel
{
    public string Sekme { get; set; } = "profil";
    public MyProfileDto? Profile { get; set; }
    public ProfileForm ProfileForm { get; set; } = new();
    public List<MyLoginDto> Logins { get; set; } = new();
    public string? FormKey { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, string[]> FieldErrors { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Yalnızca FormKey eşleştiğinde ilgili alan hatasını döndürür.
    /// </summary>
    public string? GetFieldError(string formKey, string field)
    {
        if (!string.Equals(FormKey, formKey, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        if (FieldErrors.TryGetValue(field, out var errors) && errors.Length > 0)
        {
            return errors[0];
        }

        return null;
    }
}

/// <summary>
/// Giriş geçmişi tablo etiketleri.
/// </summary>
public static class LoginLabels
{
    /// <summary>
    /// API'den dönen başarısızlık nedenini Türkçe kullanıcı metnine çevirir.
    /// </summary>
    public static string FailureText(string? reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            return "";
        }

        return reason switch
        {
            "InvalidCredentials" => "Hatalı e-posta veya şifre",
            "LockedOut" => "Hesap kilitli",
            "Inactive" => "Pasif hesap",
            "InvalidTwoFactor" => "Hatalı doğrulama kodu",
            "InvalidChallenge" => "Oturum süresi doldu",
            _ => reason
        };
    }

    /// <summary>
    /// User-Agent dizesinden tarayıcı + işletim sistemi kısa metnini üretir.
    /// </summary>
    public static string ShortUserAgent(string? ua)
    {
        if (string.IsNullOrWhiteSpace(ua))
        {
            return "—";
        }

        var browser = "Bilinmeyen";
        var os = "";

        // Tarayıcı tespiti (sıralama önemli: Edge önce, sonra Chrome)
        if (ua.Contains("Edg/", StringComparison.OrdinalIgnoreCase) ||
            ua.Contains("Edge/", StringComparison.OrdinalIgnoreCase))
        {
            browser = "Edge";
        }
        else if (ua.Contains("Firefox/", StringComparison.OrdinalIgnoreCase))
        {
            browser = "Firefox";
        }
        else if (ua.Contains("Safari/", StringComparison.OrdinalIgnoreCase) &&
                 !ua.Contains("Chrome/", StringComparison.OrdinalIgnoreCase))
        {
            browser = "Safari";
        }
        else if (ua.Contains("Chrome/", StringComparison.OrdinalIgnoreCase) ||
                 ua.Contains("CriOS/", StringComparison.OrdinalIgnoreCase))
        {
            browser = "Chrome";
        }

        // İşletim sistemi tespiti
        if (ua.Contains("Windows", StringComparison.OrdinalIgnoreCase))
        {
            os = "Windows";
        }
        else if (ua.Contains("Android", StringComparison.OrdinalIgnoreCase))
        {
            os = "Android";
        }
        else if (ua.Contains("iPhone", StringComparison.OrdinalIgnoreCase))
        {
            os = "iPhone";
        }
        else if (ua.Contains("Mac", StringComparison.OrdinalIgnoreCase))
        {
            os = "Mac";
        }

        if (!string.IsNullOrEmpty(os))
        {
            return $"{browser} / {os}";
        }

        if (browser != "Bilinmeyen")
        {
            return browser;
        }

        return ua.Length > 40 ? ua[..40] + "…" : ua;
    }
}
