using FibiEmlakDanismanlik.Application.Features.Commands.RentalCommercialPropertyListingCommands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalCommercialPropertyListingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RentalCommercialPropertyListingController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        public async Task<IActionResult> CreateRentalCommercialPropertyListing(CreateRentalCommercialPropertyListingCommand command)
        {
           await _mediator.Send(command);
            return Ok("Kiralık İş Yeri İlanı Başarıyla eklendi");
        }
    }
}
