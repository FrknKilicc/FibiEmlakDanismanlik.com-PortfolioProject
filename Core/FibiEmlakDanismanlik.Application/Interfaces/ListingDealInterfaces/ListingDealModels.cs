using System;
using System.Collections.Generic;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Domain.Enums;

namespace FibiEmlakDanismanlik.Application.Interfaces.ListingDealInterfaces
{
    /// <summary>
    /// İşlem ve hak ediş (prim) kayıtları arama ve filtreleme parametreleri.
    /// </summary>
    public class ListingDealFilter
    {
        /// <summary>
        /// İşlem durumu filtresi (Kapora alındı, Tamamlandı, İptal).
        /// </summary>
        public DealStatus? Status { get; set; }

        /// <summary>
        /// İşlem türü filtresi (Satış / Kiralama).
        /// </summary>
        public DealType? DealType { get; set; }

        /// <summary>
        /// Pay verilmiş danışman kimliği filtresi.
        /// </summary>
        public int? AgentId { get; set; }

        /// <summary>
        /// İşlem başlangıç tarihi filtresi (gün başlangıcı dahil).
        /// </summary>
        public DateTime? From { get; set; }

        /// <summary>
        /// İşlem bitiş tarihi filtresi (gün sonu dahil).
        /// </summary>
        public DateTime? To { get; set; }

        /// <summary>
        /// Müşteri adı, telefon numarası veya ilan numarası arama terimi.
        /// </summary>
        public string? Search { get; set; }

        /// <summary>
        /// Sayfa numarası (1 tabanlı).
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Sayfa başına kayıt sayısı.
        /// </summary>
        public int PageSize { get; set; } = ListingDealConstants.DefaultPageSize;
    }

    /// <summary>
    /// Satış ve kiralama işlem listesi satır görünüm modeli.
    /// </summary>
    public class ListingDealRow
    {
        public int ListingDealId { get; set; }
        public int ListingTypeId { get; set; }
        public int ListingId { get; set; }
        public string? ListingTitle { get; set; }
        public DealType DealType { get; set; }
        public DealStatus Status { get; set; }
        public decimal AgreedPrice { get; set; }
        public decimal? DepositAmount { get; set; }
        public decimal TotalCommission { get; set; }
        public decimal TotalShares { get; set; }
        public decimal CompanyShare { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime DealDate { get; set; }
        public List<string> ShareAgentNames { get; set; } = new List<string>();
    }

    /// <summary>
    /// İşlem ve prim istatistikleri ve durum özeti.
    /// </summary>
    public class ListingDealSummary
    {
        /// <summary>
        /// Kapora aşamasındaki işlem sayısı.
        /// </summary>
        public int DepositReceivedCount { get; set; }

        /// <summary>
        /// Tamamlanmış işlem sayısı.
        /// </summary>
        public int CompletedCount { get; set; }

        /// <summary>
        /// İptal edilmiş işlem sayısı.
        /// </summary>
        public int CancelledCount { get; set; }

        /// <summary>
        /// Tamamlanan işlemlerden kazanılan toplam komisyon tutarı.
        /// </summary>
        public decimal CompletedCommissionTotal { get; set; }

        /// <summary>
        /// Tamamlanan işlemlerden personele/danışmanlara dağıtılan toplam prim tutarı.
        /// </summary>
        public decimal CompletedSharesTotal { get; set; }

        /// <summary>
        /// Tamamlanan işlemlerden ofise kalan net pay toplamı (CompletedCommissionTotal - CompletedSharesTotal).
        /// </summary>
        public decimal CompanyShareTotal { get; set; }

        /// <summary>
        /// İptal edilmemiş işlemlerde henüz ödenmemiş (bekleyen) primlerin toplamı.
        /// </summary>
        public decimal PendingSharesTotal { get; set; }

        /// <summary>
        /// İptal edilip kaporası ofiste kalan işlemlerin kapora toplamı.
        /// </summary>
        public decimal KeptDepositTotal { get; set; }
    }

    /// <summary>
    /// İşleme ait çalışan / danışman prim payı modeli.
    /// </summary>
    public class DealShareItem
    {
        public int DealShareId { get; set; }
        public int AgentId { get; set; }
        public string AgentName { get; set; } = string.Empty;
        public AgentRole RoleAtDeal { get; set; }
        public decimal? SharePercent { get; set; }
        public decimal Amount { get; set; }
        public DealSharePaymentStatus PaymentStatus { get; set; }
        public DateTime? PaidDate { get; set; }
        public string? Note { get; set; }
    }

    /// <summary>
    /// İşlem kaydının ayrıntılı görünümü, tüm skaler alanlar ve prim payları.
    /// </summary>
    public class ListingDealDetail
    {
        public int ListingDealId { get; set; }
        public int ListingTypeId { get; set; }
        public int ListingId { get; set; }
        public DealType DealType { get; set; }
        public DealStatus Status { get; set; }
        public decimal AgreedPrice { get; set; }
        public decimal? DepositAmount { get; set; }
        public DateTime? DepositDate { get; set; }
        public decimal? BuyerCommissionRate { get; set; }
        public decimal? SellerCommissionRate { get; set; }
        public decimal? BuyerCommissionAmount { get; set; }
        public decimal? SellerCommissionAmount { get; set; }
        public DepositOutcome? DepositOutcome { get; set; }
        public decimal? DepositRefundAmount { get; set; }
        public DateTime? DepositRefundDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime? CancelledDate { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? CustomerPhone { get; set; }
        public string? Notes { get; set; }
        public int? CreatedByAgentId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public string? ListingTitle { get; set; }
        public string? ListingStatus { get; set; }
        public string? CreatedByAgentName { get; set; }
        public decimal TotalCommission { get; set; }
        public decimal TotalShares { get; set; }
        public decimal CompanyShare { get; set; }
        public List<DealShareItem> Shares { get; set; } = new List<DealShareItem>();
    }
}
