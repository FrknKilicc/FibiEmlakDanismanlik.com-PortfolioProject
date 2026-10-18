using FibiEmlakDanismanlik.Application.Features.Queries.FibiServiceQueries;
using FibiEmlakDanismanlik.Application.Features.Results.FibiServiceResults;
using FibiEmlakDanismanlik.Application.Helpers;
using FibiEmlakDanismanlik.Application.Interfaces.FibiServiceInterfaces;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.FibiServiceHandlers
{
    public class GetFibiServiceLocationsQueryHandler : IRequestHandler<GetFibiServiceLocationsQuery, List<GetFibiServiceLocationResult>>
    {
        private readonly IMarketStatRepository _marketStatRepository;

        public GetFibiServiceLocationsQueryHandler(IMarketStatRepository marketStatRepository)
        {
            _marketStatRepository = marketStatRepository;
        }

        public async Task<List<GetFibiServiceLocationResult>> Handle(GetFibiServiceLocationsQuery request, CancellationToken cancellationToken)
        {
            var rawLocations = await _marketStatRepository.GetCoveredLocationsAsync();
            var displayLocations = rawLocations.Select(FibiLocationSummaryFactory.ToDisplay).ToList();

            var result = displayLocations
                .GroupBy(l => new { l.CityId, l.CityName })
                .Select(cg => new GetFibiServiceLocationResult(
                    cg.Key.CityId,
                    cg.Key.CityName,
                    cg.GroupBy(d => new { d.DistrictId, d.DistrictName })
                      .Select(dg => new FibiServiceDistrictResult(
                          dg.Key.DistrictId,
                          dg.Key.DistrictName,
                          dg.Select(ng => new FibiServiceNeighborhoodResult(
                              ng.NeighborhoodId,
                              ng.NeighborhoodName
                          )).ToList()
                      )).ToList()
                ))
                .ToList();

            return result;
        }
    }
}
