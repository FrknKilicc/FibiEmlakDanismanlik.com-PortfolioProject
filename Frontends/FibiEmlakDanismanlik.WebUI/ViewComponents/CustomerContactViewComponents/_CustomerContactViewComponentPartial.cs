using FibiEmlakDanismanlik.Dto.ContactDtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.CustomerContactViewComponents
{
    public class _CustomerContactViewComponentPartial: ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public _CustomerContactViewComponentPartial(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync(ResultCustomerContactDto dto)
        {
            bool isEmpty =
            string.IsNullOrWhiteSpace(dto?.CustomerContactName) &&
            string.IsNullOrWhiteSpace(dto?.CustomerContactMail) &&
            string.IsNullOrWhiteSpace(dto?.CustomerContactPhone) &&
            string.IsNullOrWhiteSpace(dto?.CustomerContactMessage);

            if (isEmpty)
            {
                return View(new ResultCustomerContactDto()); 
            }

            
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(dto);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var responseMessage = await client.PostAsync("https://localhost:7015/api/CustomerContact", content);

            if (responseMessage.IsSuccessStatusCode)
            {
                ViewBag.Message = "Talebiniz başarıyla alındı.";
            }

            return View(dto);
        }
    }
    }

