using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Results.FibiServiceResults;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;

namespace FibiEmlakDanismanlik.Application.Helpers
{
    /// <summary>FİBİ veri kaynağı ve kapsam etiketleme fabrikası.</summary>
    public static class FibiDataSourceFactory
    {
        public static FibiDataSourceResult FromStat(ExternalMarketStat stat)
        {
            return new FibiDataSourceResult(
                GetScopeCode(stat.Scope),
                GetScopeLabel(stat.Scope),
                stat.DataSource,
                stat.LastUpdatedDate,
                stat.DataSource == FibiServiceConstants.EstimatedSeedDataSource);
        }

        public static string GetScopeLabel(MarketStatScope scope) => scope switch
        {
            MarketStatScope.Neighborhood => FibiMarketDataConstants.NeighborhoodScopeLabel,
            MarketStatScope.District => FibiMarketDataConstants.DistrictScopeLabel,
            MarketStatScope.City => FibiMarketDataConstants.CityScopeLabel,
            MarketStatScope.Country => FibiMarketDataConstants.CountryScopeLabel,
            _ => FibiMarketDataConstants.CountryScopeLabel
        };

        public static string GetScopeCode(MarketStatScope scope) => scope switch
        {
            MarketStatScope.Neighborhood => FibiMarketDataConstants.NeighborhoodScopeCode,
            MarketStatScope.District => FibiMarketDataConstants.DistrictScopeCode,
            MarketStatScope.City => FibiMarketDataConstants.CityScopeCode,
            MarketStatScope.Country => FibiMarketDataConstants.CountryScopeCode,
            _ => FibiMarketDataConstants.CountryScopeCode
        };
    }
}
