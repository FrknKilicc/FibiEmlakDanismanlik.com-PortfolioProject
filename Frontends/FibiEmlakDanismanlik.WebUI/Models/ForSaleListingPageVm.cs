using FibiEmlakDanismanlik.Dto.PropertyDtos;

namespace FibiEmlakDanismanlik.WebUI.Models
{
    public class ForSaleListingPageVm
    {
        public List<ListingTypeFacetVm> ListingTypeFacets { get; set; } = new();
        public List<ResultAllForSaleListinForPageDto> Items { get; set; } = new();
        public List<int> SelectedListingTypeIds { get; set; } = new();
    }
    public class ListingTypeFacetVm
    {
        public int ListingTypeId { get; set; }
        public string Name { get; set; } = "";
        public int Count { get; set; }
        public bool Selected { get; set; }
    }
}
