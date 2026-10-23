using System.Net;
using FibiEmlakDanismanlik.Dto.BlogDtos;
using FibiEmlakDanismanlik.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace FibiEmlakDanismanlik.WebUI.Controllers
{
    public class BlogController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public BlogController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpGet("blog")]
        public IActionResult Index(int? blogCategoryId, int page = 1, int pageSize = 3)
        {
            ViewBag.SelectedBlogCategoryId = blogCategoryId;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            return View();
        }

        [HttpGet("Blog/Index")]
        public IActionResult LegacyIndex()
        {
            return RedirectPermanent("/blog" + Request.QueryString);
        }

        private async Task<ResultBlogWithAuthorDto?> FetchAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_configuration["Url:ApiUrl"]}Blog/GetBlogDetailWithAuthorById?id={id}");
            if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.NoContent)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            var jsonData = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(jsonData))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<ResultBlogWithAuthorDto>(jsonData);
        }

        [HttpGet("blog/{slugAndId}")]
        public async Task<IActionResult> Detail(string slugAndId)
        {
            if (!ListingUrl.TryParseId(slugAndId, out var id))
            {
                return NotFound();
            }

            var dto = await FetchAsync(id);
            if (dto == null)
            {
                return NotFound();
            }

            var canonical = BlogUrl.DetailPath(dto.BlogId > 0 ? dto.BlogId : id, dto.BlogTitle);
            if (!string.Equals(Request.Path.Value, canonical, StringComparison.Ordinal))
            {
                return RedirectPermanent(canonical);
            }

            HttpContext.Items[PageMeta.CanonicalItemKey] = canonical;
            return View("Detail", dto);
        }

        [HttpGet("Blog/Detail/{id:int}")]
        public async Task<IActionResult> LegacyDetail(int id)
        {
            var dto = await FetchAsync(id);
            if (dto == null)
            {
                return NotFound();
            }

            return RedirectPermanent(BlogUrl.DetailPath(dto.BlogId > 0 ? dto.BlogId : id, dto.BlogTitle));
        }

        [HttpGet("blog/suggest")]
        public async Task<IActionResult> Suggest(string q, int take = 6)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Trim().Length < 2)
            {
                return Json(new List<BlogSuggestionDto>());
            }
            if (take <= 0) take = 6;
            if (take > 10) take = 10;

            var client = _httpClientFactory.CreateClient();
            var apiUrl = _configuration["Url:ApiUrl"];
            var url = $"{apiUrl}Blog/SearchSuggestions?q={Uri.EscapeDataString(q.Trim())}&take={take}";
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return Json(new List<BlogSuggestionDto>());
            }
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<List<BlogSuggestionDto>>(json) ?? new();
            return Json(data.Select(x => new
            {
                x.BlogId,
                x.BlogTitle,
                x.BlogDescPrew,
                x.BlogImgUrl,
                Url = BlogUrl.DetailPath(x.BlogId, x.BlogTitle)
            }));
        }
    }
}
