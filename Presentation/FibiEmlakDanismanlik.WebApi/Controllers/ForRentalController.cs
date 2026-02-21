using FibiEmlakDanismanlik.Application.Features.Queries.ForRentalPropertyQueries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForRentalController : ControllerBase
    {
        private readonly IMediator mediator;

        public ForRentalController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("GetAllForRentalPropertyForListing")]
        public async Task<IActionResult> GetAllForRentalPropertyForListing()
        {
            var value = await mediator.Send(new GetAllForRentalPropertiesForListingQuery());
            return Ok(value);
        }
        [HttpGet("GetLast10RentalProperties")]
        public async Task<IActionResult> GetLast10RentalProperties()
        {
            var query = await mediator.Send(new GetLast10RentalPropertyQuery());
            return Ok(query);
        }
    }
}
