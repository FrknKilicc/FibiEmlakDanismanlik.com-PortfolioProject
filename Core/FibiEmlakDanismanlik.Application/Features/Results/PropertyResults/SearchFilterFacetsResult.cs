using System.Collections.Generic;
using FibiEmlakDanismanlik.Application.Features.Results.ListingTypeResults;

namespace FibiEmlakDanismanlik.Application.Features.Results.PropertyResults
{
    public class SearchFilterFacetsResult
    {
        public int TotalCount { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public double? MinArea { get; set; }
        public double? MaxArea { get; set; }
        public List<string> Rooms { get; set; } = new();
        public List<int> Bathrooms { get; set; } = new();
        public List<int> Floors { get; set; } = new();
        public List<int> Sections { get; set; } = new();
        public List<string> ZoningStatuses { get; set; } = new();
        public List<FilterOptionDto> Agents { get; set; } = new();
        public List<FilterOptionDto> HousingCategories { get; set; } = new();
        public List<FilterOptionDto> LandCategories { get; set; } = new();
        public List<string> BuildingAges { get; set; } = new();
        public List<string> Heatings { get; set; } = new();
        public List<string> TitleDeedStatuses { get; set; } = new();
    }
}

