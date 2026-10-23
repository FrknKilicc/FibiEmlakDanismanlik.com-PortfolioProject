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
    }
}
