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

        [HttpGet("available-cities")]
        public async Task<IActionResult> GetAvailableCities([FromQuery] int usageType, [FromQuery] int listingTypeId, [FromQuery] string? q)
            => Ok(await _mediator.Send(new GetAvailableCitiesQuery(usageType, listingTypeId, q)));

        [HttpGet("available-districts")]
        public async Task<IActionResult> GetAvailableDistricts([FromQuery] int cityId, [FromQuery] int usageType, [FromQuery] int listingTypeId, [FromQuery] string? q)
            => Ok(await _mediator.Send(new GetAvailableDistrictsQuery(cityId, usageType, listingTypeId, q)));
    }
}
