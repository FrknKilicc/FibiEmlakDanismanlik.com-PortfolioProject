using FibiEmlakDanismanlik.Dto.OurPartnersDtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.OurPartnersViewComponents
{
    public class _OurPartnersViewComponentPartial : ViewComponent
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public _OurPartnersViewComponentPartial(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var responseMessage = await client.GetAsync($"{_configuration["Url:ApiUrl"]}OurPartners/GetOurPartnersList");
                if (responseMessage.IsSuccessStatusCode)
                {
                    var jsonData = await responseMessage.Content.ReadAsStringAsync();
                    var values = JsonConvert.DeserializeObject<List<ResultOurPartnersDto>>(jsonData);
                    if (values != null)
                    {
                        var filteredList = values
                            .Where(x => !string.IsNullOrWhiteSpace(x.PartnerName) && !string.IsNullOrWhiteSpace(x.PartnerLogoUrl))
                            .OrderBy(x => x.Id)
                            .ToList();

                        return View(filteredList);
                    }
                }
            }
            catch
            {
                // API hatası durumunda boş liste dön
            }

            return View(new List<ResultOurPartnersDto>());
        }
    }
}
