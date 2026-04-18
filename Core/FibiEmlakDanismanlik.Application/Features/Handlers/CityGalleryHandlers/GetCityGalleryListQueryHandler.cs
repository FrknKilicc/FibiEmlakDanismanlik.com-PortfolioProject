using FibiEmlakDanismanlik.Application.Features.Queries.CityGalleryListQueries;
using FibiEmlakDanismanlik.Application.Features.Results.CityGalleryResults;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.CityGalleryInterfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            return await _cityGalleryRepository.GetCityGalleryListAsync();
        }
    }
}
