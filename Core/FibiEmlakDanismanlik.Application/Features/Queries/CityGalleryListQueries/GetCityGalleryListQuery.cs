using FibiEmlakDanismanlik.Application.Features.Results.CityGalleryResults;
using MediatR;
using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Features.Queries.CityGalleryListQueries
{
    public class GetCityGalleryListQuery : IRequest<List<GetCityGalleryListResult>>
    {
        public bool IncludeInactive { get; set; }

        public GetCityGalleryListQuery()
        {
        }

        public GetCityGalleryListQuery(bool includeInactive)
        {
            IncludeInactive = includeInactive;
        }
    }
}
