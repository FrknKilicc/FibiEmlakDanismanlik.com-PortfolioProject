using FibiEmlakDanismanlik.Application.Features.Results.LocationResults;
using MediatR;
using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Features.Queries.LocationQueries
{
    public class GetAvailableCitiesQuery : IRequest<List<LocationOptionResult>>
    {
        public int UsageType { get; set; }
        public int ListingTypeId { get; set; }
        public string? Q { get; set; }

        public GetAvailableCitiesQuery(int usageType, int listingTypeId, string? q = null)
        {
            UsageType = usageType;
            ListingTypeId = listingTypeId;
            Q = q;
        }
    }
}

