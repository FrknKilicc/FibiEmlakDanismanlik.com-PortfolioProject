namespace FibiEmlakDanismanlik.Application.Features.Results.FibiServiceResults
{
    /// <summary>Mahalle m² fiyat bantları sonucu.</summary>
    public record FibiPriceBandResult(decimal MarketMinSqmPrice, decimal MarketBalancedSqmPrice, decimal PremiumPortfolioSqmPrice);
}
