using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.BusinessSettingCommands;
using FibiEmlakDanismanlik.Application.Features.Queries.BusinessSettingQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    /// <summary>
    /// İş kuralı ayarları yönetim uç noktaları.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
    public class AdminSettingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminSettingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// İş kuralı ayarlarını getirir.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetSettings()
        {
            var result = await _mediator.Send(new GetBusinessSettingsQuery());
            return Ok(result);
        }

        /// <summary>
        /// İş kuralı ayarlarını günceller (Yalnızca Sahip).
        /// </summary>
        [HttpPut]
        [Authorize(Policy = AuthConstants.PolicyOwnerOnly)]
        public async Task<IActionResult> UpdateSettings([FromBody] UpdateBusinessSettingsCommand command)
        {
            await _mediator.Send(command);
            return Ok("Ayarlar güncellendi.");
        }
    }
}
