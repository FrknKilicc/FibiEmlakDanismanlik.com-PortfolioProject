using FibiEmlakDanismanlik.Application.Features.Results.PopularPlaceResults;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Interfaces.PopularPlaceInterfaces
{
    public interface IPopularPlaceRepository
    {
        Task<GetPopularPlaceResult> GetPopularPlacesAsync(int usageType, int take, int minCityCount, CancellationToken cancellationToken);
    }
}
