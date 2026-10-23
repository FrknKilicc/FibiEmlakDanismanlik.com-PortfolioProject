using System.Threading.Tasks;
using FibiEmlakDanismanlik.Domain.Entities;

namespace FibiEmlakDanismanlik.Application.Interfaces.ListingAlertInterfaces
{
    /// <summary>
    /// Benzer ilan bildirim talepleri (haber bekleyenler) veri erişim arayüzü.
    /// </summary>
    public interface IListingAlertRepository
    {
        /// <summary>
        /// Belirtilen ilin sistemde kayıtlı olup olmadığını doğrular.
        /// </summary>
        Task<bool> CityExistsAsync(int cityId);

        /// <summary>
        /// Belirtilen ilçenin verilen ile ait olup olmadığını doğrular.
        /// </summary>
        Task<bool> DistrictBelongsToCityAsync(int districtId, int cityId);

        /// <summary>
        /// Verilen telefon numarasına ait aktif durumdaki bildirim talebi sayısını döner.
        /// </summary>
        Task<int> CountActiveByPhoneAsync(string phone);

        /// <summary>
        /// Yeni bir haber talebi kaydeder.
        /// </summary>
        Task AddAsync(ListingAlert alert);
    }
}
