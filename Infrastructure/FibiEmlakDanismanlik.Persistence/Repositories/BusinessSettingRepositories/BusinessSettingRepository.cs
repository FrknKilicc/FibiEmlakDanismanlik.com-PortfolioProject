using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Interfaces.BusinessSettingInterfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FibiEmlakDanismanlik.Persistence.Repositories.BusinessSettingRepositories
{
    /// <summary>
    /// İş kuralı ayarları için veritabanı erişim repository sınıfı.
    /// </summary>
    public class BusinessSettingRepository : IBusinessSettingRepository
    {
        private readonly FibiEmlakDanismanlikContext _context;

        public BusinessSettingRepository(FibiEmlakDanismanlikContext context)
        {
            _context = context;
        }

        public async Task<BusinessSetting> GetAsync()
        {
            var setting = await _context.BusinessSettings
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.BusinessSettingId == BusinessSettingConstants.SingletonId);

            return setting ?? BusinessSettingConstants.CreateDefault();
        }

        public async Task SaveAsync(BusinessSetting values)
        {
            var existing = await _context.BusinessSettings
                .FirstOrDefaultAsync(x => x.BusinessSettingId == BusinessSettingConstants.SingletonId);

            if (existing == null)
            {
                values.BusinessSettingId = BusinessSettingConstants.SingletonId;
                await _context.BusinessSettings.AddAsync(values);
            }
            else
            {
                existing.SaleBuyerCommissionRate = values.SaleBuyerCommissionRate;
                existing.SaleSellerCommissionRate = values.SaleSellerCommissionRate;
                existing.RentCommissionMonths = values.RentCommissionMonths;
                existing.BestDealMaxPerUsageType = values.BestDealMaxPerUsageType;
                existing.ListingAlertMaxActivePerPhone = values.ListingAlertMaxActivePerPhone;
                existing.UpdatedAtUtc = values.UpdatedAtUtc;
                existing.UpdatedByAgentId = values.UpdatedByAgentId;
            }

            await _context.SaveChangesAsync();
        }
    }
}
