using FibiEmlakDanismanlik.AdminUI.Models.Listings;

namespace FibiEmlakDanismanlik.AdminUI.Models.Leads;

/// <summary>
/// Müşteri talebi ayrıntı sayfası görünüm modeli.
/// </summary>
public sealed class LeadDetailViewModel
{
    public LeadDetailDto Detail { get; set; } = new();
    public List<AgentLookupDto> Agents { get; set; } = new();
    public bool CanAssign { get; set; }
    public string? ErrorMessage { get; set; }
}
