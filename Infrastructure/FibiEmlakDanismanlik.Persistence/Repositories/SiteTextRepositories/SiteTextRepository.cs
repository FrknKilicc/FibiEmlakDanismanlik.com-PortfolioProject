using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Interfaces.SiteTextInterfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FibiEmlakDanismanlik.Persistence.Repositories.SiteTextRepositories
{
    /// <summary>
    /// Site metinleri için veritabanı erişim repository sınıfı.
    /// </summary>
    public class SiteTextRepository : ISiteTextRepository
    {
        private readonly FibiEmlakDanismanlikContext _context;

        public SiteTextRepository(FibiEmlakDanismanlikContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<string, string>> GetAllAsync()
        {
            return await _context.SiteTexts.AsNoTracking()
                .ToDictionaryAsync(x => x.Key, x => x.Value);
        }

        public async Task SaveAsync(IReadOnlyDictionary<string, string> values, int? agentId)
        {
            var keys = values.Keys.ToList();
            var existing = await _context.SiteTexts
                .Where(x => keys.Contains(x.Key))
                .ToDictionaryAsync(x => x.Key);
            var now = DateTime.UtcNow;

            foreach (var (key, value) in values)
            {
                if (existing.TryGetValue(key, out var row))
                {
                    if (row.Value == value) continue;
                    row.Value = value;
                    row.UpdatedAtUtc = now;
                    row.UpdatedByAgentId = agentId;
                }
                else
                {
                    _context.SiteTexts.Add(new SiteText { Key = key, Value = value, UpdatedAtUtc = now, UpdatedByAgentId = agentId });
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
