using FibiEmlakDanismanlik.Application.Constants;
using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Features.Results.PopularPlaceResults
{
    public class GetPopularPlaceResult
    {
        public string Level { get; set; } = PopularPlaceConstants.LevelCity;
        public List<PopularPlaceItemResult> Items { get; set; } = new();
    }
}
