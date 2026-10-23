using FibiEmlakDanismanlik.Application.Interfaces.AgentInterfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;
using FibiEmlakDanismanlik.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
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

        /// <summary>
        /// Yalnızca sitede görünen aktif danışmanların toplam sayısını döndürür.
        /// </summary>
        public async Task<int> GetCountAsync()
        {
            return await _context.Agents
                .Where(x => x.IsActive && x.ShowOnWebsite)
                .CountAsync();
        }

        /// <summary>
        /// Yalnızca sitede görünen aktif danışmanları sayfalanmış olarak döndürür.
        /// </summary>
        public async Task<List<Agent>> GetPagedAsync(int skip, int take)
        {
            return await _context.Agents
                .AsNoTracking()
                .Where(x => x.IsActive && x.ShowOnWebsite)
                .OrderBy(x => x.AgentId)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<AgentAuthState?> GetAuthStateAsync(int agentId)
        {
            return await _context.Agents
                .AsNoTracking()
                .Where(x => x.AgentId == agentId)
                .Select(x => new AgentAuthState(x.IsActive, x.SecurityStamp, x.Role))
                .FirstOrDefaultAsync();
        }

        public async Task<List<AgentRecoveryCode>> GetUnusedRecoveryCodesAsync(int agentId)
        {
            return await _context.AgentRecoveryCodes
                .Where(x => x.AgentId == agentId && x.UsedAtUtc == null)
                .ToListAsync();
        }

        public async Task ReplaceRecoveryCodesAsync(int agentId, IEnumerable<string> codeHashes)
        {
            var existingCodes = await _context.AgentRecoveryCodes
                .Where(x => x.AgentId == agentId)
                .ToListAsync();

            if (existingCodes.Count > 0)
            {
                _context.AgentRecoveryCodes.RemoveRange(existingCodes);
            }

            var now = DateTime.UtcNow;
            var newCodes = codeHashes.Select(hash => new AgentRecoveryCode
            {
                AgentId = agentId,
                CodeHash = hash,
                CreatedAtUtc = now,
                UsedAtUtc = null
            });

            await _context.AgentRecoveryCodes.AddRangeAsync(newCodes);
            await _context.SaveChangesAsync();
        }

        public async Task MarkRecoveryCodeUsedAsync(int agentRecoveryCodeId)
        {
            var code = await _context.AgentRecoveryCodes.FindAsync(agentRecoveryCodeId);
            if (code != null)
            {
                code.UsedAtUtc = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> CountActiveOwnersAsync(int? excludeAgentId = null)
        {
            return await _context.Agents
                .Where(x => x.IsActive && x.Role == AgentRole.Owner && (excludeAgentId == null || x.AgentId != excludeAgentId))
                .CountAsync();
        }

        public async Task<List<Agent>> GetAllForAdminAsync()
        {
            return await _context.Agents
                .AsNoTracking()
                .OrderBy(x => x.Role)
                .ThenBy(x => x.AgentName)
                .ToListAsync();
        }
    }
}
