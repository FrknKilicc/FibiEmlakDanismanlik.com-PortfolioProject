using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Features.Results.AuditLogResults;
using FibiEmlakDanismanlik.Application.Interfaces.AuditLogInterfaces;
using FibiEmlakDanismanlik.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FibiEmlakDanismanlik.Persistence.Repositories.AuditLogRepositories;

/// <summary>
/// Denetim günlüğü kayıtlarını veritabanından sorgulayan depo sınıfı.
/// </summary>
public class AuditLogRepository : IAuditLogRepository
{
    private readonly FibiEmlakDanismanlikContext _context;

    public AuditLogRepository(FibiEmlakDanismanlikContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Sayfalanmış ve filtrelenmiş denetim günlüğü kayıtlarını en yeni kayıt önce olacak biçimde getirir.
    /// Personel adı bilgisi için Agents tablosu ile sol birleşim (left join) yapılır.
    /// </summary>
    public async Task<(int Total, List<AuditLogItemResult> Items)> GetPagedAsync(int skip, int take, string? entityName, int? agentId)
    {
        var query = _context.AuditLogs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(entityName))
        {
            var trimmedEntity = entityName.Trim();
            query = query.Where(a => a.EntityName == trimmedEntity);
        }

        if (agentId.HasValue)
        {
            query = query.Where(a => a.AgentId == agentId.Value);
        }

        var total = await query.CountAsync();

        var items = await (from a in query
                           join agent in _context.Agents.AsNoTracking() on a.AgentId equals agent.AgentId into agentGroup
                           from agent in agentGroup.DefaultIfEmpty()
                           select new AuditLogItemResult
                           {
                               AuditLogId = a.AuditLogId,
                               AgentId = a.AgentId,
                               AgentName = agent != null ? agent.AgentName : null,
                               Action = a.Action,
                               EntityName = a.EntityName,
                               EntityKey = a.EntityKey,
                               ChangesJson = a.ChangesJson,
                               IpAddress = a.IpAddress,
                               CreatedAtUtc = a.CreatedAtUtc
                           })
                           .OrderByDescending(x => x.CreatedAtUtc)
                           .ThenByDescending(x => x.AuditLogId)
                           .Skip(skip)
                           .Take(take)
                           .ToListAsync();

        return (total, items);
    }
}
