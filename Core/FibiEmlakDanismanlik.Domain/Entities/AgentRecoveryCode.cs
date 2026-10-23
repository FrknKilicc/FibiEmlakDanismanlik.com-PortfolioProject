using System;
using System.ComponentModel.DataAnnotations;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    /// <summary>
    /// Danışmanın iki faktörlü doğrulamaya erişemediği durumlarda kullanabileceği tek kullanımlık kurtarma kodu.
    /// </summary>
    public class AgentRecoveryCode
    {
        [Key]
        public int AgentRecoveryCodeId { get; set; }

        /// <summary>
        /// Kodun ait olduğu danışman kimliği.
        /// </summary>
        public int AgentId { get; set; }

        /// <summary>
        /// BCrypt ile hash'lenmiş kurtarma kodu.
        /// </summary>
        public string CodeHash { get; set; } = string.Empty;

        /// <summary>
        /// Kodun oluşturulma UTC zamanı.
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>
        /// Kodun kullanıldığı UTC zamanı (kullanılmadıysa null).
        /// </summary>
        public DateTime? UsedAtUtc { get; set; }
    }
}
