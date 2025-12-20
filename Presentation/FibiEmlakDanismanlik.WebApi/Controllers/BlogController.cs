using FibiEmlakDanismanlik.Application.Features.Commands.BlogCommands;
using FibiEmlakDanismanlik.Application.Features.Handlers.BlogHandlers;
using FibiEmlakDanismanlik.Application.Features.Queries.BlogQueries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BlogController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> GetBlogList()
        {
            var value = await _mediator.Send(new GetBlogQuery());
            return Ok(value);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlogById(int id)
        {
            var value = await _mediator.Send(new GetBlogByIdQuery(id));
            return Ok(value);
        }
        [HttpPost]
        public async Task<IActionResult> CreateBlog(CreateBlogCommand command)
        {
            await _mediator.Send(command);
            return Ok("Blog Bilgisi Başarıyla Eklendi");
        }
        [HttpPut]
        public async Task<IActionResult> UpdateBlog(UpdateBlogCommand command)
        {
            await _mediator.Send(command);
            return Ok("Blog Bilgisi Başarıyla Güncellendi");

        }
        [HttpDelete]
        public async Task<IActionResult> RemoveBlog(int id)
        {
            await _mediator.Send(new RemoveBlogCommand(id));
            return Ok("Blog Bilgisi Başarıyla Silindi");
        }

        [HttpGet("GetBlogDetailWithAuthorById")]
        public async Task<IActionResult> GetBlogDetailWithAuthorById(int id)
        {
            var value = await _mediator.Send(new GetBlogDetailWithAuthorByIdQuery(id));

            if (value == null)
            {
                return NotFound("Blog Detayına Erişilemiyor.");
            }
            return Ok(value);
        }

        [HttpGet("GetLast3BlogWithAuthor")]
        public async Task<IActionResult> GetLast3BlogWithAuthor()
        {
            var value = await _mediator.Send(new GetBlogDetailWithAuthorQuery());
            if (value == null)
            {
                return NotFound("Blog Detayına Erişilemiyor");
            }
            return Ok(value);
        }

        [HttpGet("GetBlogListWithAuthor")]

        public async Task<IActionResult> GetBlogListWithAuthor()
        {
            var value = await _mediator.Send(new GetBlogListWithAuthorQuery());
            if (value == null)
            {
                return NotFound("Blog Detayına erişilemiyor");
                
            }

            return Ok(value);
        }
    }
}
