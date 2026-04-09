using FibiEmlakDanismanlik.Application.Features.Queries.TestimonialsQueries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestimonailsController : ControllerBase
    {
        private readonly IMediator mediator;

        public TestimonailsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("GetTestimonialsList")]
        public async Task<IActionResult> GetTestimonialsList()
        {
            var value = await mediator.Send(new GetTestimonailsQuery ());
            return Ok (value);  
        }
    }
}
