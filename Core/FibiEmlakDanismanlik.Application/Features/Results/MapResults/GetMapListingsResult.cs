namespace FibiEmlakDanismanlik.Application.Features.Results.MapResults
{
    public class GetMapListingsResult
    {
        public int ListingId { get; set; }
        public int ListingTypeId { get; set; }
        public int UsageTypeId { get; set; }          // 1 satılık, 2 kiralık
        public string CategoryKey { get; set; } = "";  // housing / land / commercial
        public string ListingTypeName { get; set; } = "";
        public string Title { get; set; } = "";
        public decimal Price { get; set; }             // satılıkta Price, kiralıkta Rent
        public double? Area { get; set; }
        public string? Rooms { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Neighborhood { get; set; }
        public string? ImageUrl { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public bool IsApproximate { get; set; }
    }
}
