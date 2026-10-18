using FibiEmlakDanismanlik.Application.Helpers;
using FibiEmlakDanismanlik.Application.Interfaces.FibiServiceInterfaces;
using FibiEmlakDanismanlik.Application.Models.FibiServiceModels;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Services.FibiServices
{
    /// <summary>Mahalle için kademeli emsal verisi ve deprem tehlikesi çözümleme servisi.</summary>
    public class FibiMarketContextService : IFibiMarketContextService
    {
        private readonly IMarketStatRepository _marketStatRepository;

        public FibiMarketContextService(IMarketStatRepository marketStatRepository)
        {
            _marketStatRepository = marketStatRepository;
        }

        public async Task<FibiMarketContext?> GetForNeighborhoodAsync(int neighborhoodId)
        {
            var neighborhood = await _marketStatRepository.GetNeighborhoodWithParentsAsync(neighborhoodId);
            if (neighborhood == null)
                return null;

            var stat = await _marketStatRepository.ResolveStatAsync(neighborhood.NeighborhoodId, neighborhood.DistrictId, neighborhood.District.CityId);
            if (stat == null)
                return null;

            var scenario = await _marketStatRepository.GetSeismicScenarioAsync(neighborhood.NeighborhoodId);

            return new FibiMarketContext(
                FibiLocationSummaryFactory.FromNeighborhood(neighborhood),
                stat,
                FibiDataSourceFactory.FromStat(stat),
                SeismicHazardResolver.Resolve(scenario, stat));
        }
    }
}
