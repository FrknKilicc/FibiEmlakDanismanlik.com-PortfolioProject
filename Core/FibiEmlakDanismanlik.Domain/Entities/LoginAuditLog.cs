using System;
using System.ComponentModel.DataAnnotations;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    /// <summary>
    /// Panele giriş denemelerinin denetim günlüğü kaydı.
    /// </summary>
    public class LoginAuditLog
    {
        [Key]
        public int LoginAuditLogId { get; set; }

        /// <summary>
        /// Giriş denenirken kullanılan e-posta adresi.
        /// </summary>
        public string AttemptedMail { get; set; } = string.Empty;

        /// <summary>
        /// Eşleşen danışman varsa kimliği.
        /// </summary>
        public int? AgentId { get; set; }

        /// <summary>
        /// İstekte bulunan istemcinin IP adresi.
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// İstekte bulunan istemcinin User-Agent başlığı.
        /// </summary>
        public string? UserAgent { get; set; }

        /// <summary>
        /// Giriş denemesinin başarılı olup olmadığı.
        /// </summary>
        public bool Succeeded { get; set; }

        /// <summary>
        /// Başarısızlık nedeni (ör. Hatalı şifre, kilitli hesap vb.).
        /// </summary>
        public string? FailureReason { get; set; }

        /// <summary>
        /// Giriş denemesinin gerçekleştiği UTC zamanı.
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }
    }
}
