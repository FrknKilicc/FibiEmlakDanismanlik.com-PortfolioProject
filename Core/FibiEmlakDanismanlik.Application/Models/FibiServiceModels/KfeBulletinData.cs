using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Models.FibiServiceModels
{
    /// <summary>TCMB Konut Fiyat Endeksi aylık bülteninden okunan değişimler (yüzde).</summary>
    public record KfeBulletinData(
        string Period,                       // yyyy-MM
        decimal NationalMonthlyPriceChange,
        decimal NationalAnnualPriceChange,
        decimal NationalMonthlyRentChange,
        decimal NationalAnnualRentChange,
        IReadOnlyList<KfeRegionalChange> RegionalPriceChanges,
        IReadOnlyList<KfeRegionalChange> RegionalRentChanges);

    /// <summary>Bültendeki bir bölgenin il listesi (normalize edilmiş adlar) ve yıllık değişimi.</summary>
    public record KfeRegionalChange(IReadOnlyList<string> ProvinceKeys, decimal AnnualChange);
}
