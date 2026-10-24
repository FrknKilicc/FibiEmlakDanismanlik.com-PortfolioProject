using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Commands.AuthorCommands;
using FibiEmlakDanismanlik.Application.Features.Queries.AuthorQueries;
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
    public class AuthorController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthorController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAuthorList()
        {
            var value = await _mediator.Send(new GetAuthorQuery());
            return Ok(value);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAuthorById(int id)
        {
            var value = await _mediator.Send(new GetAuthorByIdQuery(id));
            if (value == null)
            {
                return NotFound();
            }
            return Ok(value);
        }

        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpPost]
        public async Task<IActionResult> CreateAuthor(CreateAuthorCommand command)
        {
            await _mediator.Send(command);
            return Ok("Yazar Bilgisi Başarıyla Oluşturuldu");
        }

        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpPut]
        public async Task<IActionResult> UpdateAuthor(UpdateAuthorCommand command)
        {
            try
            {
                await _mediator.Send(command);
                return Ok("Yazar Başarıyla Güncellendi");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpDelete]
        public async Task<IActionResult> RemoveAuthor(int id)
        {
            try
            {
                await _mediator.Send(new RemoveAuthorCommand(id));
                return Ok("Yazar Başarıyla Silindi");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (BlogContentConflictException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}
