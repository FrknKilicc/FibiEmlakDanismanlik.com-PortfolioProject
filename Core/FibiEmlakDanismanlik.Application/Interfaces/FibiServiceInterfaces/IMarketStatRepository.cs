using FibiEmlakDanismanlik.Application.Features.Results.FibiServiceResults;
using FibiEmlakDanismanlik.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Interfaces.FibiServiceInterfaces
{
    /// <summary>Mahalle dış piyasa emsal istatistikleri repository arayüzü.</summary>
    public interface IMarketStatRepository
    {
        Task<Neighborhood?> GetNeighborhoodWithParentsAsync(int neighborhoodId);
        /// <summary>Mahalle, ilçe, il ve Türkiye kapsamları arasından en dar mevcut kaydı döner.</summary>
        Task<ExternalMarketStat?> ResolveStatAsync(int neighborhoodId, int districtId, int cityId);
        Task<NeighborhoodSeismicScenarioStat?> GetSeismicScenarioAsync(int neighborhoodId);
        Task<List<FibiLocationSummaryResult>> GetCoveredLocationsAsync();
        Task<decimal> GetCityAverageSalePriceAsync(int cityId);
    }
}
