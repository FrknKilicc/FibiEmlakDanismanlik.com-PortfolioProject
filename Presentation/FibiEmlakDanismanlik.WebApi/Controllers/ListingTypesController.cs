using FibiEmlakDanismanlik.Application.Features.Queries.ListingTypeQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/listingtypes")]
    [ApiController]
    public class ListingTypesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ListingTypesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int usageType, [FromQuery] bool onlyWithActiveListings = false)
            => Ok(await _mediator.Send(new GetListingTypesByUsageQuery(usageType, onlyWithActiveListings)));

        [HttpGet("advanced-filters")]
        public async Task<IActionResult> GetAdvancedFilters([FromQuery] int listingTypeId)
            => Ok(await _mediator.Send(new GetAdvancedFiltersByListingTypeQuery(listingTypeId)));
    }
}

