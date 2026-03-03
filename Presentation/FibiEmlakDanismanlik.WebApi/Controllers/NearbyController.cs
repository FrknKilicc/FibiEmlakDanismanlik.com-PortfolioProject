using FibiEmlakDanismanlik.Application.Features.Queries.NearbyQueries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NearbyController : ControllerBase
    {
        private readonly IMediator mediator;

        public NearbyController(IMediator mediator)
        {
            this.mediator = mediator;
        }
        [HttpGet("GetByListing")]
        public async Task<IActionResult> GetByListing([FromQuery] int listingId, [FromQuery] int ListingTypeId)
        {
            var result = await mediator.Send(new GetNearbyByListingQuery(listingId, ListingTypeId));
            return Ok(result); 
        }
    }
}
