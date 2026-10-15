using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Queries.PopularPlaceQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PopularPlacesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PopularPlacesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetPopularPlaces(int usageType, int take = PopularPlaceConstants.DefaultCount, CancellationToken cancellationToken = default)
        {
            if (usageType != 1 && usageType != 2)
            {
                return BadRequest("usageType 1 (satılık) veya 2 (kiralık) olmalı.");
            }

            var result = await _mediator.Send(new GetPopularPlacesQuery(usageType, take), cancellationToken);
            return Ok(result);
        }
    }
}
