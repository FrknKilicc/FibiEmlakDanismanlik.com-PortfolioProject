using FibiEmlakDanismanlik.Application.Features.Results.ListingTypeResults;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Queries.ListingTypeQueries
{
    public class GetAdvancedFiltersByListingTypeQuery : IRequest<AdvancedFiltersVisibilityDto>
    {
        public int ListingTypeId { get; set; }
        public int UsageType { get; set; }
        public int? CityId { get; set; }
        public int? DistrictId { get; set; }

        public GetAdvancedFiltersByListingTypeQuery(int listingTypeId, int usageType = 0, int? cityId = null, int? districtId = null)
        {
            ListingTypeId = listingTypeId;
            UsageType = usageType;
            CityId = cityId;
            DistrictId = districtId;
        }
    }
}

