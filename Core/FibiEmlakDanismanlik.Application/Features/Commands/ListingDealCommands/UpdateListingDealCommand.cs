using System;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.ListingDealCommands
{
    /// <summary>
    /// Mevcut bir işlem kaydını güncelleme komutu.
    /// </summary>
    public class UpdateListingDealCommand : IRequest<bool>
    {
        public int ListingDealId { get; set; }
        public decimal AgreedPrice { get; set; }
        public decimal? DepositAmount { get; set; }
        public DateTime? DepositDate { get; set; }
        public decimal? BuyerCommissionRate { get; set; }
        public decimal? SellerCommissionRate { get; set; }
        public decimal? BuyerCommissionAmount { get; set; }
        public decimal? SellerCommissionAmount { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? CustomerPhone { get; set; }
        public string? Notes { get; set; }
    }
}
