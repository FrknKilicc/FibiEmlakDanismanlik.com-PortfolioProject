using FibiEmlakDanismanlik.Application.Features.Results.LocationResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Queries.LocationQueries
{
    public record GetNeighborhoodsQuery(int DistrictId, string? Q) : IRequest<List<LocationOptionResult>>;
    
}
