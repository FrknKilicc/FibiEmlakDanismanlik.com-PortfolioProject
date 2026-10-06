using FibiEmlakDanismanlik.Application.Features.Queries.LocationQueries;
using FibiEmlakDanismanlik.Application.Features.Results.LocationResults;
using FibiEmlakDanismanlik.Application.Helpers;
using FibiEmlakDanismanlik.Application.Interfaces.LocationInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.LocationHandlers
{
    public class GetDistrictsQueryHandler : IRequestHandler<GetDistrictsQuery, List<LocationOptionResult>>
    {
        private readonly ILocationRepository _locationRepository;

        public GetDistrictsQueryHandler(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        public async Task<List<LocationOptionResult>> Handle(GetDistrictsQuery request, CancellationToken cancellationToken)
        {
            var items = await _locationRepository.GetDistrictsAsync(request.CityId, request.Q);
            return items
                .Select(item => new LocationOptionResult(item.Id, TurkishTextHelper.ToTurkishTitleCase(item.Text)))
                .ToList();
        }
    }
}
