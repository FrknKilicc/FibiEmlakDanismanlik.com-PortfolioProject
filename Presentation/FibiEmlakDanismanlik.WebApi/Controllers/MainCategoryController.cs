using FibiEmlakDanismanlik.Application.Features.Commands.MainCategoryCommands;
using FibiEmlakDanismanlik.Application.Features.Queries.MainCategoryQueries;
using FibiEmlakDanismanlik.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainCategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MainCategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> GetMainCategoryList()
        {
            var value = await _mediator.Send(new GetMainCategoryQuery());
            return Ok(value);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMainCategoryById(int id)
        {
            var value = await _mediator.Send(new GetMainCategoryByIdQuery(id));
            return Ok(value);
        }
        [HttpPost]
        public async Task<IActionResult> CreateMainCategory(CreateMainCategoryCommand command)
        {
            await _mediator.Send(command);
            return Ok("Ana Kategori Bilgisi Başarıyla Oluşturuldu");
        }
        [HttpPut]
        public async Task<IActionResult> UpdateMainCategory(UpdateMainCategoryCommand command)
        {
            await _mediator.Send(command);
            return Ok("Ana Kategori Bilgisi Başarıyla Güncellendi");
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveMainCategory(int id)
        {
            await _mediator.Send(new RemoveMainCategoryCommand(id));
            return Ok("Ana Kategori Bilgisi Başarıyla Silindi");
        }
    }
}
