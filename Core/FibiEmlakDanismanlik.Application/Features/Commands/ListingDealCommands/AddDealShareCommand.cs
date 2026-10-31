using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.ListingDealCommands
{
    /// <summary>
    /// İşleme yeni bir çalışan / danışman prim payı ekleme komutu.
    /// </summary>
    public class AddDealShareCommand : IRequest<int>
    {
        public int ListingDealId { get; set; }
        public int AgentId { get; set; }
        public decimal? SharePercent { get; set; }
        public decimal? Amount { get; set; }
        public string? Note { get; set; }
    }
}
