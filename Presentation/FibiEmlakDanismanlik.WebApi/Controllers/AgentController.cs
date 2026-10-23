using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Commands.AgentCommands;
using FibiEmlakDanismanlik.Application.Features.Handlers.AgentHandlers;
using FibiEmlakDanismanlik.Application.Features.Queries.AgentQueries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("paged")]
        public async Task<IActionResult> GetAgentPagedList(int page = 1, int pageSize = 3)
        {
            var value = await _mediator.Send(new GetAgentPagedQuery(page, pageSize));
            return Ok(value);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAgentById(int id)
        {
            var value = await _mediator.Send(new GetAgentByIdQuery(id));
            if (value == null)
            {
                return NotFound();
            }
            return Ok(value);
        }
        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpPost]
        public async Task<IActionResult> CreateAgent(CreateAgentCommand command)
        {
            try
            {
                await _mediator.Send(command);
                return Ok("Personel Kaydı Başarıyla Gerçekleştirildi.");
            }
            catch (DuplicateAgentMailException ex)
            {
                return Conflict(ex.Message);
            }
        }
        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpPut]
        public async Task<IActionResult> UpdateAgent(UpdateAgentCommand command)
        {
            try
            {
                await _mediator.Send(command);
                return Ok("Personel Güncelleme Başarılı");
            }
            catch (DuplicateAgentMailException ex)
            {
                return Conflict(ex.Message);
            }
        }
        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpDelete]
        public async Task<IActionResult> RemoveAgent(int id)
        {
            await _mediator.Send(new RemoveAgentCommand(id));
            return Ok("Personel Silme İşlemi Başarılı");
        }
    }
}