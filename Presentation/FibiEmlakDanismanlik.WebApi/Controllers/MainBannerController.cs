using FibiEmlakDanismanlik.Application.Features.Commands.MainBannerCommands;
using FibiEmlakDanismanlik.Application.Features.Queries.AgentQueries;
using FibiEmlakDanismanlik.Application.Features.Queries.MainBannerQueries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainBannerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MainBannerController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> GetMainBannerList()
        {
            var value = await _mediator.Send(new GetMainBannerQuery());
            return Ok(value);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMainBannerById(int id)
        {
            var value = await _mediator.Send(new GetMainBannerByIdQuery(id));
            return Ok(value);
        }
        [HttpPost]
        public async Task<IActionResult> CreateMainBanner(CreateMainBannerCommand command)
        {
            await _mediator.Send(command);
            return Ok("Banner Bilgisi Başarıyla Eklendi");
        }
        [HttpPut]
        public async Task<IActionResult> UpdateMainBanner(UpdateMainBannerCommand command)
        {
            await _mediator.Send(command);
            return Ok("Banner Bilgisi Başarıyla Güncellendi");

        }
        [HttpDelete]
        public async Task<IActionResult> RemoveMainBanner(int id)
        {
            await _mediator.Send(new RemoveMainBannerCommand(id));
            return Ok("Banner Bilgisi Başarıyla Silindi");
        }
    }
}
