namespace FibiEmlakDanismanlik.WebUI.Models.FibiServices;

/// <summary>
/// FİBİ Hizmetler modül navigasyon ve katalog öğesi.
/// </summary>
public record FibiServiceNavItemVm(
    string Key,
    int ModuleId,
    string Title,
    string ShortDescription,
    string ActionName,
    string IconClass,
    bool IsAvailable);
