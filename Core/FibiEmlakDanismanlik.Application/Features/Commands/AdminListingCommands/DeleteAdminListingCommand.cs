using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.AdminListingCommands
{
    /// <summary>
    /// İlanı, bağlı olanaklarını, koordinatlarını, erişim izinlerini, yakındaki yerlerini ve fotoğraflarını kalıcı olarak silme komutu (Yalnızca Sahip).
    /// </summary>
    public class DeleteAdminListingCommand : IRequest<bool>
    {
        public int ListingTypeId { get; set; }
        public int ListingId { get; set; }

        public DeleteAdminListingCommand()
        {
        }

        public DeleteAdminListingCommand(int listingTypeId, int listingId)
        {
            ListingTypeId = listingTypeId;
            ListingId = listingId;
        }
    }
}
