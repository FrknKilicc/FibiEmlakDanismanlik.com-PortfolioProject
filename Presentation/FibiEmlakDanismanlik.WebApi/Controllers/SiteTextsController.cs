using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.SiteTextCommands;
using FibiEmlakDanismanlik.Application.Features.Queries.SiteTextQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    /// <summary>
    /// Sitede panelden düzenlenebilen metinler (başlıklar, kart yazıları, sayılar).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SiteTextsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SiteTextsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Site için anahtar → değer sözlüğü (varsayılanlarla birleştirilmiş).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetSiteTexts()
        {
            return Ok(await _mediator.Send(new GetSiteTextsQuery()));
        }

        /// <summary>
        /// Panel düzenleme ekranı için tanımlarla birlikte metinler.
        /// </summary>
        [HttpGet("admin")]
        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        public async Task<IActionResult> GetAdminSiteTexts()
        {
            return Ok(await _mediator.Send(new GetAdminSiteTextsQuery()));
        }

        /// <summary>
        /// Site metinlerini günceller (Sahip veya Yönetici).
        /// </summary>
        [HttpPut]
        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        public async Task<IActionResult> UpdateSiteTexts([FromBody] UpdateSiteTextsCommand command)
        {
            await _mediator.Send(command);
            return Ok("Site metinleri güncellendi.");
        }
    }
}
