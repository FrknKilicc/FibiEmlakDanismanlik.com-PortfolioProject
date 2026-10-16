using FibiEmlakDanismanlik.Dto.AgentDtos;
using System;
using System.Collections.Generic;

namespace FibiEmlakDanismanlik.WebUI.Models
{
    public class AgentListingPageVm
    {
        public List<ResultAgentDto> Items { get; set; } = new();

        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        public int TotalPages =>
            PageSize == 0 ? 0 : (int)Math.Ceiling((double)Total / PageSize);

        public int FirstItemNo => Total == 0 ? 0 : (Page - 1) * PageSize + 1;
        public int LastItemNo => Math.Min(Page * PageSize, Total);
    }
}
