using System.Collections.Generic;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.AdminListingCommands
{
    /// <summary>
    /// İlanın yakın yer listesini toptan değiştirme komutu.
    /// </summary>
    public class SetAdminListingNearbyPlacesCommand : IRequest<bool>
    {
        public int ListingTypeId { get; set; }
        public int ListingId { get; set; }
        public List<NearbyPlaceInput> Places { get; set; } = new();

        public SetAdminListingNearbyPlacesCommand()
        {
        }

        public SetAdminListingNearbyPlacesCommand(int listingTypeId, int listingId, List<NearbyPlaceInput>? places)
        {
            ListingTypeId = listingTypeId;
            ListingId = listingId;
            Places = places ?? new List<NearbyPlaceInput>();
        }
    }

    /// <summary>
    /// Yakın yer giriş satırı.
    /// </summary>
    public class NearbyPlaceInput
    {
        public int NearbyCategoryId { get; set; }
        public string PlaceName { get; set; } = string.Empty;
        public decimal DistanceKm { get; set; }
    }
}
