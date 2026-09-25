using FibiEmlakDanismanlik.Application.Features.Queries.LocationQueries;
using FibiEmlakDanismanlik.Application.Features.Results.LocationResults;
using FibiEmlakDanismanlik.Application.Interfaces.LocationInterfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.LocationHandlers
{
    public class GetAvailableCitiesQueryHandler : IRequestHandler<GetAvailableCitiesQuery, List<LocationOptionResult>>
    {
        private readonly ILocationRepository _locationRepository;

        public GetAvailableCitiesQueryHandler(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        public async Task<List<LocationOptionResult>> Handle(GetAvailableCitiesQuery request, CancellationToken cancellationToken)
        {
            return await _locationRepository.GetAvailableCitiesAsync(request.UsageType, request.ListingTypeId, request.Q);
        }
    }
}

