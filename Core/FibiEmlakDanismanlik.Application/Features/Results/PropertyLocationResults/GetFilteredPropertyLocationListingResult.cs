namespace FibiEmlakDanismanlik.Application.Features.Results.PropertyLocationResults
{
    public class GetFilteredPropertyLocationListingResult
    {
        public int ListingId { get; set; }
        public int ListingTypeId { get; set; }
        public string PropertyName { get; set; } = null!;
        public string PropertyDescription { get; set; } = null!;
        public string City { get; set; } = null!;
        public string District { get; set; } = null!;
        public string Neighborhood { get; set; } = null!;
        public decimal? Price { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? PropertyStatus { get; set; }
    }

    public class GetFilteredPropertyLocationListingsResponse
    {
        public bool ListingTypeFound { get; set; }
        public bool IsValidCombination { get; set; }
        public List<GetFilteredPropertyLocationListingResult> Items { get; set; } = new();
    }
}
