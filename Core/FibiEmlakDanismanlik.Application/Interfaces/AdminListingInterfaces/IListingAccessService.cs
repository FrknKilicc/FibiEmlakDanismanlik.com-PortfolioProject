using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces
{
    /// <summary>
    /// Mevcut oturum açmış kullanıcının ilanlar üzerindeki yetki ve kapsam denetim servisi arayüzü.
    /// </summary>
    public interface IListingAccessService
    {
        /// <summary>
        /// Belirli bir ilan için kullanıcının erişim düzeyini (None, View, Edit) döner.
        /// </summary>
        Task<ListingAccess> GetAccessAsync(int listingTypeId, int listingId);

        /// <summary>
        /// Kullanıcının görebileceği ilanların kapsamını döner.
        /// </summary>
        Task<ListingScope> GetScopeAsync();
    }
}
