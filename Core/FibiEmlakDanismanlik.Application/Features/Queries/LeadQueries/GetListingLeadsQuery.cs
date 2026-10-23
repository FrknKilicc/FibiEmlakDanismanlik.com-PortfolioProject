using System.Collections.Generic;
using FibiEmlakDanismanlik.Application.Interfaces.LeadInterfaces;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Queries.LeadQueries
{
    /// <summary>
    /// Belirli bir ilana ait müşteri talep ve randevu formlarını getiren sorgu.
    /// </summary>
    public class GetListingLeadsQuery : IRequest<List<LeadRow>>
    {
        public int ListingTypeId { get; set; }
        public int ListingId { get; set; }

        public GetListingLeadsQuery()
        {
        }

        public GetListingLeadsQuery(int listingTypeId, int listingId)
        {
            ListingTypeId = listingTypeId;
            ListingId = listingId;
        }
    }
}
