using FibiEmlakDanismanlik.Application.Features.Results.FibiServiceResults;
using FibiEmlakDanismanlik.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Interfaces.FibiServiceInterfaces
{
    /// <summary>Mahalle dış piyasa emsal istatistikleri repository arayüzü.</summary>
    public interface IMarketStatRepository
    {
        Task<ExternalNeighborhoodMarketStat?> GetByNeighborhoodIdAsync(int neighborhoodId);
        Task<List<FibiLocationSummaryResult>> GetCoveredLocationsAsync();
        Task<decimal> GetCityAverageSalePriceAsync(int cityId);
    }
}
