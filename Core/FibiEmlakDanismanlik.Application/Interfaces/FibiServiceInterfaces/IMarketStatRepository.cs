using FibiEmlakDanismanlik.Domain.Entities;
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
        Task<decimal> GetCityAverageSalePriceAsync(int cityId);
    }
}
