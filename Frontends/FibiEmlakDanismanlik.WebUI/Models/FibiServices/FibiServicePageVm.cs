namespace FibiEmlakDanismanlik.WebUI.Models.FibiServices;

/// <summary>
/// FİBİ Hizmetler ortak sayfa görünüm modeli.
/// </summary>
public class FibiServicePageVm
{
    public FibiServiceNavItemVm Active { get; init; } = null!;
    public IReadOnlyList<FibiServiceNavItemVm> Items { get; init; } = Array.Empty<FibiServiceNavItemVm>();
    public string ApiUrl { get; init; } = string.Empty;
}
