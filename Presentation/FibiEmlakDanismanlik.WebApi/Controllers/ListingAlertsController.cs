using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.ListingAlertCommands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListingAlertsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ListingAlertsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [AllowAnonymous]
        [EnableRateLimiting(ListingAlertConstants.RateLimitPolicy)]
        public async Task<IActionResult> CreateListingAlert(CreateListingAlertCommand command)
        {
            await _mediator.Send(command);
            return Ok("Talebiniz alındı. Uygun ilan çıktığında sizinle iletişime geçeceğiz.");
        }
    }
}
