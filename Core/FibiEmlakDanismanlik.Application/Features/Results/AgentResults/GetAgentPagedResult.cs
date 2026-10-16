using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Features.Results.AgentResult
{
    public class GetAgentPagedResult
    {
        public List<GetAgentResult> Items { get; set; } = new();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
