using FibiEmlakDanismanlik.Application.Features.Queries.RentalPropertyQueries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RentalController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetLast10RentalProperties()
        {
            var query = await _mediator.Send(new GetLast10RentalPropertyQuery());
            return Ok(query);
        }
    }
}
