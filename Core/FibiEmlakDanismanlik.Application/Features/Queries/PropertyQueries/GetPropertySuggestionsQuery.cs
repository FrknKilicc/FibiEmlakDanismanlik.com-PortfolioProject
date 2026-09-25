using FibiEmlakDanismanlik.Application.Features.Results.PropertyResults;
using MediatR;
using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Features.Queries.PropertyQueries
{
    public class GetPropertySuggestionsQuery : IRequest<List<PropertySuggestionDto>>
    {
        public int UsageType { get; set; }
        public int? ListingTypeId { get; set; }
        public string SearchTerm { get; set; }

        public GetPropertySuggestionsQuery(int usageType, int? listingTypeId, string searchTerm)
        {
            UsageType = usageType;
            ListingTypeId = listingTypeId;
            SearchTerm = searchTerm;
        }
    }
}

