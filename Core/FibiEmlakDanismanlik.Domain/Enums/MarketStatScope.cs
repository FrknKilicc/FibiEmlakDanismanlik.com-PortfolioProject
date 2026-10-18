namespace FibiEmlakDanismanlik.Domain.Enums
{
    /// <summary>Emsal verisinin coğrafi kapsamı. Büyük değer daha dar kapsamdır.</summary>
    public enum MarketStatScope
    {
        Country = 1,      // Türkiye geneli
        City = 2,         // İl
        District = 3,     // İlçe
        Neighborhood = 4  // Mahalle
    }
}
