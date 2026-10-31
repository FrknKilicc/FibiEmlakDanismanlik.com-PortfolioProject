using FibiEmlakDanismanlik.Application.Features.Queries.CityGalleryListQueries;
using FibiEmlakDanismanlik.Application.Features.Results.CityGalleryResults;
using FibiEmlakDanismanlik.Application.Interfaces.CityGalleryInterfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.CityGalleryHandlers
{
    public class GetCityGalleryListQueryHandler : IRequestHandler<GetCityGalleryListQuery, List<GetCityGalleryListResult>>
    {
        private readonly ICityGalleryRepository _cityGalleryRepository;

        public GetCityGalleryListQueryHandler(ICityGalleryRepository cityGalleryRepository)
        {
            _cityGalleryRepository = cityGalleryRepository;
        }

        public async Task<List<GetCityGalleryListResult>> Handle(GetCityGalleryListQuery request, CancellationToken cancellationToken)
        {
            return await _cityGalleryRepository.GetCityGalleryListAsync(request.IncludeInactive);
        }
    }
}
