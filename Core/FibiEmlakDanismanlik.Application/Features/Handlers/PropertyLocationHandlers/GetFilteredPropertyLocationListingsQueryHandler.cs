using FibiEmlakDanismanlik.Application.Features.Queries.PropertyLocationQueries;
using FibiEmlakDanismanlik.Application.Features.Results.PropertyLocationResults;
using FibiEmlakDanismanlik.Application.Interfaces.LocationInterfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.PropertyLocationHandlers
{
    public class GetFilteredPropertyLocationListingsQueryHandler : IRequestHandler<GetFilteredPropertyLocationListingsQuery, GetFilteredPropertyLocationListingsResponse>
    {
        private readonly ILocationRepository _locationRepository;

        public GetFilteredPropertyLocationListingsQueryHandler(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        public async Task<GetFilteredPropertyLocationListingsResponse> Handle(GetFilteredPropertyLocationListingsQuery request, CancellationToken cancellationToken)
        {
            return await _locationRepository.GetFilteredListingsByLocationAsync(
                request.UsageType,
                request.ListingTypeId,
                request.Location,
                request.Keyword);
        }
    }
}
