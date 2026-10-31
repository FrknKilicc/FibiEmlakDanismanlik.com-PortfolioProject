using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FibiEmlakDanismanlik.Domain.Enums;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    /// <summary>
    /// İşlemden çalışana / danışmana düşen prim / hak ediş payı.
    /// </summary>
    public class DealShare
    {
        [Key]
        public int DealShareId { get; set; }

        /// <summary>
        /// İlgili işlem kaydının kimliği.
        /// </summary>
        public int ListingDealId { get; set; }

        /// <summary>
        /// İlgili işlem kaydı gezinti özelliği.
        /// </summary>
        [ForeignKey(nameof(ListingDealId))]
        public ListingDeal ListingDeal { get; set; } = null!;

        /// <summary>
        /// Prim payı sahibi personel / danışman kimliği.
        /// </summary>
        public int AgentId { get; set; }

        /// <summary>
        /// Pay eklendiği andaki rol (rol sonradan değişse de geçmiş bozulmaz).
        /// </summary>
        public AgentRole RoleAtDeal { get; set; }

        /// <summary>
        /// Komisyondan pay yüzdesi (%), boşsa tutar elle girilmiştir.
        /// </summary>
        public decimal? SharePercent { get; set; }

        /// <summary>
        /// Prim tutarı.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Prim ödeme durumu (Bekliyor / Ödendi).
        /// </summary>
        public DealSharePaymentStatus PaymentStatus { get; set; } = DealSharePaymentStatus.Pending;

        /// <summary>
        /// Primin ödendiği tarih (yalnızca gün).
        /// </summary>
        public DateTime? PaidDate { get; set; }

        /// <summary>
        /// Pay ile ilgili açıklama notu.
        /// </summary>
        [MaxLength(500)]
        public string? Note { get; set; }

        /// <summary>
        /// Pay kaydının oluşturulma tarihi (UTC).
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }
    }
}
