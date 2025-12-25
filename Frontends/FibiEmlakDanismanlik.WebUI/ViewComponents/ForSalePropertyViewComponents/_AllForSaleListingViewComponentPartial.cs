using FibiEmlakDanismanlik.Dto.PropertyDtos;
using FibiEmlakDanismanlik.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using FibiEmlakDanismanlik.Application.Features.Requests.PropertyRequests;


namespace FibiEmlakDanismanlik.WebUI.ViewComponents.ForSalePropertyViewComponents
{
    public class _AllForSaleListingViewComponentPartial:ViewComponent
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        public _AllForSaleListingViewComponentPartial(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public  async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var apiUrl = _configuration["Url:ApiUrl"];
            ViewBag.ApiUrl = apiUrl;

            // 1) listingTypeIds
            var selectedIds = new List<int>();
            if (HttpContext.Request.Query.TryGetValue("listingTypeIds", out var values))
            {
                foreach (var v in values)
                    if (int.TryParse(v, out var id))
                        selectedIds.Add(id);
            }

            // 2) other query params
            int? cityId = TryInt("cityId");
            int? districtId = TryInt("districtId");
            int? neighborhoodId = TryInt("neighborhoodId");

            string? city = TryStr("city");
            string? district = TryStr("district");
            string? neighborhood = TryStr("neighborhood");

            decimal? minPrice = TryDecimal("minPrice");
            decimal? maxPrice = TryDecimal("maxPrice");
            string? numberOfRoom = TryStr("numberOfRoom");

            var sortBy = TryStr("sortBy") ?? "CreatedDate";
            var sortDir = TryStr("sortDir") ?? "desc";

            // 3) facets
            var facetResp = await client.GetAsync($"{apiUrl}ForSales/GetForSaleListingTypeFacets");
            var facets = new List<ListingTypeFacetVm>();
            if (facetResp.IsSuccessStatusCode)
            {
                var json = await facetResp.Content.ReadAsStringAsync();
                facets = JsonConvert.DeserializeObject<List<ListingTypeFacetVm>>(json) ?? new();
            }
            foreach (var f in facets)
                f.Selected = selectedIds.Contains(f.ListingTypeId);

            var request = new PropertyFilterRequest
            {
                ListingTypeIds = selectedIds.Any() ? selectedIds : null,
                City = city,
                District = district,
                Neighborhood = neighborhood,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                NumberOfRoom = numberOfRoom,
                SortBy = sortBy,
                SortDir = sortDir,
                Page = 1,
                PageSize = 20
            };

            var body = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var listResp = await client.PostAsync($"{apiUrl}ForSales/filter", body);

            var items = new List<ResultAllForSaleListinForPageDto>();
            if (listResp.IsSuccessStatusCode)
            {
                var json = await listResp.Content.ReadAsStringAsync();
                items = JsonConvert.DeserializeObject<List<ResultAllForSaleListinForPageDto>>(json) ?? new();
            }

            var vm = new ForSaleListingPageVm
            {
                ListingTypeFacets = facets,
                Items = items,
                SelectedListingTypeIds = selectedIds,

                CityId = cityId,
                DistrictId = districtId,
                NeighborhoodId = neighborhoodId,

                City = city,
                District = district,
                Neighborhood = neighborhood,

                MinPrice = minPrice,
                MaxPrice = maxPrice,
                NumberOfRoom = numberOfRoom,
                SortBy = sortBy,
                SortDir = sortDir
            };

            return View(vm);

            int? TryInt(string key) => int.TryParse(HttpContext.Request.Query[key], out var x) ? x : null;
            decimal? TryDecimal(string key) => decimal.TryParse(HttpContext.Request.Query[key], out var x) ? x : null;
            string? TryStr(string key)
            {
                var v = HttpContext.Request.Query[key].ToString();
                return string.IsNullOrWhiteSpace(v) ? null : v;
            }

        }
    }
}



