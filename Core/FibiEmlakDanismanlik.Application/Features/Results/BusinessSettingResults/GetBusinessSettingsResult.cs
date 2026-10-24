using System;

namespace FibiEmlakDanismanlik.Application.Features.Results.BusinessSettingResults
{
    /// <summary>
    /// İş kuralı ayarları sorgu sonucu.
    /// </summary>
    public class GetBusinessSettingsResult
    {
        public decimal SaleBuyerCommissionRate { get; set; }
        public decimal SaleSellerCommissionRate { get; set; }
        public decimal RentCommissionMonths { get; set; }
        public int BestDealMaxPerUsageType { get; set; }
        public int ListingAlertMaxActivePerPhone { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public int? UpdatedByAgentId { get; set; }
    }
}
