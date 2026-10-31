using FibiEmlakDanismanlik.AdminUI.Models.Listings;

namespace FibiEmlakDanismanlik.AdminUI.Models.ListingAlerts;

/// <summary>
/// Haber bekleyen talep ayrıntı sayfası görünüm modeli.
/// </summary>
public sealed class ListingAlertDetailViewModel
{
    public ListingAlertDetailDto Detail { get; set; } = new();
    public List<AgentLookupDto> Agents { get; set; } = new();
    public bool CanAssign { get; set; }
    public string? ErrorMessage { get; set; }
}
