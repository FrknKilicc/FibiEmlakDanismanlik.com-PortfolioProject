namespace FibiEmlakDanismanlik.Application.Features.Results.PopularPlaceResults
{
    public class PopularPlaceItemResult
    {
        public int? CityId { get; set; }
        public int? DistrictId { get; set; }
        public string City { get; set; } = string.Empty;
        public string? District { get; set; }
        public int ListingCount { get; set; }
    }
}
