using System.Collections.Generic;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Domain.Entities;

namespace FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces
{
    /// <summary>
    /// Yönetim paneli ilan okuma, filtreleme, özet ve yardımcı veri erişim arayüzü.
    /// </summary>
    public interface IAdminListingRepository
    {
        /// <summary>
        /// İlanın ait olduğu danışman kimliğini döner. İlan yoksa null döner.
        /// </summary>
        Task<int?> GetListingAgentIdAsync(int listingTypeId, int listingId);

        /// <summary>
        /// Danışmanın tüm ilanları görme yetkisi (CanViewAllListings) olup olmadığını sorgular.
        /// </summary>
        Task<bool> GetCanViewAllListingsAsync(int agentId);

        /// <summary>
        /// Danışmana atanmış özel ilan erişim izinlerini döner.
        /// </summary>
        Task<List<ListingAccessGrant>> GetGrantsForAgentAsync(int agentId);

        /// <summary>
        /// Verilen filtre ve kapsam kurallarına göre birleşik ilanları arar, sıralar ve sayfalar.
        /// </summary>
        Task<(int Total, List<AdminListingRow> Rows)> SearchAsync(AdminListingFilter filter, ListingScope scope);

        /// <summary>
        /// Kullanıcının görebileceği ilan kapsamı üzerinden durum ve kategori sayaçlarını döner.
        /// </summary>
        Task<AdminListingSummary> GetSummaryAsync(ListingScope scope);

        /// <summary>
        /// İlan türü ve kimliğine göre birleştirilmiş ilan detayını, olanaklarını ve konumunu döner.
        /// </summary>
        Task<AdminListingDetail?> GetDetailAsync(int listingTypeId, int listingId);

        /// <summary>
        /// Sistemde tanımlı aktif olanakları sıralı olarak döner.
        /// </summary>
        Task<List<AmenityDefinition>> GetAmenitiesAsync();

        /// <summary>
        /// Konut kategorilerini döner.
        /// </summary>
        Task<List<HousingCategory>> GetHousingCategoriesAsync();

        /// <summary>
        /// Arsa kategorilerini döner.
        /// </summary>
        Task<List<LandCategory>> GetLandCategoriesAsync();

        /// <summary>
        /// Aktif danışmanların kimlik ve ad listesini ada göre sıralı döner.
        /// </summary>
        Task<List<(int AgentId, string AgentName)>> GetActiveAgentsAsync();
    }
}
