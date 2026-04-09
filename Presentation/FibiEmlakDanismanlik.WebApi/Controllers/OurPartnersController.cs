using FibiEmlakDanismanlik.Application.Features.Queries.OurPartnersQueries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OurPartnersController : ControllerBase
    {
        private readonly IMediator mediator;

        public OurPartnersController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("GetOurPartnersList")]
        public async Task<IActionResult> GetOurPartnersList()
        {
            var value = await mediator.Send(new GetOurPartnersQuery());
            return Ok(value);   
        }
    }
}
