using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.SocialMediaCommands;
using FibiEmlakDanismanlik.Application.Features.Queries.SocialMediaQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SocialMediaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SocialMediaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetActiveSocialMediaList()
        {
            var value = await _mediator.Send(new GetSocialMediaListQuery());
            return Ok(value);
        }

        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpPost]
        public async Task<IActionResult> Create(CreateSocialMediaCommand command)
        {
            await _mediator.Send(command);
            return Ok("Sosyal medya hesabı eklendi.");
        }

        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpPut]
        public async Task<IActionResult> Update(UpdateSocialMediaCommand command)
        {
            try
            {
                await _mediator.Send(command);
                return Ok("Sosyal medya hesabı güncellendi.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpDelete]
        public async Task<IActionResult> Remove(int id)
        {
            try
            {
                await _mediator.Send(new RemoveSocialMediaCommand(id));
                return Ok("Sosyal medya hesabı silindi.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
