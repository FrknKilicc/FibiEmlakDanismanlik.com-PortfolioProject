using System.ComponentModel.DataAnnotations;

namespace FibiEmlakDanismanlik.AdminUI.Models.Account;

/// <summary>
/// İki adımlı doğrulama (2FA) giriş doğrulama görünüm modeli.
/// </summary>
public sealed class TwoFactorVerifyViewModel
{
    [RegularExpression(@"^\d{6}$", ErrorMessage = "Doğrulama kodu 6 rakamdan oluşmalıdır.")]
    [Display(Name = "Doğrulama Kodu")]
    public string? Code { get; set; }

    [Display(Name = "Kurtarma Kodu")]
    public string? RecoveryCode { get; set; }

    /// <summary>
    /// Kurtarma kodu formunun aktif olup olmadığını belirtir.
    /// </summary>
    public bool UseRecoveryCode { get; set; }

    public string? ReturnUrl { get; set; }
}
