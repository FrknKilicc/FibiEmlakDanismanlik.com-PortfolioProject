using System.Collections.Generic;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;

namespace FibiEmlakDanismanlik.Application.Interfaces.ListingDealInterfaces
{
    /// <summary>
    /// Satış ve kiralama işlem kayıtları (kapora, komisyon) ve hak ediş (prim) repository arayüzü.
    /// </summary>
    public interface IListingDealRepository
    {
        /// <summary>
        /// Filtreye göre işlem kayıtlarını sayfalanmış olarak arar.
        /// </summary>
        Task<(int Total, List<ListingDealRow> Rows)> SearchAsync(ListingDealFilter f);

        /// <summary>
        /// Status ve sayfalama hariç filtreleri uygulayarak işlem ve prim özetini hesaplar.
        /// </summary>
        Task<ListingDealSummary> GetSummaryAsync(ListingDealFilter f);

        /// <summary>
        /// Belirtilen kimlikteki işlemin tüm detayını ve prim paylarını getirir.
        /// </summary>
        Task<ListingDealDetail?> GetDetailAsync(int id);

        /// <summary>
        /// Belirtilen ilana ait tüm işlemleri en yeni işlem önce gelecek şekilde sayfalamasız getirir.
        /// </summary>
        Task<List<ListingDealRow>> GetForListingAsync(int listingTypeId, int listingId);

        /// <summary>
        /// Belirtilen kimlikteki işlemi prim paylarıyla birlikte takipli (tracked) olarak getirir.
        /// </summary>
        Task<ListingDeal?> GetTrackedAsync(int id);

        /// <summary>
        /// Belirtilen kimlikteki prim payını bağlı işlem ve o işlemin tüm paylarıyla birlikte takipli getirir.
        /// </summary>
        Task<DealShare?> GetTrackedShareAsync(int shareId);

        /// <summary>
        /// Belirtilen ilanda kapora aşamasında (Status == DepositReceived) açık bir işlem olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> HasOpenDealAsync(int listingTypeId, int listingId);

        /// <summary>
        /// Danışmanın aktif olup olmadığını kontrol eder ve aktifse rolünü döner, aktif değilse veya yoksa null döner.
        /// </summary>
        Task<AgentRole?> GetActiveAgentRoleAsync(int agentId);

        /// <summary>
        /// Yeni bir işlem kaydını ekler ve değişiklikleri veritabanına kaydeder.
        /// </summary>
        Task AddAsync(ListingDeal deal);

        /// <summary>
        /// Prim payını DbContext'ten silinmek üzere işaretler (SaveChanges çağırmaz).
        /// </summary>
        void RemoveShare(DealShare share);

        /// <summary>
        /// DbContext üzerindeki değişiklikleri veritabanına kaydeder.
        /// </summary>
        Task SaveChangesAsync();
    }
}
