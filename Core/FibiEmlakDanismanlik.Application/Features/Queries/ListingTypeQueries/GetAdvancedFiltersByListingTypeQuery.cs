using FibiEmlakDanismanlik.Application.Features.Results.ListingTypeResults;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Queries.ListingTypeQueries
{
    public class GetAdvancedFiltersByListingTypeQuery : IRequest<AdvancedFiltersVisibilityDto>
    {
        public int ListingTypeId { get; set; }

        public GetAdvancedFiltersByListingTypeQuery(int listingTypeId)
        {
            ListingTypeId = listingTypeId;
        }
    }
}

