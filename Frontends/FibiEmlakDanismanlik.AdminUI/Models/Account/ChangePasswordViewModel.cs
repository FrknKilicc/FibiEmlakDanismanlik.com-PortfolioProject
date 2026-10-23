using System.ComponentModel.DataAnnotations;

namespace FibiEmlakDanismanlik.AdminUI.Models.Account;

/// <summary>
/// Zorunlu şifre değiştirme formu görünüm modeli.
/// </summary>
public sealed class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Yeni şifre alanı zorunludur.")]
    [StringLength(100, MinimumLength = 12, ErrorMessage = "Şifreniz en az 12 karakter olmalıdır.")]
    [DataType(DataType.Password)]
    [Display(Name = "Yeni Şifre")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre tekrarı zorunludur.")]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "Şifreler eşleşmiyor.")]
    [Display(Name = "Yeni Şifre (Tekrar)")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}
