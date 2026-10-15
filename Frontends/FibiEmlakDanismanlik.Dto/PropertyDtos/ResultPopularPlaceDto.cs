using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Dto.PropertyDtos
{
    public class ResultPopularPlaceDto
    {
        public string Level { get; set; } = "City";
        public List<ResultPopularPlaceItemDto> Items { get; set; } = new();
    }
}
