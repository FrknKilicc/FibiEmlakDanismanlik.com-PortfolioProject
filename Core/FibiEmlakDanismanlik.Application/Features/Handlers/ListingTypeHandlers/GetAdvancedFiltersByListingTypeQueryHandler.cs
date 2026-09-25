using FibiEmlakDanismanlik.Application.Features.Queries.ListingTypeQueries;
using FibiEmlakDanismanlik.Application.Features.Results.ListingTypeResults;
using FibiEmlakDanismanlik.Application.Interfaces.ListingTypeInterfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ListingTypeHandlers
{
    public class GetAdvancedFiltersByListingTypeQueryHandler : IRequestHandler<GetAdvancedFiltersByListingTypeQuery, AdvancedFiltersVisibilityDto>
    {
        private readonly IListingTypeRepository _listingTypeRepository;

        public GetAdvancedFiltersByListingTypeQueryHandler(IListingTypeRepository listingTypeRepository)
        {
            _listingTypeRepository = listingTypeRepository;
        }

        public async Task<AdvancedFiltersVisibilityDto> Handle(GetAdvancedFiltersByListingTypeQuery request, CancellationToken cancellationToken)
        {
            var listingType = await _listingTypeRepository.GetByIdAsync(request.ListingTypeId);
            
            var result = new AdvancedFiltersVisibilityDto
            {
                ShowRooms = true,
                ShowBath = true,
                ShowFloor = true,
                ShowPrice = true,
                ShowArea = true,
                ShowDistance = true
            };

            if (listingType != null && listingType.Name.Contains("Arsa", System.StringComparison.OrdinalIgnoreCase))
            {
                result.ShowRooms = false;
                result.ShowBath = false;
                result.ShowFloor = false;
            }

            return result;
        }
    }
}

