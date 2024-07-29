using FibiEmlakDanismanlik.Application.Features.Queries.ForSaleLandListingQueries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForSaleLandListingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ForSaleLandListingController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> GetLandListing()
        {
            var values = await _mediator.Send(new GetForSaleLandListingQuery());
            return Ok(values);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLandListingById(int id)
        {
            var values = await _mediator.Send(new GetForSaleLandListingByIdQuery(id));
            return Ok(values);
        }
    }
}
