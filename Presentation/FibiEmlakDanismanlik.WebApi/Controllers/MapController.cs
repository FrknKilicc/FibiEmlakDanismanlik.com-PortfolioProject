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
    }
}
