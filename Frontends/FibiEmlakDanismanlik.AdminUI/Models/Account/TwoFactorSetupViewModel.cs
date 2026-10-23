using System.ComponentModel.DataAnnotations;

namespace FibiEmlakDanismanlik.AdminUI.Models.Account;

/// <summary>
/// İki adımlı doğrulama (2FA) ilk kurulum görünüm modeli.
/// </summary>
public sealed class TwoFactorSetupViewModel
{
    /// <summary>
    /// QR kod görselinin Data URL (data:image/png;base64,...) gösterimi.
    /// </summary>
    public string? QrCodeBase64 { get; set; }

    /// <summary>
    /// QR kod okunamadığında elle girilecek gruplanmış gizli anahtar.
    /// </summary>
    public string? ManualSecret { get; set; }

    [Required(ErrorMessage = "Doğrulama kodu zorunludur.")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "Doğrulama kodu 6 haneli olmalıdır.")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "Doğrulama kodu yalnızca 6 rakamdan oluşmalıdır.")]
    [Display(Name = "Doğrulama Kodu")]
    public string Code { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}
