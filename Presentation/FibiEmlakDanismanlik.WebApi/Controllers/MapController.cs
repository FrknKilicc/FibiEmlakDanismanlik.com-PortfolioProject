using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Queries.MapQueries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MapController : ControllerBase
    {
        private readonly IMediator mediator;

        public MapController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("GetListingMarker")]
        public async Task<IActionResult> GetListingMarker([FromQuery] int listingId, [FromQuery] int listingTypeId)
        {
            var data = await mediator.Send(new GetListingMarkerQuery
            {
                ListingId = listingId,
                ListingTypeId = listingTypeId
            });
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpGet("listings")]
        public async Task<IActionResult> GetMapListings([FromQuery] int? usageType, [FromQuery] string? category, [FromQuery] int? cityId)
        {
            if (usageType.HasValue && usageType.Value != 1 && usageType.Value != 2)
            {
                return BadRequest("Geçersiz filtre.");
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                var normCategory = category.Trim().ToLowerInvariant();
                if (normCategory != ListingGeoConstants.CategoryHousing &&
                    normCategory != ListingGeoConstants.CategoryLand &&
                    normCategory != ListingGeoConstants.CategoryCommercial)
                {
                    return BadRequest("Geçersiz filtre.");
                }
            }

            return Ok(await mediator.Send(new GetMapListingsQuery
            {
                UsageType = usageType,
                Category = category,
                CityId = cityId
            }));
        }
    }
}
