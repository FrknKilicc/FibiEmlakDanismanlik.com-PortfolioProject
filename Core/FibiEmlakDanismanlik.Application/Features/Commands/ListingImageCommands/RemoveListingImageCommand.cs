using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.ListingImageCommands
{
    /// <summary>
    /// Belirtilen görsel kaydını ve yerel depolamadaki dosyalarını silme komutu.
    /// </summary>
    public class RemoveListingImageCommand : IRequest<bool>
    {
        /// <summary>
        /// Silinecek görsel birincil anahtar kimliği.
        /// </summary>
        public int ListingImageId { get; set; }

        public RemoveListingImageCommand()
        {
        }

        public RemoveListingImageCommand(int listingImageId)
        {
            ListingImageId = listingImageId;
        }
    }
}
