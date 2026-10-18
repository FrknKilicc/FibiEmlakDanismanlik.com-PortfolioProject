namespace FibiEmlakDanismanlik.WebUI.Models.FibiServices;

/// <summary>
/// FİBİ Hizmetler modül kataloğu (tek kaynak).
/// </summary>
public static class FibiServiceCatalog
{
    public const string RiskAnalysis = "risk-analysis";
    public const string NeighborhoodAnalysis = "neighborhood-analysis";
    public const string Finance = "finance";
    public const string Expert = "expert";
    public const string Investment = "investment";

    public static IReadOnlyList<FibiServiceNavItemVm> Items { get; } = new List<FibiServiceNavItemVm>
    {
        new(
            Key: RiskAnalysis,
            ModuleId: 1,
            Title: "Fibi Risk Analiz",
            ShortDescription: "Bina deprem riski ve kentsel dönüşüm potansiyeli",
            ActionName: "RiskAnalysis",
            IconClass: "fas fa-house-damage",
            IsAvailable: true
        ),
        new(
            Key: NeighborhoodAnalysis,
            ModuleId: 2,
            Title: "Fibi Bölge Analiz",
            ShortDescription: "Mahalle m² bandı, amortisman ve değer projeksiyonu",
            ActionName: "NeighborhoodAnalysis",
            IconClass: "fas fa-chart-line",
            IsAvailable: true
        ),
        new(
            Key: Finance,
            ModuleId: 3,
            Title: "Fibi Finans",
            ShortDescription: "Kira destekli kredi ve gizli maliyet hesaplayıcı",
            ActionName: "Finance",
            IconClass: "fas fa-calculator",
            IsAvailable: false
        ),
        new(
            Key: Expert,
            ModuleId: 4,
            Title: "Fibi Eksper",
            ShortDescription: "Şerefiye katsayılı mülk değerleme",
            ActionName: "Expert",
            IconClass: "fas fa-clipboard-check",
            IsAvailable: false
        ),
        new(
            Key: Investment,
            ModuleId: 5,
            Title: "Fibi Yatırım",
            ShortDescription: "Bütçe ve amaca göre yatırım fırsatı simülatörü",
            ActionName: "Investment",
            IconClass: "fas fa-coins",
            IsAvailable: false
        )
    };

    public static FibiServiceNavItemVm Get(string key)
    {
        var item = Items.FirstOrDefault(x => string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase));
        if (item is null)
        {
            throw new InvalidOperationException($"Tanımlı olmayan FİBİ hizmet anahtarı: '{key}'.");
        }

        return item;
    }
}
