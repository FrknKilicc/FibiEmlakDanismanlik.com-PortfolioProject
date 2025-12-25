using FibiEmlakDanismanlik.Application.Features.Queries.LocationQueries;
using FibiEmlakDanismanlik.Application.Features.Results.LocationResults;
using FibiEmlakDanismanlik.Application.Interfaces.LocationInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.LocationHandlers
{
    public class GetCitiesQueryHandler : IRequestHandler<GetCitiesQuery, List<LocationOptionResult>>
    {
        private readonly ILocationRepository _locationRepository;

        public GetCitiesQueryHandler(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        public Task<List<LocationOptionResult>> Handle(GetCitiesQuery request, CancellationToken cancellationToken)
        {
            return _locationRepository.GetCitiesAsync(request.Q);
        }
    }
}
