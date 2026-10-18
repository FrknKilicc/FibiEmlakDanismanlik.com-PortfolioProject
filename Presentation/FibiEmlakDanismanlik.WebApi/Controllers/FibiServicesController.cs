using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.FibiServiceCommands;
using FibiEmlakDanismanlik.Application.Features.Queries.FibiServiceQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    // Analiz ve lead uçları bilerek anonimdir (analizler salt okunur, lead formu ziyaretçiden gelir). Veri senkronizasyonu uçları [Authorize] ile korunur.
    [Route("api/[controller]")]
    [ApiController]
    public class FibiServicesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FibiServicesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("locations")]
        public async Task<IActionResult> GetLocations()
            => Ok(await _mediator.Send(new GetFibiServiceLocationsQuery()));

        [HttpPost("risk-analysis")]
        public async Task<IActionResult> CalculateBuildingRisk([FromBody] CalculateBuildingRiskQuery query)
        {
            var result = await _mediator.Send(query);
            if (result == null)
                return NotFound("Seçilen mahalle bulunamadı veya bu konum için henüz emsal verisi yok.");

            return Ok(result);
        }

        [HttpPost("neighborhood-analysis")]
        public async Task<IActionResult> GetNeighborhoodAnalysis([FromBody] GetNeighborhoodAnalysisQuery query)
        {
            var result = await _mediator.Send(query);
            if (result == null)
                return NotFound("Seçilen mahalle bulunamadı veya bu konum için henüz emsal verisi yok.");

            return Ok(result);
        }

        [HttpPost("leads")]
        [EnableRateLimiting(FibiServiceConstants.LeadRateLimitPolicy)]
        public async Task<IActionResult> CreateLead([FromBody] CreateFibiServiceLeadCommand command)
        {
            await _mediator.Send(command);
            return Ok("Talebiniz alınmıştır. Uzmanımız en kısa sürede sizinle iletişime geçecektir.");
        }

        [Authorize]
        [HttpPost("sync")]
        public async Task<IActionResult> RunMarketDataSync([FromQuery] string? providerKey)
        {
            var result = await _mediator.Send(new RunMarketDataSyncCommand(providerKey));
            if (result == null)
                return NotFound("Bu anahtarla bir veri sağlayıcısı bulunamadı.");

            return Ok(result);
        }

        [Authorize]
        [HttpGet("sync/logs")]
        public async Task<IActionResult> GetMarketDataSyncLogs()
            => Ok(await _mediator.Send(new GetMarketDataSyncLogsQuery()));
    }
}
