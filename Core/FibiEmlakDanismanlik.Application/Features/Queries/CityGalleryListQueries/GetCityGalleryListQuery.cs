using FibiEmlakDanismanlik.Application.Features.Results.CityGalleryResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Queries.CityGalleryListQueries
{
    public class GetCityGalleryListQuery:IRequest<List<GetCityGalleryListResult>>
    {

    }
}
