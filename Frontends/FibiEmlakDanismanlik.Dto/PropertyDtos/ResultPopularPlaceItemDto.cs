namespace FibiEmlakDanismanlik.Dto.PropertyDtos
{
    public class ResultPopularPlaceItemDto
    {
        public int? CityId { get; set; }
        public int? DistrictId { get; set; }
        public string City { get; set; } = "";
        public string? District { get; set; }
        public int ListingCount { get; set; }
    }
}
