using FibiEmlakDanismanlik.Application.Features.Commands.TestimonialsCommands;
using FibiEmlakDanismanlik.Application.Features.Queries.TestimonialsQueries;
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
    public class TestimonailsController : ControllerBase
    {
        private readonly IMediator mediator;

        public TestimonailsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("GetTestimonialsList")]
        public async Task<IActionResult> GetTestimonialsList()
        {
            var value = await mediator.Send(new GetTestimonailsQuery());
            return Ok(value);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTestimonialById(int id)
        {
            var value = await mediator.Send(new GetTestimonialByIdQuery(id));
            if (value == null)
            {
                return NotFound();
            }
            return Ok(value);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateTestimonial([FromBody] CreateTestimonialCommand command)
        {
            try
            {
                await mediator.Send(command);
                return Ok("Yorum başarıyla eklendi");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateTestimonial([FromBody] UpdateTestimonialCommand command)
        {
            try
            {
                await mediator.Send(command);
                return Ok("Yorum başarıyla güncellendi");
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

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> RemoveTestimonial(int id)
        {
            try
            {
                await mediator.Send(new RemoveTestimonialCommand(id));
                return Ok("Yorum başarıyla silindi");
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
