using FibiEmlakDanismanlik.Application.Features.Results.LocationResults;
using MediatR;
using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Features.Queries.LocationQueries
{
    public class GetAvailableDistrictsQuery : IRequest<List<LocationOptionResult>>
    {
        public int CityId { get; set; }
        public int UsageType { get; set; }
        public int ListingTypeId { get; set; }
        public string? Q { get; set; }

        public GetAvailableDistrictsQuery(int cityId, int usageType, int listingTypeId, string? q = null)
        {
            CityId = cityId;
            UsageType = usageType;
            ListingTypeId = listingTypeId;
            Q = q;
        }
    }
}

