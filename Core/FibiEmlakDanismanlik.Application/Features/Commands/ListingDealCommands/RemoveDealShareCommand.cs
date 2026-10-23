using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.ListingDealCommands
{
    /// <summary>
    /// İşleme ait bir prim payını silme komutu.
    /// </summary>
    public class RemoveDealShareCommand : IRequest<bool>
    {
        public int DealShareId { get; set; }

        public RemoveDealShareCommand()
        {
        }

        public RemoveDealShareCommand(int dealShareId)
        {
            DealShareId = dealShareId;
        }
    }
}
