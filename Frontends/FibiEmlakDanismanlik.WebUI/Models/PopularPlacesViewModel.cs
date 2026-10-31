using FibiEmlakDanismanlik.Dto.PropertyDtos;

namespace FibiEmlakDanismanlik.WebUI.Models
{
    public class PopularPlacesViewModel
    {
        public ResultPopularPlaceDto ForSale { get; set; } = new();
        public ResultPopularPlaceDto ForRent { get; set; } = new();

        public string PrimaryImageUrl { get; set; } = "";
        public string PrimaryImageAlt { get; set; } = "";
        public string SecondaryImageUrl { get; set; } = "";
        public string SecondaryImageAlt { get; set; } = "";
    }
}
