using System.Threading.Tasks;
using FibiEmlakDanismanlik.Domain.Entities;

namespace FibiEmlakDanismanlik.Application.Interfaces.BusinessSettingInterfaces
{
    /// <summary>
    /// İş kuralı ayarları için veri erişim arayüzü.
    /// </summary>
    public interface IBusinessSettingRepository
    {
        /// <summary>
        /// Mevcut ayarları getirir; kayıt yoksa varsayılan nesneyi döner.
        /// </summary>
        Task<BusinessSetting> GetAsync();

        /// <summary>
        /// Ayar kaydını günceller veya yoksa ekler.
        /// </summary>
        Task SaveAsync(BusinessSetting values);
    }
}
