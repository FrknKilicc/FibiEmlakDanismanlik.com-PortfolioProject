using FibiEmlakDanismanlik.Application.Features.Results.CityGalleryResults;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Interfaces.CityGalleryInterfaces
{
    public interface ICityGalleryRepository
    {
        Task<List<GetCityGalleryListResult>> GetCityGalleryListAsync(bool includeInactive = false);
        Task<GetCityGalleryByIdResult?> GetCityGalleryByIdAsync(int id);
    }
}
