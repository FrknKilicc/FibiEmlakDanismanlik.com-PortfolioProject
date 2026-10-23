using FibiEmlakDanismanlik.Application.Features.Results.AdminListingResults;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Queries.AdminListingQueries
{
    /// <summary>
    /// Belirli bir ilan türü ve kimliğine göre ilan detayını getiren MediatR sorgusu.
    /// </summary>
    public class GetAdminListingDetailQuery : IRequest<GetAdminListingDetailResult?>
    {
        public int ListingTypeId { get; set; }
        public int ListingId { get; set; }

        public GetAdminListingDetailQuery()
        {
        }

        public GetAdminListingDetailQuery(int listingTypeId, int listingId)
        {
            ListingTypeId = listingTypeId;
            ListingId = listingId;
        }
    }
}
