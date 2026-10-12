using FibiEmlakDanismanlik.Domain.Entities;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Interfaces.AgentInterfaces
{
    public interface IAgentRepository
    {
        Task<Agent?> GetByMailAsync(string mail);
        Task<bool> MailExistsAsync(string mail, int? excludeAgentId = null);
    }
}
