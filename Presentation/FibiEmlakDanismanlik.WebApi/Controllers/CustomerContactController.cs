using FibiEmlakDanismanlik.Application.Features.Commands.CustomerContactCommands;
using FibiEmlakDanismanlik.Application.Features.Handlers.CustomerContactHandlers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerContactController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CustomerContactController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost]
        public async Task<IActionResult> CreateCustomerContactRequest(CreateCustomerContactCommand command)
        {
          await _mediator.Send(command);
            return Ok("Müşteri Talebi Başarılı Bir Şekilde Eklenmiştir.");
        }
    }
}
