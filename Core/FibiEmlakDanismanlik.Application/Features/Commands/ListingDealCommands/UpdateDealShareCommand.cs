using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.ListingDealCommands
{
    /// <summary>
    /// İşleme ait mevcut bir prim payını güncelleme komutu.
    /// </summary>
    public class UpdateDealShareCommand : IRequest<bool>
    {
        public int DealShareId { get; set; }
        public decimal? SharePercent { get; set; }
        public decimal? Amount { get; set; }
        public string? Note { get; set; }
    }
}
