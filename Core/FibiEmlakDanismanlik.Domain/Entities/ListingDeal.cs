using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FibiEmlakDanismanlik.Domain.Enums;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    /// <summary>
    /// İlan satış ve kiralama işlem kaydı (kapora, komisyon ve tamamlama/iptal süreci).
    /// </summary>
    public class ListingDeal
    {
        [Key]
        public int ListingDealId { get; set; }

        /// <summary>
        /// İlan türü (1 satılık arsa, 2 satılık konut, 3 satılık işyeri, 7 kiralık arsa, 8 kiralık konut, 9 kiralık işyeri).
        /// </summary>
        public int ListingTypeId { get; set; }

        /// <summary>
        /// İlgili ilanın kimliği.
        /// </summary>
        public int ListingId { get; set; }

        /// <summary>
        /// İşlem türü (Satış / Kiralama).
        /// </summary>
        public DealType DealType { get; set; }

        /// <summary>
        /// İşlem durumu (Kapora alındı, Tamamlandı, İptal).
        /// </summary>
        public DealStatus Status { get; set; } = DealStatus.DepositReceived;

        /// <summary>
        /// Anlaşılan satış bedeli veya aylık kira bedeli.
        /// </summary>
        public decimal AgreedPrice { get; set; }

        /// <summary>
        /// Alınan kapora tutarı.
        /// </summary>
        public decimal? DepositAmount { get; set; }

        /// <summary>
        /// Kaporanın alındığı tarih (yalnızca gün).
        /// </summary>
        public DateTime? DepositDate { get; set; }

        /// <summary>
        /// Alıcıdan alınacak komisyon oranı (%) - Satışta isteğe bağlı, kiralamada kullanılmaz.
        /// </summary>
        public decimal? BuyerCommissionRate { get; set; }

        /// <summary>
        /// Satıcıdan alınacak komisyon oranı (%) - Satışta isteğe bağlı, kiralamada kullanılmaz.
        /// </summary>
        public decimal? SellerCommissionRate { get; set; }

        /// <summary>
        /// Alıcıdan (kiralamada kiracıdan) alınan komisyon tutarı.
        /// </summary>
        public decimal? BuyerCommissionAmount { get; set; }

        /// <summary>
        /// Satıcıdan (kiralamada mülk sahibinden) alınan komisyon tutarı.
        /// </summary>
        public decimal? SellerCommissionAmount { get; set; }

        /// <summary>
        /// İptal durumunda kaporanın akıbeti (yalnızca iptalde).
        /// </summary>
        public DepositOutcome? DepositOutcome { get; set; }

        /// <summary>
        /// İptal durumunda iade edilen kapora tutarı (yalnızca iptalde).
        /// </summary>
        public decimal? DepositRefundAmount { get; set; }

        /// <summary>
        /// Kapora iade tarihi (yalnızca iptalde).
        /// </summary>
        public DateTime? DepositRefundDate { get; set; }

        /// <summary>
        /// İşlemin tamamlanma tarihi.
        /// </summary>
        public DateTime? CompletedDate { get; set; }

        /// <summary>
        /// İşlemin iptal edilme tarihi.
        /// </summary>
        public DateTime? CancelledDate { get; set; }

        /// <summary>
        /// Müşteri adı soyadı (KVKK kapsamında kişisel veri).
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// Müşteri telefon numarası (KVKK kapsamında kişisel veri).
        /// </summary>
        [MaxLength(20)]
        public string? CustomerPhone { get; set; }

        /// <summary>
        /// İşlem ile ilgili genel notlar.
        /// </summary>
        [MaxLength(2000)]
        public string? Notes { get; set; }

        /// <summary>
        /// İşlem kaydını oluşturan personel / danışman kimliği.
        /// </summary>
        public int? CreatedByAgentId { get; set; }

        /// <summary>
        /// Kaydın oluşturulma tarihi (UTC).
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>
        /// Kaydın son güncellenme tarihi (UTC).
        /// </summary>
        public DateTime? UpdatedAtUtc { get; set; }

        /// <summary>
        /// Bu işlemden personele / danışmanlara düşen prim / hak ediş payları.
        /// </summary>
        public ICollection<DealShare> Shares { get; set; } = new List<DealShare>();

        /// <summary>
        /// Alıcı ve satıcıdan alınan toplam komisyon tutarı (hesaplanan, eşlenmeyen yardımcı alan).
        /// </summary>
        [NotMapped]
        public decimal TotalCommission => (BuyerCommissionAmount ?? 0) + (SellerCommissionAmount ?? 0);
    }
}
