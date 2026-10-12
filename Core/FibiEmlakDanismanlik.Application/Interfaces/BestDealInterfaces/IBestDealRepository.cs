using FibiEmlakDanismanlik.Application.Features.Results.BestDealResults;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Interfaces.BestDealInterfaces
{
    public interface IBestDealRepository
    {
        Task<List<GetBestDealResult>> GetBestDealsAsync(int usageType, int take);
    }
}
