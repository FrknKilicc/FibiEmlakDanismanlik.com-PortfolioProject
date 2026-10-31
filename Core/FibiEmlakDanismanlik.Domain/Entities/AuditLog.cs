using System;
using System.ComponentModel.DataAnnotations;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    /// <summary>
    /// Sistemde gerçekleşen veri değişiklikleri ve yönetimsel eylemlerin denetim günlüğü kaydı.
    /// </summary>
    public class AuditLog
    {
        [Key]
        public int AuditLogId { get; set; }

        /// <summary>
        /// İşlemi gerçekleştiren danışman/kullanıcı kimliği.
        /// </summary>
        public int? AgentId { get; set; }

        /// <summary>
        /// Gerçekleştirilen eylem (Created, Updated, Deleted veya özel eylem adı).
        /// </summary>
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// Değişikliğe uğrayan varlığın adı.
        /// </summary>
        public string EntityName { get; set; } = string.Empty;

        /// <summary>
        /// Varlığın birincil anahtarı veya tanımlayıcısı.
        /// </summary>
        public string? EntityKey { get; set; }

        /// <summary>
        /// Değişiklik öncesi/sonrası verilerin JSON formatındaki özeti.
        /// </summary>
        public string? ChangesJson { get; set; }

        /// <summary>
        /// İstekte bulunan istemcinin IP adresi.
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// Eylemin gerçekleştiği UTC zamanı.
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }
    }
}
