using FibiEmlakDanismanlik.Application.Features.Commands.RentalCommercialPropertyListingCommands;
using FibiEmlakDanismanlik.Application.Features.Queries.RentalCommercialPropertyListingQueries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForRentalCommercialPropertyListingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ForRentalCommercialPropertyListingController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        public async Task<IActionResult> CreateRentalCommercialPropertyListing(CreateRentalCommercialPropertyListingCommand command)
        {
            await _mediator.Send(command);
            return Ok("Kiralık İş Yeri İlanı Başarıyla eklendi");
        }
        [HttpGet("GetForRentalCommercialListing")]
        public async Task<IActionResult> GetForRentalCommercialListing ()
        {
            var value = await _mediator.Send(new GetRentalCommercialPropertyListingQuery());
            return Ok(value);
        }
    }
}
