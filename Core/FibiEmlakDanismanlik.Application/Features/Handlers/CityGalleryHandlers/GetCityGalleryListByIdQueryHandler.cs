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
    public class GetCityGalleryListByIdQueryHandler : IRequestHandler<GetCityGalleryListByIdQuery, GetCityGalleryByIdResult?>
    {
        private readonly ICityGalleryRepository _cityGalleryRepository;

        public GetCityGalleryListByIdQueryHandler(ICityGalleryRepository cityGalleryRepository)
        {
            _cityGalleryRepository = cityGalleryRepository;
        }

        public async Task<GetCityGalleryByIdResult?> Handle(GetCityGalleryListByIdQuery request, CancellationToken cancellationToken)
        {
            return await _cityGalleryRepository.GetCityGalleryByIdAsync(request.Id);
        }
    }
}
