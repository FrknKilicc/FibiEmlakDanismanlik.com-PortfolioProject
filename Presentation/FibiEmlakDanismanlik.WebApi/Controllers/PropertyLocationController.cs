using FibiEmlakDanismanlik.Application.Features.Queries.PropertyLocationQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/locations")]
    [ApiController]
    public class PropertyLocationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PropertyLocationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int usageType)
        {
            var result = await _mediator.Send(new GetPropertyLocationCitiesQuery(usageType));
            return Ok(result);
        }

        [HttpGet("GetFilteredListings")]
        public async Task<IActionResult> GetFilteredListings(
            [FromQuery] int usageType,
            [FromQuery] int listingTypeId,
            [FromQuery] string? location,
            [FromQuery] string? keyword)
        {
            var response = await _mediator.Send(new GetFilteredPropertyLocationListingsQuery(usageType, listingTypeId, location, keyword));

            if (!response.ListingTypeFound)
                return NotFound("Geçerli ilan tipi giriniz");

            if (!response.IsValidCombination)
                return BadRequest("Geçersiz kullanım tipi veya ilan tipi.");

            return Ok(response.Items);
        }
    }
}