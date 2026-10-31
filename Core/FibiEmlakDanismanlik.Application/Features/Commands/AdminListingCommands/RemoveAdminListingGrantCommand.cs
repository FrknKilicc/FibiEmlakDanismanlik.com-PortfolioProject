using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.AdminListingCommands
{
    /// <summary>
    /// Bir ilana ait özel erişim iznini kaldırma komutu (Yalnızca Sahip/Yönetici).
    /// </summary>
    public class RemoveAdminListingGrantCommand : IRequest<bool>
    {
        public int ListingAccessGrantId { get; set; }

        public RemoveAdminListingGrantCommand()
        {
        }

        public RemoveAdminListingGrantCommand(int listingAccessGrantId)
        {
            ListingAccessGrantId = listingAccessGrantId;
        }
    }
}
