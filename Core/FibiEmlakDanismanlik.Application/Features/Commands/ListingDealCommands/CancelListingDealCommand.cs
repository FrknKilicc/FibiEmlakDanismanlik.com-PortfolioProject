using System;
using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.ListingDealCommands
{
    /// <summary>
    /// Kapora aşamasındaki işlemi iptal etme komutu.
    /// </summary>
    public class CancelListingDealCommand : IRequest<bool>
    {
        public int ListingDealId { get; set; }
        public DateTime? CancelledDate { get; set; }
        public DepositOutcome DepositOutcome { get; set; } = DepositOutcome.Kept;
        public decimal? DepositRefundAmount { get; set; }
        public DateTime? DepositRefundDate { get; set; }
    }
}
