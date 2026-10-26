using System;
using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.ListingDealCommands
{
    /// <summary>
    /// Yeni satış veya kiralama işlem kaydı (kapora, komisyon) oluşturma komutu.
    /// </summary>
    public class CreateListingDealCommand : IRequest<int>
    {
        public int ListingTypeId { get; set; }
        public int ListingId { get; set; }
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
        public LeadSource? LinkedLeadSource { get; set; }
        public int? LinkedLeadId { get; set; }
    }
}
