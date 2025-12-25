using FibiEmlakDanismanlik.Application.Features.Results.LocationResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Interfaces.LocationInterfaces
{
    public interface ILocationRepository
    {
        public Task<List<LocationOptionResult>> GetCitiesAsync(string? q);
        public Task<List<LocationOptionResult>> GetDistrictsAsync(int cityId, string? q);
        public Task<List<LocationOptionResult>> GetNeighborhoodsAsync(int districtId, string? q);
    }
}
