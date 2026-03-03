using FibiEmlakDanismanlik.Application.Features.Results.NearbyResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Queries.NearbyQueries
{
    public class GetNearbyByListingQuery:IRequest<List<NearbyCategoryGroupResult>>
    {
        public int ListingId { get; }
        public int ListingType { get; }

        public GetNearbyByListingQuery(int listingId, int listingType)
        {
            ListingId = listingId;
            ListingType = listingType;
        }
    }
}
