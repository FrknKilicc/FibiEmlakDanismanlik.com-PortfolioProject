using FibiEmlakDanismanlik.Application.Features.Queries.BestDealQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BestDealsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BestDealsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetBestDeals(int usageType)
        {
            if (usageType != 1 && usageType != 2)
            {
                return BadRequest("usageType 1 (satılık) veya 2 (kiralık) olmalı.");
            }

            return Ok(await _mediator.Send(new GetBestDealsQuery(usageType)));
        }
    }
}
