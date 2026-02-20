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
        private static int ReadIntQuery(HttpRequest request, string key, int defaultValue)
        {
            if (!request.Query.TryGetValue(key, out var v)) return defaultValue;
            if (int.TryParse(v.ToString(), out var parsed) && parsed > 0) return parsed;
            return defaultValue;
        }
        private static int ReadIntQuery(HttpRequest request, string key, int defaultValue, int min, int max)
        {
            var val = ReadIntQuery(request, key, defaultValue);
            if (val < min) return min;
            if (val > max) return max;
            return val;
        }


        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var apiUrl = _configuration["Url:ApiUrl"];
            ViewBag.ApiUrl = apiUrl;

            //  listingTypeIds
            var selectedIds = new List<int>();
            if (HttpContext.Request.Query.TryGetValue("listingTypeIds", out var values))
            {
                foreach (var v in values)
                    if (int.TryParse(v, out var id))
                        selectedIds.Add(id);
            }
            //seçilen özellik idsi 
            var selectedAmenityIds = new List<int>();
            if (HttpContext.Request.Query.TryGetValue("selectedAmenityIds", out var amenityValues))
            {
                foreach (var v in amenityValues)
                    if (int.TryParse(v, out var id))
                        selectedAmenityIds.Add(id);
            }

            // sol framde bulunan sorgu parametreleri 
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
            var page = ReadIntQuery(Request, "page", 1, 1, int.MaxValue);
            var pageSize = ReadIntQuery(Request, "pageSize", 3, 1, 100);


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
                SelectedAmenityIds = selectedAmenityIds.Any() ? selectedAmenityIds : null,
                City = city,
                District = district,
                Neighborhood = neighborhood,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                NumberOfRoom = numberOfRoom,
                SortBy = sortBy,
                SortDir = sortDir,
                Page = page,
                PageSize = pageSize
                
                
            };


            var body = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var apiBase = apiUrl!.TrimEnd('/') + "/";
            var listResp = await client.PostAsync($"{apiBase}ForSales/filter", body);
            var items = new List<ResultAllForSaleListinForPageDto>();
            var total = 0;
            var amenities = new List<AmenityFacetDto>(); 

            if (listResp.IsSuccessStatusCode)
            {
                var json = await listResp.Content.ReadAsStringAsync();
                var respObj = JsonConvert.DeserializeObject<ForSaleFilterResponseDto>(json);

                if (respObj != null)
                {
                    items = respObj.Items ?? new();
                    total = respObj.Total;
                    amenities = respObj.Amenties ?? new();
                    request.Page = page;
                    request.PageSize = pageSize;
                }
            }

            var vm = new ForSaleListingPageVm
            {
                ListingTypeFacets = facets,
                Items = items,
                Total = total,
                SelectedListingTypeIds = selectedIds,

                Amenities = amenities, 
                

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
                SortDir = sortDir,
                Page = page,
                PageSize = pageSize,
                
                
                
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



