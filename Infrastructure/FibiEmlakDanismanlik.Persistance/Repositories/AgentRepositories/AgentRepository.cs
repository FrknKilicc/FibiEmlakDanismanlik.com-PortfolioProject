using FibiEmlakDanismanlik.Application.Interfaces.AgentInterfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Persistence.Repositories.AgentRepositories
{
    public class AgentRepository : IAgentRepository
    {
        private readonly FibiEmlakDanismanlikContext _context;

        public AgentRepository(FibiEmlakDanismanlikContext context)
        {
            _context = context;
        }

        public async Task<Agent?> GetByMailAsync(string mail)
        {
            var trimmed = mail?.Trim();
            return await _context.Agents
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Mail == trimmed);
        }

        public async Task<bool> MailExistsAsync(string mail, int? excludeAgentId = null)
        {
            var trimmed = mail?.Trim();
            return await _context.Agents
                .AnyAsync(x => x.Mail == trimmed && (excludeAgentId == null || x.AgentId != excludeAgentId));
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.Agents.CountAsync();
        }

        public async Task<List<Agent>> GetPagedAsync(int skip, int take)
        {
            return await _context.Agents
                .AsNoTracking()
                .OrderBy(x => x.AgentId)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }
    }
}
