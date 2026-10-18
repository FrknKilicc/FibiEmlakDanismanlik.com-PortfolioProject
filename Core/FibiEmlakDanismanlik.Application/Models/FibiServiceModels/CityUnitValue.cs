namespace FibiEmlakDanismanlik.Application.Models.FibiServiceModels
{
    /// <summary>Resmi kaynaktan okunan il (null = Türkiye) birim m² satış ve aylık kira değeri; Period yyyy-MM.</summary>
    public record CityUnitValue(
        int? CityPlaceCode,
        decimal SalePricePerSqm,
        decimal RentPricePerSqm,
        string Period);
}
