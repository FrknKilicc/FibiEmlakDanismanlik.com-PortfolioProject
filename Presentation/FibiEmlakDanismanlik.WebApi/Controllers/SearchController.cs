using FibiEmlakDanismanlik.Application.Features.Queries.PropertyQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SearchController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("suggestions")]
        public async Task<IActionResult> GetSuggestions([FromQuery] int usageType, [FromQuery] int? listingTypeId, [FromQuery] string searchTerm)
        {
            return Ok(await _mediator.Send(new GetPropertySuggestionsQuery(usageType, listingTypeId, searchTerm)));
        }
    }
}

