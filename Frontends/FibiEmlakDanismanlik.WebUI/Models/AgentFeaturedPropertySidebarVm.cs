namespace FibiEmlakDanismanlik.WebUI.Models
{
    public class AgentFeaturedPropertySidebarVm
    {
        public int ListingId { get; set; }
        public string PropertyName { get; set; }
        public string PropertyDescription { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal Price { get; set; }
        public string? PropImgUrl1 { get; set; }
        public string PropertyStatusLabel { get; set; } // For Rent / For Sale
        public string DetailUrl { get; set; }
    }
}
