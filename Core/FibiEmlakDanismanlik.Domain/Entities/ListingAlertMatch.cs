using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FibiEmlakDanismanlik.Domain.Enums;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    /// <summary>
    /// Haber bekleyen talep ile eşleşen ilan hareket ve bildirim kaydı.
    /// </summary>
    public class ListingAlertMatch
    {
        [Key]
        public int ListingAlertMatchId { get; set; }

        public int ListingAlertId { get; set; }

        [ForeignKey(nameof(ListingAlertId))]
        public ListingAlert ListingAlert { get; set; } = null!;

        public int ListingTypeId { get; set; }

        public int ListingId { get; set; }

        /// <summary>
        /// Bildirim / eşleşme durumu (Haber verildi, Uygun değil).
        /// </summary>
        public ListingAlertMatchStatus Status { get; set; }

        /// <summary>
        /// İşlemi gerçekleştiren danışman kimliği.
        /// </summary>
        public int? AgentId { get; set; }

        /// <summary>
        /// Eşleşme veya bildirim işlem tarihi (UTC).
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }
    }
}
