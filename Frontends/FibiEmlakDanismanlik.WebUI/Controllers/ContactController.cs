using FibiEmlakDanismanlik.Application.Features.Commands.CustomerContactCommands;
using FibiEmlakDanismanlik.Dto.ContactDtos;
using FibiEmlakDanismanlik.WebUI.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.WebUI.Controllers
{
    public class ContactController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly FormSpamGuard _formSpamGuard;

        public ContactController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            FormSpamGuard formSpamGuard)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _formSpamGuard = formSpamGuard;
        }

        public IActionResult Index()
        {
            return View(new ResultCustomerContactDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CustomerContactForm(ResultCustomerContactDto dto)
        {
            if (_formSpamGuard.IsHoneypotFilled(HttpContext))
            {
                TempData["MessageType"] = "success";
                TempData["Message"] = "Mesajınız alındı, en kısa sürede size dönüş yapacağız.";
                return RedirectToAction("Index");
            }

            if (!_formSpamGuard.TryConsume(HttpContext, "contact"))
            {
                TempData["MessageType"] = "warning";
                TempData["Message"] = "Çok fazla deneme yaptınız. Lütfen birkaç dakika sonra tekrar deneyin.";
                return RedirectToAction("Index");
            }

            if (!await _formSpamGuard.VerifyCaptchaAsync(HttpContext))
            {
                TempData["MessageType"] = "error";
                TempData["Message"] = "Doğrulama başarısız oldu, lütfen tekrar deneyin.";
                return RedirectToAction("Index");
            }

            if (!dto.KvkkApproved)
            {
                TempData["MessageType"] = "error";
                TempData["Message"] = "Lütfen KVKK ve Gizlilik Politikası metnini onaylayınız.";
                return RedirectToAction("Index");
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                var jsonData = JsonConvert.SerializeObject(dto);
                using var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                var responseMessage = await client.PostAsync($"{_configuration["Url:ApiUrl"]}CustomerContact", content);
                if (responseMessage.IsSuccessStatusCode)
                {
                    TempData["MessageType"] = "success";
                    TempData["Message"] = "Mesajınız alındı, en kısa sürede size dönüş yapacağız.";
                }
                else
                {
                    TempData["MessageType"] = "error";
                    TempData["Message"] = "Mesajınız gönderilemedi, lütfen tekrar deneyin.";
                }
            }
            catch (Exception)
            {
                TempData["MessageType"] = "error";
                TempData["Message"] = "İşlem sırasında beklenmeyen bir hata oluştu";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendAppointment(
            CreateCustomerContactCommand model,
            string date,
            string time)
        {
            var returnUrl = GetSafeRefererUrl();

            if (_formSpamGuard.IsHoneypotFilled(HttpContext))
            {
                TempData["MessageType"] = "success";
                TempData["Message"] = "Randevu talebiniz alınmıştır";
                return Redirect(returnUrl);
            }

            if (!_formSpamGuard.TryConsume(HttpContext, "appointment"))
            {
                TempData["MessageType"] = "warning";
                TempData["Message"] = "Çok fazla deneme yaptınız. Lütfen birkaç dakika sonra tekrar deneyin.";
                return Redirect(returnUrl);
            }

            if (!await _formSpamGuard.VerifyCaptchaAsync(HttpContext))
            {
                TempData["MessageType"] = "error";
                TempData["Message"] = "Doğrulama başarısız oldu, lütfen tekrar deneyin.";
                return Redirect(returnUrl);
            }

            if (!model.KvkkApproved)
            {
                TempData["MessageType"] = "error";
                TempData["Message"] = "Lütfen KVKK ve Gizlilik Politikası metnini onaylayınız.";
                return Redirect(returnUrl);
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(date) && !string.IsNullOrWhiteSpace(time))
                {
                    model.AppointmentDateTime = DateTime.ParseExact(
                        $"{date} {time}",
                        "dd.MM.yyyy HH:mm",
                        System.Globalization.CultureInfo.InvariantCulture);
                }

                var client = _httpClientFactory.CreateClient();
                var apiUrl = _configuration["Url:ApiUrl"];
                var response = await client.PostAsJsonAsync($"{apiUrl}CustomerContact", model);

                if (!response.IsSuccessStatusCode)
                {
                    TempData["MessageType"] = "error";
                    TempData["Message"] = "Randevu talebi gönderilemedi";

                    return Redirect(returnUrl);
                }

                TempData["MessageType"] = "success";
                TempData["Message"] = "Randevu talebiniz alınmıştır";

                return Redirect(returnUrl);
            }
            catch (Exception)
            {
                TempData["MessageType"] = "error";
                TempData["Message"] = "İşlem sırasında beklenmeyen bir hata oluştu";

                return Redirect(returnUrl);
            }
        }

        private string GetSafeRefererUrl()
        {
            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrWhiteSpace(referer) &&
                Uri.TryCreate(referer, UriKind.Absolute, out var uri) &&
                string.Equals(uri.Host, Request.Host.Host, StringComparison.OrdinalIgnoreCase))
            {
                return uri.PathAndQuery;
            }

            return "/";
        }
    }
}
