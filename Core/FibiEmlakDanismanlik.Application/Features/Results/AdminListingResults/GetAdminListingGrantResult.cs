using System;

namespace FibiEmlakDanismanlik.Application.Features.Results.AdminListingResults
{
    /// <summary>
    /// Bir ilana tanımlanmış özel danışman erişim izni modeli.
    /// </summary>
    public class GetAdminListingGrantResult
    {
        public int ListingAccessGrantId { get; set; }
        public int AgentId { get; set; }
        public string AgentName { get; set; } = string.Empty;
        public string Access { get; set; } = string.Empty; // "View" veya "Edit"
        public DateTime GrantedAt { get; set; }
    }
}
