using FibiEmlakDanismanlik.Application.Features.Commands.ForSaleCommercialPropertyListingCommands;
using FibiEmlakDanismanlik.Application.Features.Queries.ForSaleCommercialPropertyListingQueries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForSaleCommercialPropertyListingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ForSaleCommercialPropertyListingController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> GetForSaleCommercialPropertyListingList()
        {
            var value = await _mediator.Send(new GetForSaleCommercialPropertyListingQuery());
            return Ok(value);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetForSaleCommercialPropertyListingById(int id)
        {
            var value = await _mediator.Send(new GetForSaleCommercialPropertListingByIdQuery(id));
            return Ok(value);
        }
        [HttpPost]
        public async Task<IActionResult> CreateForSaleCommercialPropertyListing(CreateForSaleCommercialPropertyListingCommand command)
        {
            await _mediator.Send(command);
            return Ok("Satılık İş Yeri İlanı Başarıyla Oluşturuldu.");
        }
        [HttpPut]
        public async Task<IActionResult> UpdateForSaleCommercialPropertyListing(UpdateForSaleCommercialPropertyListingCommand command)
        {
            await _mediator.Send(command);
            return Ok("Satılık İş Yeri İlanı Başarıyla Güncelleştirildi.");
        }
        [HttpDelete]
        public async Task<IActionResult> RemoveForSaleCommercialPropertyListing(int id)
        {
            await _mediator.Send(new RemoveForSaleCommercialPropertyListingCommand(id));
            return Ok("Satılık İş Yeri İlanı Başarıyla Güncelleştirildi.");
        }
    }
}
