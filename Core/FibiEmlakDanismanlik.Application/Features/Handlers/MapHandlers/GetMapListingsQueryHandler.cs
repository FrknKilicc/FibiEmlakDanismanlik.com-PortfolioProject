using FibiEmlakDanismanlik.Application.Features.Queries.MapQueries;
using FibiEmlakDanismanlik.Application.Features.Results.MapResults;
using FibiEmlakDanismanlik.Application.Interfaces.MapInterfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.MapHandlers
{
    public class GetMapListingsQueryHandler : IRequestHandler<GetMapListingsQuery, List<GetMapListingsResult>>
    {
        private readonly IMapRepository mapRepository;

        public GetMapListingsQueryHandler(IMapRepository mapRepository)
        {
            this.mapRepository = mapRepository;
        }

        public Task<List<GetMapListingsResult>> Handle(GetMapListingsQuery request, CancellationToken cancellationToken)
            => mapRepository.GetMapListingsAsync(request.UsageType, request.Category, request.CityId);
    }
}
