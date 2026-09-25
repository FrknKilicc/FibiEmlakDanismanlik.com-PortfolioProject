using FibiEmlakDanismanlik.Application.Features.Queries.PropertyQueries;
using FibiEmlakDanismanlik.Application.Features.Results.PropertyResults;
using FibiEmlakDanismanlik.Application.Interfaces.PropertyInterfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.PropertyHandlers
{
    public class GetPropertySuggestionsQueryHandler : IRequestHandler<GetPropertySuggestionsQuery, List<PropertySuggestionDto>>
    {
        private readonly IPropertyRepository _propertyRepository;

        public GetPropertySuggestionsQueryHandler(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }

        public async Task<List<PropertySuggestionDto>> Handle(GetPropertySuggestionsQuery request, CancellationToken cancellationToken)
        {
            return await _propertyRepository.GetPropertySuggestionsAsync(request.UsageType, request.ListingTypeId, request.SearchTerm);
        }
    }
}

