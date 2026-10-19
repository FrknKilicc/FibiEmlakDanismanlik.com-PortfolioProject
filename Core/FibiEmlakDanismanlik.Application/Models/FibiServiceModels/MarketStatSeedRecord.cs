using FibiEmlakDanismanlik.Domain.Enums;

namespace FibiEmlakDanismanlik.Application.Models.FibiServiceModels
{
    /// <summary>Gömülü başlangıç emsal kaydı. İl için CityPlaceCode, ilçe için CityPlaceCode + DistrictName (DB'deki büyük harfli ad) doldurulur; Türkiye kaydında ikisi de null.</summary>
    public record MarketStatSeedRecord(
        MarketStatScope Scope,
        int? CityPlaceCode,
        string? DistrictName,
        decimal SalePricePerSqm,
        decimal RentPricePerSqm,
        decimal Last1YearGrowthRate,
        decimal Last3YearsCompoundGrowthRate,
        decimal RegionalDevelopmentFactor,
        SoilRiskClass? SoilRiskClass,
        int UrbanTransformationAppetiteScore);
}
