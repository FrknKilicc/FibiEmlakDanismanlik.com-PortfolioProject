using System.Net;
using FibiEmlakDanismanlik.Dto.PropertyDtos;
using FibiEmlakDanismanlik.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FibiEmlakDanismanlik.WebUI.Controllers
{
    public class AllForRentalDetailPageController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AllForRentalDetailPageController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        /// <summary>
        /// İlan yoksa null döner; API hatası istisna olarak yukarı çıkar (geçici hatada 404 dönüp arama motorunun sayfayı düşürmesini önlemek için).
        /// </summary>
        private async Task<ResultAllForRentalListingForPageDto?> FetchAsync(int id, int? listingTypeId)
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"{_configuration["Url:ApiUrl"]}ForRental/GetUnifiedForRentalPropertyById/{id}";
            if (listingTypeId.HasValue && listingTypeId.Value > 0)
            {
                url += $"?listingTypeId={listingTypeId.Value}";
            }

            var response = await client.GetAsync(url);
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

            var dto = JsonConvert.DeserializeObject<ResultAllForRentalListingForPageDto>(jsonData);
            if (dto == null)
            {
                return null;
            }

            dto.ListingId = id;
            if (string.IsNullOrWhiteSpace(dto.ListingType))
            {
                dto.ListingType = dto.ListingTypeName;
            }

            return dto;
        }

        private static string CanonicalPath(ResultAllForRentalListingForPageDto dto)
        {
            return ListingUrl.DetailPath(true, dto.ListingId, dto.ListingTypeId, dto.ListingType, dto.PropertyName, dto.District);
        }

        public async Task<IActionResult> Index(int id, int? listingTypeId)
        {
            var dto = await FetchAsync(id, listingTypeId);
            if (dto == null)
            {
                return NotFound();
            }

            return RedirectPermanent(CanonicalPath(dto));
        }

        [HttpGet("kiralik/{category}/{slugAndId}")]
        public async Task<IActionResult> Detail(string category, string slugAndId)
        {
            if (!ListingUrl.TryParseId(slugAndId, out var id))
            {
                return NotFound();
            }

            var listingTypeId = ListingUrl.ListingTypeIdFromCategory(true, category);
            var dto = await FetchAsync(id, listingTypeId);
            if (dto == null)
            {
                return NotFound();
            }

            var canonical = CanonicalPath(dto);
            if (!string.Equals(Request.Path.Value, canonical, StringComparison.Ordinal))
            {
                return RedirectPermanent(canonical);
            }

            HttpContext.Items[PageMeta.CanonicalItemKey] = canonical;
            return View("Index", dto);
        }
    }
}
