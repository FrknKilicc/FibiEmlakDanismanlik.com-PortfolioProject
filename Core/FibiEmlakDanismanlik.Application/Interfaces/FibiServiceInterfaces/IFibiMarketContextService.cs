using FibiEmlakDanismanlik.Application.Models.FibiServiceModels;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Interfaces.FibiServiceInterfaces
{
    /// <summary>Mahalle için kademeli emsal verisi ve deprem tehlikesi çözümleme servisi.</summary>
    public interface IFibiMarketContextService
    {
        /// <summary>Mahalle yoksa veya hiçbir kapsamda emsal verisi yoksa null döner.</summary>
        Task<FibiMarketContext?> GetForNeighborhoodAsync(int neighborhoodId);
    }
}
