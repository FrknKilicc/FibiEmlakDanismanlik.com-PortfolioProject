using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Queries.AuditLogQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebApi.Controllers;

/// <summary>
/// Sistem denetim günlükleri (AuditLogs) yönetim uç noktası.
/// Yalnızca Sahip (Owner) ve Yönetici (Manager) rollerine açıktır.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
public class AuditLogsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuditLogsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Sayfalanmış ve filtrelenmiş denetim günlüğü kayıtlarını getirir.
    /// </summary>
    /// <param name="query">Sayfalama ve filtreleme parametreleri.</param>
    /// <returns>Sayfalanmış denetim günlüğü listesi.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAuditLogs([FromQuery] GetAuditLogPagedQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
