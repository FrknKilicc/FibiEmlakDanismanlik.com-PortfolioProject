using FibiEmlakDanismanlik.Application.Features.Results.MapResults;
using MediatR;
using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Features.Queries.MapQueries
{
    public class GetMapListingsQuery : IRequest<List<GetMapListingsResult>>
    {
        public int? UsageType { get; set; }
        public string? Category { get; set; }
        public int? CityId { get; set; }
    }
}
