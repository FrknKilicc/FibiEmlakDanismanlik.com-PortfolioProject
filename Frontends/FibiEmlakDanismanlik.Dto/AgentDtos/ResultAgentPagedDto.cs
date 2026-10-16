using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Dto.AgentDtos
{
    public class ResultAgentPagedDto
    {
        public List<ResultAgentDto> Items { get; set; } = new();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
