using System.Collections.Generic;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Interfaces.SiteTextInterfaces
{
    /// <summary>
    /// Site metinleri (anahtar-değer) veri erişimi.
    /// </summary>
    public interface ISiteTextRepository
    {
        /// <summary>
        /// Kayıtlı tüm metinleri anahtar → değer olarak döner.
        /// </summary>
        Task<Dictionary<string, string>> GetAllAsync();

        /// <summary>
        /// Verilen anahtarları ekler veya günceller.
        /// </summary>
        Task SaveAsync(IReadOnlyDictionary<string, string> values, int? agentId);
    }
}
