using FibiEmlakDanismanlik.Application.Features.Queries.PropertyLocationQueries;
using FibiEmlakDanismanlik.Application.Interfaces.LocationInterfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.PropertyLocationHandlers
{
    public class GetPropertyLocationCitiesQueryHandler : IRequestHandler<GetPropertyLocationCitiesQuery, List<string>>
    {
        private readonly ILocationRepository _locationRepository;

        public GetPropertyLocationCitiesQueryHandler(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        public async Task<List<string>> Handle(GetPropertyLocationCitiesQuery request, CancellationToken cancellationToken)
        {
            return await _locationRepository.GetListingCitiesAsync(request.UsageType);
        }
    }
}
