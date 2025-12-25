using FibiEmlakDanismanlik.Application.Features.Queries.LocationQueries;
using FibiEmlakDanismanlik.Persistence.Context;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LocationController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet("cities")]
        public async Task<IActionResult> GetCities([FromQuery] string? q)
                    => Ok(await _mediator.Send(new GetCitiesQuery(q)));

        [HttpGet("districts")]
        public async Task<IActionResult> GetDistricts([FromQuery] int cityId, [FromQuery] string? q)
            => Ok(await _mediator.Send(new GetDistrictsQuery(cityId, q)));

        [HttpGet("neighborhoods")]
        public async Task<IActionResult> GetNeighborhoods([FromQuery] int districtId, [FromQuery] string? q)
            => Ok(await _mediator.Send(new GetNeighborhoodsQuery(districtId, q)));
    }
}
