using FibiEmlakDanismanlik.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Interfaces.AgentInterfaces
{
    public interface IAgentRepository
    {
        Task<Agent?> GetByMailAsync(string mail);
        Task<bool> MailExistsAsync(string mail, int? excludeAgentId = null);
        /// <summary>
        /// Yalnızca sitede görünen aktif danışmanların toplam sayısını döndürür.
        /// </summary>
        Task<int> GetCountAsync();
        /// <summary>
        /// Yalnızca sitede görünen aktif danışmanları sayfalanmış olarak döndürür.
        /// </summary>
        Task<List<Agent>> GetPagedAsync(int skip, int take);
        /// <summary>
        /// Danışmanın geçerli oturum ve yetki durumunu döndürür.
        /// </summary>
        Task<AgentAuthState?> GetAuthStateAsync(int agentId);

        /// <summary>
        /// Danışmanın henüz kullanılmamış iki adımlı kurtarma kodlarını döndürür.
        /// </summary>
        Task<List<AgentRecoveryCode>> GetUnusedRecoveryCodesAsync(int agentId);

        /// <summary>
        /// Danışmanın mevcut tüm kurtarma kodlarını siler ve verilen yeni kod hash'lerini ekler.
        /// </summary>
        Task ReplaceRecoveryCodesAsync(int agentId, IEnumerable<string> codeHashes);

        /// <summary>
        /// Belirtilen kurtarma kodunu kullanıldı olarak işaretler.
        /// </summary>
        Task MarkRecoveryCodeUsedAsync(int agentRecoveryCodeId);

        /// <summary>
        /// Sistemdeki aktif Sahip (Owner) sayısını döndürür. İsteğe bağlı olarak belirtilen personel hariç tutulabilir.
        /// </summary>
        Task<int> CountActiveOwnersAsync(int? excludeAgentId = null);

        /// <summary>
        /// Yönetim paneli için tüm personelleri (gizli ve pasifler dahil) rol ve ada göre sıralı olarak döndürür.
        /// </summary>
        Task<List<Agent>> GetAllForAdminAsync();

        /// <summary>
        /// Danışmanın son giriş denemelerini en yeni tarihten eskiye doğru döndürür.
        /// </summary>
        Task<List<LoginAuditLog>> GetRecentLoginsAsync(int agentId, int take);
    }
}
