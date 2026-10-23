namespace FibiEmlakDanismanlik.WebUI.Models
{
    public class ListingLocationVm
    {
        public bool IsRent { get; set; }
        public int CityId { get; set; }
        public string City { get; set; } = string.Empty;
        public int? DistrictId { get; set; }
        public string? District { get; set; }
        public string CanonicalPath { get; set; } = string.Empty;   // ListingUrl.ListPath(IsRent, City, District)
        public string Heading { get; set; } = string.Empty;         // "Kadıköy, İstanbul Satılık İlanlar" / "İstanbul Kiralık İlanlar"
    }

    public class LocationOptionVm
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
