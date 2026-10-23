using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Interfaces.ListingAlertInterfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;
using FibiEmlakDanismanlik.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FibiEmlakDanismanlik.Persistence.Repositories.ListingAlertRepositories
{
    public class ListingAlertRepository : IListingAlertRepository
    {
        private readonly FibiEmlakDanismanlikContext _context;

        public ListingAlertRepository(FibiEmlakDanismanlikContext context)
        {
            _context = context;
        }

        public async Task<bool> CityExistsAsync(int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.CityId == cityId);
        }

        public async Task<bool> DistrictBelongsToCityAsync(int districtId, int cityId)
        {
            return await _context.Districts.AnyAsync(d => d.DistrictId == districtId && d.CityId == cityId);
        }

        public async Task<int> CountActiveByPhoneAsync(string phone)
        {
            return await _context.ListingAlerts.CountAsync(x => x.PhoneNumber == phone && x.Status == ListingAlertStatus.Active);
        }

        public async Task AddAsync(ListingAlert alert)
        {
            await _context.ListingAlerts.AddAsync(alert);
            await _context.SaveChangesAsync();
        }
    }
}
