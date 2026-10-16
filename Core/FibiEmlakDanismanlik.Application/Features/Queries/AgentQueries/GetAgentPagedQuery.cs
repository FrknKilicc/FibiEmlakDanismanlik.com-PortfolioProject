using FibiEmlakDanismanlik.Application.Features.Results.AgentResult;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Queries.AgentQueries
{
    public class GetAgentPagedQuery : IRequest<GetAgentPagedResult>
    {
        public GetAgentPagedQuery(int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
        }

        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
