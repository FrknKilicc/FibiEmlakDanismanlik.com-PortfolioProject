using FibiEmlakDanismanlik.Application.Features.Commands.AgentCommands;
using FibiEmlakDanismanlik.Application.Features.Handlers.AgentHandlers;
using FibiEmlakDanismanlik.Application.Features.Queries.AgentQueries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AgentController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> GetAgentList()
        {
            var value = await _mediator.Send(new GetAgentQuery());
            return Ok(value);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAgentById(int id)
        {
            var value = await _mediator.Send(new GetAgentByIdQuery(id));
            return Ok(value);
        }
        [HttpPost]
        public async Task<IActionResult> CreateAgent(CreateAgentCommand command)
        {
            await _mediator.Send(command);
            return Ok("Personel Kaydı Başarıyla Gerçekleştirildi.");
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAgent(UpdateAgentCommand command)
        {
            await _mediator.Send(command);
            return Ok("Personel Güncelleme Başarılı");
        }
        [HttpDelete]
        public async Task<IActionResult> RemoveAgent(int id)
        {
            await _mediator.Send(new RemoveAgentCommand(id));
            return Ok("Personel Silme İşlemi Başarılı");
        }
    }
}
