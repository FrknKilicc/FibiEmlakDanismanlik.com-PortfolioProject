using System.Collections.Generic;
using FibiEmlakDanismanlik.Application.Interfaces.ListingDealInterfaces;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Queries.ListingDealQueries
{
    /// <summary>
    /// Belirli bir ilana ait tüm işlem kayıtlarını getiren sorgu.
    /// </summary>
    public class GetListingDealsForListingQuery : IRequest<List<ListingDealRow>>
    {
        public int ListingTypeId { get; set; }
        public int ListingId { get; set; }

        public GetListingDealsForListingQuery()
        {
        }

        public GetListingDealsForListingQuery(int listingTypeId, int listingId)
        {
            ListingTypeId = listingTypeId;
            ListingId = listingId;
        }
    }
}
