using System;
using System.ComponentModel.DataAnnotations;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    /// <summary>
    /// Sistem genelinde geçerli tek satırlık iş kuralı ayarları.
    /// </summary>
    public class BusinessSetting
    {
        /// <summary>
        /// Ayar kaydı birincil anahtarı (tek satır, Id = 1).
        /// </summary>
        [Key]
        public int BusinessSettingId { get; set; }

        /// <summary>
        /// Satış işlemlerinde varsayılan alıcı komisyon oranı (%).
        /// </summary>
        public decimal SaleBuyerCommissionRate { get; set; }

        /// <summary>
        /// Satış işlemlerinde varsayılan satıcı komisyon oranı (%).
        /// </summary>
        public decimal SaleSellerCommissionRate { get; set; }

        /// <summary>
        /// Kiralama işlemlerinde varsayılan komisyon süresi (kira ayı cinsinden).
        /// </summary>
        public decimal RentCommissionMonths { get; set; }

        /// <summary>
        /// Satılık ve kiralık vitrinde ayrı ayrı gösterilebilecek azami ilan sayısı.
        /// </summary>
        public int BestDealMaxPerUsageType { get; set; }

        /// <summary>
        /// Haber bekleyenler formunda aynı telefon için izin verilen azami aktif talep sayısı.
        /// </summary>
        public int ListingAlertMaxActivePerPhone { get; set; }

        /// <summary>
        /// Ayarların son güncellendiği UTC zamanı.
        /// </summary>
        public DateTime? UpdatedAtUtc { get; set; }

        /// <summary>
        /// Ayarları son güncelleyen danışman kimliği.
        /// </summary>
        public int? UpdatedByAgentId { get; set; }
    }
}
