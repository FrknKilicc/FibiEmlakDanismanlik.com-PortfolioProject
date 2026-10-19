using MediatR;
using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Features.Queries.PropertyLocationQueries
{
    public record GetPropertyLocationCitiesQuery(int UsageType) : IRequest<List<string>>;
}
