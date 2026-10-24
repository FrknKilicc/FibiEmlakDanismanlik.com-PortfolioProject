using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.BusinessSettingCommands
{
    /// <summary>
    /// İş kuralı ayarlarını güncelleme komutu.
    /// </summary>
    public class UpdateBusinessSettingsCommand : IRequest
    {
        public decimal SaleBuyerCommissionRate { get; set; }
        public decimal SaleSellerCommissionRate { get; set; }
        public decimal RentCommissionMonths { get; set; }
        public int BestDealMaxPerUsageType { get; set; }
        public int ListingAlertMaxActivePerPhone { get; set; }
    }
}
