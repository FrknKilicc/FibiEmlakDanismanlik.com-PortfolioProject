using FibiEmlakDanismanlik.Application.Features.Results.CityGalleryResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Interfaces.CityGalleryInterfaces
{
    public interface ICityGalleryRepository
    {
        Task<List<GetCityGalleryListResult>> GetCityGalleryListAsync();
        Task<GetCityGalleryByIdResult?> GetCityGalleryByIdAsync(int id);
    }
}
