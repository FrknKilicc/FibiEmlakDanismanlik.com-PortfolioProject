using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Queries.PopularPlaceQueries;
using FibiEmlakDanismanlik.Application.Features.Results.PopularPlaceResults;
using FibiEmlakDanismanlik.Application.Interfaces.PopularPlaceInterfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.PopularPlaceHandlers
{
    public class GetPopularPlacesQueryHandler : IRequestHandler<GetPopularPlacesQuery, GetPopularPlaceResult>
    {
        private readonly IPopularPlaceRepository _repository;

        public GetPopularPlacesQueryHandler(IPopularPlaceRepository repository)
        {
            _repository = repository;
        }

        public Task<GetPopularPlaceResult> Handle(GetPopularPlacesQuery request, CancellationToken cancellationToken)
        {
            var take = request.Take <= 0
                ? PopularPlaceConstants.DefaultCount
                : request.Take > PopularPlaceConstants.MaxCount
                    ? PopularPlaceConstants.MaxCount
                    : request.Take;

            return _repository.GetPopularPlacesAsync(
                request.UsageType,
                take,
                PopularPlaceConstants.MinCityCount,
                cancellationToken);
        }
    }
}
