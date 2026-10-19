namespace FibiEmlakDanismanlik.Application.Models.MapModels
{
    public class ListingGeocodeCandidate
    {
        public int ListingId { get; set; }
        public int ListingTypeId { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Neighborhood { get; set; }
    }
}
