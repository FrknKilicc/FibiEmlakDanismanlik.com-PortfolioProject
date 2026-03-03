using FibiEmlakDanismanlik.Application.Features.Commands.CustomerContactCommands;
using FibiEmlakDanismanlik.Dto.ContactDtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace FibiEmlakDanismanlik.WebUI.Controllers
{
    public class ContactController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ContactController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var dto = TempData["FormResult"] != null
                ? JsonConvert.DeserializeObject<ResultCustomerContactDto>((string)TempData["FormResult"])
                : new ResultCustomerContactDto();

            return View(dto);
        }


        [HttpPost]
        public async Task<IActionResult> CustomerContactForm(ResultCustomerContactDto dto)
        {
            TempData["FormResult"] = JsonConvert.SerializeObject(dto);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SendAppointment(
      CreateCustomerContactCommand model,
      string date,
      string time)
        {
            if (!string.IsNullOrEmpty(date) && !string.IsNullOrEmpty(time))
            {
                model.AppointmentDateTime = DateTime.ParseExact(
                    $"{date} {time}",
                    "dd.MM.yyyy HH:mm",
                    System.Globalization.CultureInfo.InvariantCulture);
            }
            var client = _httpClientFactory.CreateClient();
            var apiUrl = _configuration["Url:ApiUrl"];
            var response = await client.PostAsJsonAsync($"{apiUrl}CustomerContact", model);
            var body = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"POST URL: {apiUrl}CustomerContact");
            Console.WriteLine($"API Status: {(int)response.StatusCode} {response.StatusCode}");
            Console.WriteLine($"API Body: {body}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Randevu talebi gönderilemedi";
                return Redirect(Request.Headers["Referer"].ToString());
            }
            TempData["Message"] = "Randevu talebiniz alınmıştır.";

            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
