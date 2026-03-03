using FibiEmlakDanismanlik.WebUI.Models.Nearby;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.NearbyViewComponents
{
    public class _NearbyPlacesViewComponentPartial : ViewComponent
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public _NearbyPlacesViewComponentPartial(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(int listingId, int ListingTypeId)
        {
            if (listingId <= 0 || ListingTypeId <= 0)
                return Content(string.Empty);
            var client = _httpClientFactory.CreateClient();
            var url = $"{_configuration["Url:ApiUrl"]}Nearby/GetByListing?listingId={listingId}&ListingTypeId={ListingTypeId}";
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return Content(string.Empty);

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<List<NearbyCategoryGroupVm>>(json) ?? new();

            if (data.Count == 0)
                return Content(string.Empty);
            return View(data);
        }
    }
}
