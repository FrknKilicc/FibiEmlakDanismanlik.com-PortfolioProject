using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Domain.Enums;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public class Agent
    {
        [Key]
        public int AgentId { get; set; }

        [Required(ErrorMessage = "Personel Adı Giriniz")]
        public string AgentName { get; set; }

        [Display(Name = "Ünvan")]
        public string AgentTitle { get; set; }

        [Display(Name = "Açıklama")]
        [MaxLength(4000, ErrorMessage = "Açıklama en fazla 4000 karakter olabilir.")]
        public string AgentDescription { get; set; }

        [Display(Name = "Resim URL")]
        [Url(ErrorMessage = "Geçerli bir URL giriniz.")]
        public string AgentImgUrl { get; set; }

        [Display(Name = "E-posta")]
        [Required(ErrorMessage = "E-posta adresi giriniz.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Mail { get; set; }

        [Display(Name = "Telefon Numarası")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        public string AgentPhoneNumber { get; set; }

        [Display(Name = "Şifre")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        [System.Text.Json.Serialization.JsonIgnore]
        public string Password { get; set; }

        /// <summary>
        /// Kullanıcının panel üzerindeki yetki rolü (Sahip, Yönetici, Danışman).
        /// </summary>
        public AgentRole Role { get; set; } = AgentRole.Consultant;

        /// <summary>
        /// Sitede "Ekibimiz" sayfasında gösterilip gösterilmeyeceği.
        /// </summary>
        public bool ShowOnWebsite { get; set; } = true;

        /// <summary>
        /// Hesabın aktiflik durumu. Pasif hesaplar giriş yapamaz ve sitede görünmez.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Rol, şifre veya aktiflik değiştiğinde oturumları geçersiz kılan güvenlik damgası.
        /// </summary>
        public string SecurityStamp { get; set; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// Danışmanın tüm ilanları görüntüleme yetkisi (düzenleme yapamaz).
        /// </summary>
        public bool CanViewAllListings { get; set; }

        /// <summary>
        /// Kullanıcının ilk girişte veya sıfırlama sonrası şifre değiştirmesi gerekip gerekmediği.
        /// </summary>
        public bool MustChangePassword { get; set; }

        /// <summary>
        /// Ardışık başarısız giriş denemesi sayısı.
        /// </summary>
        public int FailedLoginCount { get; set; }

        /// <summary>
        /// Hesap kilitlemesinin biteceği UTC zamanı.
        /// </summary>
        public DateTime? LockoutEndUtc { get; set; }

        /// <summary>
        /// İki faktörlü doğrulamanın (2FA) aktif olup olmadığı.
        /// </summary>
        public bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// Data Protection ile şifrelenmiş TOTP gizli anahtarı.
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public string? TwoFactorSecretProtected { get; set; }

        /// <summary>
        /// Son başarılı giriş yapılan UTC zamanı.
        /// </summary>
        public DateTime? LastLoginAtUtc { get; set; }

        /// <summary>
        /// Şifrenin son değiştirildiği UTC zamanı.
        /// </summary>
        public DateTime? PasswordChangedAtUtc { get; set; }

    }
}
