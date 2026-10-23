using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.OurPartnersCommands;
using FibiEmlakDanismanlik.Application.Features.Queries.OurPartnersQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OurPartnersController : ControllerBase
    {
        private readonly IMediator mediator;

        public OurPartnersController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("GetOurPartnersList")]
        public async Task<IActionResult> GetOurPartnersList()
        {
            var value = await mediator.Send(new GetOurPartnersQuery());
            return Ok(value);   
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOurPartnerById(int id)
        {
            var value = await mediator.Send(new GetOurPartnerByIdQuery(id));
            if (value == null)
            {
                return NotFound();
            }
            return Ok(value);
        }

        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpPost]
        public async Task<IActionResult> CreateOurPartner([FromBody] CreateOurPartnerCommand command)
        {
            try
            {
                await mediator.Send(command);
                return Ok("Partner başarıyla eklendi");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpPut]
        public async Task<IActionResult> UpdateOurPartner([FromBody] UpdateOurPartnerCommand command)
        {
            try
            {
                await mediator.Send(command);
                return Ok("Partner başarıyla güncellendi");
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
        public async Task<IActionResult> RemoveOurPartner(int id)
        {
            try
            {
                await mediator.Send(new RemoveOurPartnerCommand(id));
                return Ok("Partner başarıyla silindi");
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
