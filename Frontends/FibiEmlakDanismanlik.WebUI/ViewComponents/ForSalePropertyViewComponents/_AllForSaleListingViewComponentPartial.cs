using FibiEmlakDanismanlik.Dto.PropertyDtos;
using FibiEmlakDanismanlik.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
            var api = _configuration["Url:ApiUrl"];
            
            var selectedIds = new List<int>();
            if (HttpContext.Request.Query.TryGetValue("listingTypeIds", out var values))
            {
                foreach (var v in values)
                    if (int.TryParse(v, out var id))
                        selectedIds.Add(id);
            }
           
            var facetResp = await client.GetAsync($"{api}ForSales/GetForSaleListingTypeFacets");
            var facets = new List<ListingTypeFacetVm>();
            if (facetResp.IsSuccessStatusCode)
            {
                var json = await facetResp.Content.ReadAsStringAsync();
                facets = JsonConvert.DeserializeObject<List<ListingTypeFacetVm>>(json) ?? new();
            }

            foreach (var f in facets)
                f.Selected = selectedIds.Contains(f.ListingTypeId);

          
            var listUrl = $"{api}ForSales/GetAllForSalesPropertyForListing";
            if (selectedIds.Any())
            {
                var qs = string.Join("&", selectedIds.Select(id => $"listingTypeIds={id}"));
                listUrl += "?" + qs;
            }

            var listResp = await client.GetAsync(listUrl);
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
                SelectedListingTypeIds = selectedIds
            };

            return View(vm);
        }
    }
}
