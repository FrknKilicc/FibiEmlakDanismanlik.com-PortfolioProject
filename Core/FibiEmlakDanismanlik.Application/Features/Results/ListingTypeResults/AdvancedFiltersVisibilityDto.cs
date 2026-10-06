using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Features.Results.ListingTypeResults
{
    public class FilterOptionDto
    {
        public string Id { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;

        public FilterOptionDto() { }
        public FilterOptionDto(string id, string text)
        {
            Id = id;
            Text = text;
        }
    }

    public class AdvancedFiltersVisibilityDto
    {
        public string Category { get; set; } = string.Empty;
        public string RoomLabel { get; set; } = string.Empty;
        public string BathLabel { get; set; } = string.Empty;

        public bool ShowRooms { get; set; }
        public bool ShowBath { get; set; }
        public bool ShowFloor { get; set; }
        public bool ShowZoning { get; set; }
        public bool ShowPrice { get; set; }
        public bool ShowArea { get; set; }
        public bool ShowDistance { get; set; }

        public List<FilterOptionDto> RoomOptions { get; set; } = new();
        public List<FilterOptionDto> BathOptions { get; set; } = new();
        public List<FilterOptionDto> FloorOptions { get; set; } = new();
        public List<FilterOptionDto> ZoningOptions { get; set; } = new();
        public List<FilterOptionDto> AgentOptions { get; set; } = new();

        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public double MinArea { get; set; }
        public double MaxArea { get; set; }
        public int TotalCount { get; set; }
    }
}

