namespace FibiEmlakDanismanlik.WebUI.Models
{
    /// <summary>
    /// Danışman iletişim butonları ve mobil alt çubuk için görünüm modeli.
    /// </summary>
    public class AgentContactVm
    {
        public string? AgentName { get; set; } = null;
        public string? PhoneNumber { get; set; } = null;
        public string? ListingTitle { get; set; } = null;
        public string? ListingAbsoluteUrl { get; set; } = null;
    }
}
