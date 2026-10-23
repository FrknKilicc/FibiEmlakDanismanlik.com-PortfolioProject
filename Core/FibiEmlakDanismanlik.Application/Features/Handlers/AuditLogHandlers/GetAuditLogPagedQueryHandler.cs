using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Features.Queries.AuditLogQueries;
using FibiEmlakDanismanlik.Application.Features.Results.AuditLogResults;
using FibiEmlakDanismanlik.Application.Interfaces.AuditLogInterfaces;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.AuditLogHandlers;

/// <summary>
/// Sayfalanmış denetim günlüğü kayıtları sorgusunu işleyen işleyici sınıfı.
/// </summary>
public class GetAuditLogPagedQueryHandler : IRequestHandler<GetAuditLogPagedQuery, GetAuditLogPagedResult>
{
    private readonly IAuditLogRepository _auditLogRepository;

    public GetAuditLogPagedQueryHandler(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<GetAuditLogPagedResult> Handle(GetAuditLogPagedQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize switch
        {
            < 1 => 50,
            > 100 => 100,
            _ => request.PageSize
        };

        var skip = (page - 1) * pageSize;
        var (total, items) = await _auditLogRepository.GetPagedAsync(skip, pageSize, request.EntityName, request.AgentId);

        return new GetAuditLogPagedResult
        {
            TotalCount = total,
            Page = page,
            PageSize = pageSize,
            Items = items
        };
    }
}
