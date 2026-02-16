using FibiEmlakDanismanlik.Application.Features.Commands.ForSaleHousingCommands;
using FibiEmlakDanismanlik.Application.Features.Handlers.ForSaleHousingListingHandlers;
using FibiEmlakDanismanlik.Application.Features.Queries.ForSaleHousingListingQueries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForSaleHousingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ForSaleHousingController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("GetForSaleHousingList")]
        public async Task<IActionResult> GetForSaleHousingList()
        {
            var value = await _mediator.Send(new GetForSaleHousingPropertyListingQuery());
            return Ok(value);
        }

        [HttpPost]
        public async Task<IActionResult> CreateForSaleHousingListing(CreateForSaleHousingCommand command)
        {
            await _mediator.Send(command);
            return Ok("Kayıt Başarıyla Eklendi");    
        }
    }
}
