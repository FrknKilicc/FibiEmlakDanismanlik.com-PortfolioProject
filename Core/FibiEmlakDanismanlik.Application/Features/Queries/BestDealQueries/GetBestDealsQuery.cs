using FibiEmlakDanismanlik.Application.Features.Results.BestDealResults;
using MediatR;
using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Features.Queries.BestDealQueries
{
    public class GetBestDealsQuery : IRequest<List<GetBestDealResult>>
    {
        public int UsageType { get; set; }

        public GetBestDealsQuery(int usageType)
        {
            UsageType = usageType;
        }

        public GetBestDealsQuery()
        {
        }
    }
}
