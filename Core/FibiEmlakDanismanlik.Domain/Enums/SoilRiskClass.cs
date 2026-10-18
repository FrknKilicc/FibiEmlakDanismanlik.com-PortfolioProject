namespace FibiEmlakDanismanlik.Domain.Enums
{
    /// <summary>Zemin risk sınıfı (deprem yönetmeliği).</summary>
    public enum SoilRiskClass
    {
        Low = 1,       // Z1 sağlam zemin
        Medium = 2,    // Z2
        High = 3,      // Z3 alüvyon
        VeryHigh = 4   // Z4 dolgu
    }
}
