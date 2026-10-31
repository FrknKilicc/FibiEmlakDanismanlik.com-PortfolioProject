using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.WebUI.Models;
using FibiEmlakDanismanlik.WebUI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace FibiEmlakDanismanlik.WebUI.Controllers
{
    /// <summary>
    /// Kullanıcıların ilgilendikleri kriterlere uygun yeni ilanlar yayınlandığında haberdar edilmesini sağlayan kontrolcü.
    /// </summary>
    public class ListingAlertController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly FormSpamGuard _formSpamGuard;
        private readonly ListingLocationResolver _listingLocationResolver;

        public ListingAlertController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            FormSpamGuard formSpamGuard,
            ListingLocationResolver listingLocationResolver)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _formSpamGuard = formSpamGuard;
            _listingLocationResolver = listingLocationResolver;
        }

        /// <summary>
        /// Benzer ilan bildirim form sayfasını görüntüler.
        /// </summary>
        [HttpGet("haber-ver")]
        public IActionResult Index(
            int? tur,
            int? cityId,
            int? districtId,
            decimal? min,
            decimal? max,
            string? oda)
        {
            var listingTypeId = tur.HasValue && ListingAlertFormVm.ListingTypeOptions.ContainsKey(tur.Value)
                ? tur.Value
                : 2;

            var model = new ListingAlertFormVm
            {
                ListingTypeId = listingTypeId,
                CityId = cityId,
                DistrictId = districtId,
                MinPrice = min,
                MaxPrice = max
            };

            if (!string.IsNullOrWhiteSpace(oda) &&
                ListingAlertFormVm.IsHousing(listingTypeId) &&
                ListingAlertFormVm.RoomOptions.Contains(oda.Trim()))
            {
                model.RoomCounts.Add(oda.Trim());
            }

            ViewBag.ApiUrl = _configuration["Url:ApiUrl"];

            HttpContext.Items[PageMeta.TitleItemKey] = "Benzer İlan Çıkınca Haber Ver";
            HttpContext.Items[PageMeta.DescriptionItemKey] = "Aradığınız kriterlere uygun yeni bir satılık veya kiralık gayrimenkul ilanı yayınlandığında haberdar olun.";

            return View(model);
        }

        /// <summary>
        /// Benzer ilan bildirim talebini doğrular ve API servisine iletir.
        /// </summary>
        [HttpPost("haber-ver")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(ListingAlertFormVm model, string? returnUrl)
        {
            var targetUrl = GetSafeRedirectUrl(returnUrl);

            if (_formSpamGuard.IsHoneypotFilled(HttpContext))
            {
                TempData["MessageType"] = "success";
                TempData["Message"] = "Talebiniz alındı. Uygun ilan çıktığında size haber vereceğiz.";
                return Redirect(targetUrl);
            }

            if (!_formSpamGuard.TryConsume(HttpContext, "listing-alert"))
            {
                TempData["MessageType"] = "warning";
                TempData["Message"] = "Çok fazla deneme yaptınız. Lütfen birkaç dakika sonra tekrar deneyin.";
                return Redirect(targetUrl);
            }

            if (!await _formSpamGuard.VerifyCaptchaAsync(HttpContext))
            {
                TempData["MessageType"] = "error";
                TempData["Message"] = "Doğrulama başarısız oldu, lütfen tekrar deneyin.";
                return Redirect(targetUrl);
            }

            // Detay sayfası widget'ı il/ilçe adı gönderdiğinde kimlik çözümlemesi yapılır
            if (!model.CityId.HasValue && !string.IsNullOrWhiteSpace(model.CityName))
            {
                var (resolvedCityId, resolvedDistrictId) = await _listingLocationResolver.ResolveIdsByNameAsync(model.CityName, model.DistrictName);
                model.CityId = resolvedCityId;
                if (!model.DistrictId.HasValue && resolvedDistrictId.HasValue)
                {
                    model.DistrictId = resolvedDistrictId;
                }
            }

            if (!model.CityId.HasValue || model.CityId.Value <= 0)
            {
                TempData["MessageType"] = "error";
                TempData["Message"] = "Lütfen geçerli bir il seçiniz.";
                return Redirect(targetUrl);
            }

            if (!model.KvkkApproved)
            {
                TempData["MessageType"] = "error";
                TempData["Message"] = "Lütfen KVKK ve Gizlilik Politikası metnini onaylayınız.";
                return Redirect(targetUrl);
            }

            // Konut dışı ilan türlerinde oda seçimi geçerli olmadığından temizlenir
            if (!ListingAlertFormVm.IsHousing(model.ListingTypeId))
            {
                model.RoomCounts.Clear();
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                var apiUrl = _configuration["Url:ApiUrl"];
                var payload = new
                {
                    fullName = model.FullName?.Trim() ?? string.Empty,
                    phoneNumber = model.PhoneNumber?.Trim() ?? string.Empty,
                    email = string.IsNullOrWhiteSpace(model.Email) ? null : model.Email.Trim(),
                    listingTypeId = model.ListingTypeId,
                    cityId = model.CityId,
                    districtId = model.DistrictId,
                    minPrice = model.MinPrice,
                    maxPrice = model.MaxPrice,
                    roomCounts = model.RoomCounts ?? new List<string>(),
                    note = string.IsNullOrWhiteSpace(model.Note) ? null : model.Note.Trim(),
                    sourceListingTypeId = model.SourceListingTypeId,
                    sourceListingId = model.SourceListingId,
                    kvkkApproved = model.KvkkApproved
                };

                var response = await client.PostAsJsonAsync($"{apiUrl}ListingAlerts", payload);
                if (response.IsSuccessStatusCode)
                {
                    TempData["MessageType"] = "success";
                    TempData["Message"] = "Talebiniz alındı. Uygun ilan çıktığında size haber vereceğiz.";
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var errorMessage = "Lütfen form bilgilerini kontrol edip tekrar deneyiniz.";
                    try
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var problem = JsonConvert.DeserializeObject<ApiValidationProblem>(content);
                        var firstError = problem?.Errors?.Values.SelectMany(x => x).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
                        if (!string.IsNullOrWhiteSpace(firstError))
                        {
                            errorMessage = firstError;
                        }
                    }
                    catch
                    {
                        // JSON ayrıştırılamazsa varsayılan hata mesajı kullanılır
                    }

                    TempData["MessageType"] = "error";
                    TempData["Message"] = errorMessage;
                }
                else
                {
                    TempData["MessageType"] = "error";
                    TempData["Message"] = "İşlem sırasında bir hata oluştu, lütfen tekrar deneyiniz.";
                }
            }
            catch (Exception)
            {
                // Kişisel veriyi loglamadan genel hata bildirimi döndürülür
                TempData["MessageType"] = "error";
                TempData["Message"] = "İşlem sırasında beklenmeyen bir hata oluştu.";
            }

            return Redirect(targetUrl);
        }

        private string GetSafeRedirectUrl(string? returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return returnUrl;
            }

            return "/haber-ver";
        }

        private sealed class ApiValidationProblem
        {
            public Dictionary<string, string[]>? Errors { get; set; }
        }
    }
}
