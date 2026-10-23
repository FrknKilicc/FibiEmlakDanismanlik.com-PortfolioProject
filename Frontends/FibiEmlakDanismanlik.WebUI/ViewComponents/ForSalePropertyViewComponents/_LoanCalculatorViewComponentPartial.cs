using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Dto.BankLoanRateDtos;
using FibiEmlakDanismanlik.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.ForSalePropertyViewComponents
{
    /// <summary>Satılık ilan detayında banka kredi hesaplayıcı bileşeni.</summary>
    public class _LoanCalculatorViewComponentPartial : ViewComponent
    {
        private const string CacheKey = "BankLoanRates_ActiveCache";
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;

        public _LoanCalculatorViewComponentPartial(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IMemoryCache memoryCache)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _memoryCache = memoryCache;
        }

        public async Task<IViewComponentResult> InvokeAsync(decimal price, string? listingType)
        {
            if (price <= 0)
            {
                return Content(string.Empty);
            }

            if (!_memoryCache.TryGetValue(CacheKey, out List<ResultBankLoanRateDto>? rates) || rates == null || rates.Count == 0)
            {
                try
                {
                    var client = _httpClientFactory.CreateClient();
                    var apiUrl = _configuration["Url:ApiUrl"];
                    if (string.IsNullOrWhiteSpace(apiUrl))
                    {
                        return Content(string.Empty);
                    }

                    var response = await client.GetAsync($"{apiUrl}BankLoanRates");
                    if (!response.IsSuccessStatusCode)
                    {
                        return Content(string.Empty);
                    }

                    var jsonData = await response.Content.ReadAsStringAsync();
                    rates = JsonConvert.DeserializeObject<List<ResultBankLoanRateDto>>(jsonData);

                    if (rates == null || rates.Count == 0)
                    {
                        return Content(string.Empty);
                    }

                    _memoryCache.Set(CacheKey, rates, TimeSpan.FromMinutes(30));
                }
                catch (HttpRequestException)
                {
                    return Content(string.Empty);
                }
                catch (JsonException)
                {
                    return Content(string.Empty);
                }
                catch (TaskCanceledException)
                {
                    return Content(string.Empty);
                }
            }

            var isHousing = string.Equals(listingType?.Trim(), "Konut", StringComparison.OrdinalIgnoreCase);

            var model = new LoanCalculatorVm
            {
                Price = price,
                IsHousing = isHousing,
                Rates = rates
            };

            return View(model);
        }
    }
}
