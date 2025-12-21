using FibiEmlakDanismanlik.Dto.BlogDtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

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

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("/Blog/Detail/{id:int}")]
        public IActionResult Detail(int id)
        {
            ViewBag.BlogId = id;    
            return View();

        }
        [HttpGet]
        public async Task<IActionResult> Suggest(string q, int take = 6)
        {
            if(string.IsNullOrWhiteSpace(q)|| q.Trim().Length < 2)
            {
                return Json(new List<BlogSuggestionDto>());
            }
            if(take<=0) take = 6;
            if(take>10) take = 10;

            var client = _httpClientFactory.CreateClient();
            var apiUrl = _configuration["Url:ApiUrl"];
            var url = $"{apiUrl}Blog/SearchSuggestions?q={Uri.EscapeDataString(q.Trim())}&take={take}";
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return Json(new List<BlogSuggestionDto>());
            }
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<List<BlogSuggestionDto>>(json) ?? new ();
            return Json(data);
        }

    }
}
