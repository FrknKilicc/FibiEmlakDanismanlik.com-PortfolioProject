using FibiEmlakDanismanlik.Application.Features.Results.ListingAlertResults;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Queries.ListingAlertQueries
{
    /// <summary>
    /// Belirli bir ilanın kriterlerine uyan aktif haber taleplerini getiren sorgu.
    /// </summary>
    public class GetListingAlertsForListingQuery : IRequest<GetListingAlertsForListingResult>
    {
        public int ListingTypeId { get; set; }
        public int ListingId { get; set; }

        public GetListingAlertsForListingQuery()
        {
        }

        public GetListingAlertsForListingQuery(int listingTypeId, int listingId)
        {
            ListingTypeId = listingTypeId;
            ListingId = listingId;
        }
    }
}
