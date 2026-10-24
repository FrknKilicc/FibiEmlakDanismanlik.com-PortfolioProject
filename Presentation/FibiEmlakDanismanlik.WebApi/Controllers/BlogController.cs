using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.BlogCommands;
using FibiEmlakDanismanlik.Application.Features.Handlers.BlogHandlers;
using FibiEmlakDanismanlik.Application.Features.Queries.BlogQueries;
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

        [HttpGet("categories")]
        public async Task<IActionResult> GetBlogCategories()
        {
            var values = await _mediator.Send(new GetBlogCategoriesQuery());
            return Ok(values);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBlogById(int id)
        {
            var value = await _mediator.Send(new GetBlogByIdQuery(id));
            if (value == null)
            {
                return NotFound();
            }
            return Ok(value);
        }

        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpPost]
        public async Task<IActionResult> CreateBlog(CreateBlogCommand command)
        {
            await _mediator.Send(command);
            return Ok("Blog Bilgisi Başarıyla Eklendi");
        }

        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpPut]
        public async Task<IActionResult> UpdateBlog(UpdateBlogCommand command)
        {
            try
            {
                await _mediator.Send(command);
                return Ok("Blog Bilgisi Başarıyla Güncellendi");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpDelete]
        public async Task<IActionResult> RemoveBlog(int id)
        {
            try
            {
                await _mediator.Send(new RemoveBlogCommand(id));
                return Ok("Blog Bilgisi Başarıyla Silindi");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
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
        public async Task<IActionResult> GetBlogListWithAuthor([FromQuery] int? blogCategoryId)
        {
            {
                var values = await _mediator.Send(new GetBlogListWithAuthorQuery(blogCategoryId));
                return Ok(values);
            }
        }

        [HttpGet("SearchSuggestions")]
        public async Task<IActionResult> SearchSuggestions([FromQuery] string q, [FromQuery] int take = 6)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Trim().Length < 2)
            {
                return Ok(new List<object>());
            }
            var result = await _mediator.Send(new GetBlogSuggestionsQuery(q, take));
            return Ok(result);
        }

        [HttpGet("GetTopBlogCategories")]
        public async Task<IActionResult> GetTopBlogCategories([FromQuery] int take = 5)
        {
            var values = await _mediator.Send(new GetTopBlogCategoriesQuery(take));
            return Ok(values);
        }
    }
}
