using FibiEmlakDanismanlik.Dto.PropertyDtos;

namespace FibiEmlakDanismanlik.WebUI.Models
{
    public class ForSaleListingPageVm
    {
        public List<ListingTypeFacetVm> ListingTypeFacets { get; set; } = new();
        public List<ResultAllForSaleListinForPageDto> Items { get; set; } = new();
        public List<int> SelectedListingTypeIds { get; set; } = new();
        public List<AmenityFacetDto> Amenties { get; set; } = new();
        public int? CityId { get; set; }
        public int? DistrictId { get; set; }
        public int? NeighborhoodId { get; set; }
        public  int  Total { get; set; }

        public string? City { get; set; }
        public string? District { get; set; }
        public string? Neighborhood { get; set; }

       
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? NumberOfRoom { get; set; }

        public string SortBy { get; set; } = "CreatedDate";
        public string SortDir { get; set; } = "desc";
    }
    public class ListingTypeFacetVm
    {
        public int ListingTypeId { get; set; }
        public string Name { get; set; } = "";
        public int Count { get; set; }
        public bool Selected { get; set; }
    }
}
