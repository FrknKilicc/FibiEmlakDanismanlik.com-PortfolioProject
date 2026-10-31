using System;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.ListingDealCommands
{
    /// <summary>
    /// Prim payının ödeme durumunu (Ödendi / Bekliyor) güncelleme komutu.
    /// </summary>
    public class SetDealSharePaymentCommand : IRequest<bool>
    {
        public int DealShareId { get; set; }
        public bool Paid { get; set; }
        public DateTime? PaidDate { get; set; }
    }
}
