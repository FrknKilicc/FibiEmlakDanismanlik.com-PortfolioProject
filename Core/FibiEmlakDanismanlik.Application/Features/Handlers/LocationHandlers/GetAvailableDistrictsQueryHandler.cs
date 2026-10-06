using FibiEmlakDanismanlik.Application.Features.Queries.LocationQueries;
using FibiEmlakDanismanlik.Application.Features.Results.LocationResults;
using FibiEmlakDanismanlik.Application.Helpers;
using FibiEmlakDanismanlik.Application.Interfaces.LocationInterfaces;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.LocationHandlers
{
    public class GetAvailableDistrictsQueryHandler : IRequestHandler<GetAvailableDistrictsQuery, List<LocationOptionResult>>
    {
        private readonly ILocationRepository _locationRepository;

        public GetAvailableDistrictsQueryHandler(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        public async Task<List<LocationOptionResult>> Handle(GetAvailableDistrictsQuery request, CancellationToken cancellationToken)
        {
            var items = await _locationRepository.GetAvailableDistrictsAsync(request.CityId, request.UsageType, request.ListingTypeId, request.Q);
            return items
                .Select(item => new LocationOptionResult(item.Id, TurkishTextHelper.ToTurkishTitleCase(item.Text)))
                .ToList();
        }
    }
}

