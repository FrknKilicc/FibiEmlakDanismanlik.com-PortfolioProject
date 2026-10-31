using System.Collections.Generic;
using FibiEmlakDanismanlik.Domain.Entities;

namespace FibiEmlakDanismanlik.Application.Features.Results.AdminListingResults
{
    /// <summary>
    /// Olanak arama/seçim öğesi.
    /// </summary>
    public class AmenityLookupItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? GroupName { get; set; }
        public bool AppliesToHousing { get; set; }
        public bool AppliesToLand { get; set; }
        public bool AppliesToCommercial { get; set; }
    }

    /// <summary>
    /// Danışman seçim öğesi.
    /// </summary>
    public class AgentLookupItem
    {
        public int AgentId { get; set; }
        public string AgentName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Yönetim paneli ilan filtreleme ve düzenleme sayfalarında kullanılan sabit/tanım verileri.
    /// </summary>
    public class GetAdminListingLookupsResult
    {
        public List<AmenityLookupItem> Amenities { get; set; } = new();
        public List<HousingCategory> HousingCategories { get; set; } = new();
        public List<LandCategory> LandCategories { get; set; } = new();
        public List<AgentLookupItem> Agents { get; set; } = new();
        public List<string> Statuses { get; set; } = new();
    }
}
