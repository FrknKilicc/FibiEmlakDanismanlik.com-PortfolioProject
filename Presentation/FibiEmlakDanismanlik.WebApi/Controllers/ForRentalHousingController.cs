using FibiEmlakDanismanlik.Application.Features.Commands.ForSaleCommercialPropertyListingCommands;
using FibiEmlakDanismanlik.Application.Features.Commands.RentalHousingPropertyListingCommands;
using FibiEmlakDanismanlik.Application.Features.Queries.ForRentalHousingListingQueries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForRentalHousingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ForRentalHousingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]

        public async Task<IActionResult> CreateForRentalHousingListing(CreateRentalHousingPropertyListingCommand command)
        {
            await _mediator.Send(command);
            return Ok("Kayıt Başarıyla Tamamlandı");
        }

        [HttpGet("GetForRentalHousingListing")]

        public async Task<IActionResult> GetForRentalHousingListing()
        {
            var value = await _mediator.Send(new GetRentalHousingPropertyListingQueries());
            return Ok(value);
        }
    }
}
