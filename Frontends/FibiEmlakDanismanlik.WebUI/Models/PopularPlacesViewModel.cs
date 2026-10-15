using FibiEmlakDanismanlik.Dto.PropertyDtos;

namespace FibiEmlakDanismanlik.WebUI.Models
{
    public class PopularPlacesViewModel
    {
        public ResultPopularPlaceDto ForSale { get; set; } = new();
        public ResultPopularPlaceDto ForRent { get; set; } = new();
    }
}
