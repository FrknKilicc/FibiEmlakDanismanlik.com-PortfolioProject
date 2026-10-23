using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.AdminListingCommands
{
    /// <summary>
    /// İlanın harita konum koordinatlarını (enlem, boylam) manuel olarak güncelleme komutu.
    /// </summary>
    public class SetAdminListingGeoPointCommand : IRequest<bool>
    {
        public int ListingTypeId { get; set; }
        public int ListingId { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public SetAdminListingGeoPointCommand()
        {
        }

        public SetAdminListingGeoPointCommand(int listingTypeId, int listingId, decimal latitude, decimal longitude)
        {
            ListingTypeId = listingTypeId;
            ListingId = listingId;
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
