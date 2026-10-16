using FibiEmlakDanismanlik.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Interfaces.AgentInterfaces
{
    public interface IAgentRepository
    {
        Task<Agent?> GetByMailAsync(string mail);
        Task<bool> MailExistsAsync(string mail, int? excludeAgentId = null);
        Task<int> GetCountAsync();
        Task<List<Agent>> GetPagedAsync(int skip, int take);
    }
}
