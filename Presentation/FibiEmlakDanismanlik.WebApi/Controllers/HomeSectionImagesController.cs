using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Commands.HomeSectionImageCommands;
using FibiEmlakDanismanlik.Application.Features.Queries.HomeSectionImageQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeSectionImagesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HomeSectionImagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetHomeSectionImages([FromQuery] string? sectionKey)
        {
            var values = await _mediator.Send(new GetHomeSectionImageQuery(sectionKey));
            return Ok(values);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetHomeSectionImageById(int id)
        {
            var value = await _mediator.Send(new GetHomeSectionImageByIdQuery(id));
            if (value == null)
            {
                return NotFound();
            }
            return Ok(value);
        }

        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpPost]
        public async Task<IActionResult> CreateHomeSectionImage([FromBody] CreateHomeSectionImageCommand command)
        {
            try
            {
                await _mediator.Send(command);
                return Ok("Görsel başarıyla eklendi");
            }
            catch (DuplicateHomeSectionImageSlotException ex)
            {
                return Conflict(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpPut]
        public async Task<IActionResult> UpdateHomeSectionImage([FromBody] UpdateHomeSectionImageCommand command)
        {
            try
            {
                await _mediator.Send(command);
                return Ok("Görsel başarıyla güncellendi");
            }
            catch (DuplicateHomeSectionImageSlotException ex)
            {
                return Conflict(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpDelete]
        public async Task<IActionResult> RemoveHomeSectionImage(int id)
        {
            try
            {
                await _mediator.Send(new RemoveHomeSectionImageCommand(id));
                return Ok("Görsel başarıyla silindi");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
