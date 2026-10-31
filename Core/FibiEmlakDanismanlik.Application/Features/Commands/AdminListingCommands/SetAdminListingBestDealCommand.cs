using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.AdminListingCommands
{
    /// <summary>
    /// İlanın ana sayfa vitrin (BestDeals) durumunu açıp kapatma komutu (Yalnızca Sahip/Yönetici).
    /// </summary>
    public class SetAdminListingBestDealCommand : IRequest<bool>
    {
        public int ListingTypeId { get; set; }
        public int ListingId { get; set; }
        public bool IsBestDeal { get; set; }

        public SetAdminListingBestDealCommand()
        {
        }

        public SetAdminListingBestDealCommand(int listingTypeId, int listingId, bool isBestDeal)
        {
            ListingTypeId = listingTypeId;
            ListingId = listingId;
            IsBestDeal = isBestDeal;
        }
    }
}
