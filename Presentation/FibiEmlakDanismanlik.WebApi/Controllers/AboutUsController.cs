using FibiEmlakDanismanlik.Application.Features.Commands.AboutUsCommands;
using FibiEmlakDanismanlik.Application.Features.Queries.AboutUsQueries;
using MediatR;
using Microsoft.AspNetCore.Http;
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAboutUsById(int id)
        {
            var value = await _mediator.Send(new GetAboutUsByIdQuery(id));
            return Ok(value);
        }
        [HttpPost]
        public async Task<IActionResult> CreateAboutUs(CreateAboutUsCommand command)
        {
            await _mediator.Send(command);
            return Ok("Hakkımızda Bilgisi Başarıyla Eklendi");
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAboutUs(UpdateAboutUsCommand command)
        {
            await _mediator.Send(command);
            return Ok("Hakkımızda Bilgisi Başarıyla Güncellendi");

        }
        [HttpDelete]
        public async Task<IActionResult> RemoveAboutUs(int id)
        {
             await _mediator.Send(new RemoveAboutUsCommand(id));
            return Ok("Hakkımızda Bilgisi Başarıyla Silindi");
        }
    }
}
