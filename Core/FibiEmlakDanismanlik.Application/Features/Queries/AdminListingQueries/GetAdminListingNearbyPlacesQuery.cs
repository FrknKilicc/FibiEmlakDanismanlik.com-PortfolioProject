using FibiEmlakDanismanlik.Application.Features.Results.AdminListingResults;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Queries.AdminListingQueries
{
    /// <summary>
    /// İlanın yakın yer kayıtlarını ve seçilebilir kategorileri getiren sorgu.
    /// </summary>
    public class GetAdminListingNearbyPlacesQuery : IRequest<GetAdminListingNearbyPlacesResult>
    {
        public int ListingTypeId { get; set; }
        public int ListingId { get; set; }

        public GetAdminListingNearbyPlacesQuery()
        {
        }

        public GetAdminListingNearbyPlacesQuery(int listingTypeId, int listingId)
        {
            ListingTypeId = listingTypeId;
            ListingId = listingId;
        }
    }
}
