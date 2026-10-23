using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.AboutUsCommands;
using FibiEmlakDanismanlik.Application.Features.Queries.AboutUsQueries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AboutUsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AboutUsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentAboutUs()
        {
            var value = await _mediator.Send(new GetCurrentAboutUsQuery());
            if (value == null)
            {
                return NotFound();
            }
            return Ok(value);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAboutUsById(int id)
        {
            var value = await _mediator.Send(new GetAboutUsByIdQuery(id));
            if (value == null)
            {
                return NotFound();
            }
            return Ok(value);
        }
        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpPost]
        public async Task<IActionResult> CreateAboutUs(CreateAboutUsCommand command)
        {
            await _mediator.Send(command);
            return Ok("Hakkımızda Bilgisi Başarıyla Eklendi");
        }
        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpPut]
        public async Task<IActionResult> UpdateAboutUs(UpdateAboutUsCommand command)
        {
            await _mediator.Send(command);
            return Ok("Hakkımızda Bilgisi Başarıyla Güncellendi");

        }
        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpDelete]
        public async Task<IActionResult> RemoveAboutUs(int id)
        {
             await _mediator.Send(new RemoveAboutUsCommand(id));
            return Ok("Hakkımızda Bilgisi Başarıyla Silindi");
        }
    }
}
