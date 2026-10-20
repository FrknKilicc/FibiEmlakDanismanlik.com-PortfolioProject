using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.WebUI.Services
{
    public class FormSpamGuard
    {
        public const string HoneypotFieldName = "Website";

        private static readonly object _rateLimitLock = new();

        private readonly IMemoryCache _memoryCache;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly int _permitCount;
        private readonly int _windowMinutes;
        private readonly string? _turnstileSiteKey;
        private readonly string? _turnstileSecretKey;
        private readonly string? _captchaScriptUrl;
        private readonly string? _captchaVerifyUrl;

        public FormSpamGuard(IMemoryCache memoryCache, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _memoryCache = memoryCache;
            _httpClientFactory = httpClientFactory;

            var permitCountStr = configuration["FormSpam:PermitCount"];
            _permitCount = int.TryParse(permitCountStr, out var pc) && pc > 0 ? pc : 5;

            var windowMinutesStr = configuration["FormSpam:WindowMinutes"];
            _windowMinutes = int.TryParse(windowMinutesStr, out var wm) && wm > 0 ? wm : 10;

            _turnstileSiteKey = configuration["Captcha:TurnstileSiteKey"];
            _turnstileSecretKey = configuration["Captcha:TurnstileSecretKey"];
            _captchaScriptUrl = configuration["Captcha:ScriptUrl"];
            _captchaVerifyUrl = configuration["Captcha:VerifyUrl"];
        }

        public bool CaptchaEnabled =>
            !string.IsNullOrWhiteSpace(_turnstileSiteKey) &&
            !string.IsNullOrWhiteSpace(_turnstileSecretKey) &&
            !string.IsNullOrWhiteSpace(_captchaScriptUrl) &&
            !string.IsNullOrWhiteSpace(_captchaVerifyUrl);

        public string? CaptchaSiteKey => _turnstileSiteKey;
        public string? CaptchaScriptUrl => _captchaScriptUrl;

        public bool TryConsume(HttpContext context, string formKey)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var key = $"form-rate:{formKey}:{ip}";

            lock (_rateLimitLock)
            {
                if (!_memoryCache.TryGetValue(key, out RateLimitCounter? counter) || counter == null)
                {
                    counter = new RateLimitCounter { Count = 1 };
                    var options = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_windowMinutes)
                    };
                    _memoryCache.Set(key, counter, options);
                    return 1 <= _permitCount;
                }

                counter.Count++;
                return counter.Count <= _permitCount;
            }
        }

        public bool IsHoneypotFilled(HttpContext context)
        {
            if (!context.Request.HasFormContentType)
                return false;

            if (context.Request.Form.TryGetValue(HoneypotFieldName, out var value))
            {
                return !string.IsNullOrWhiteSpace(value.ToString());
            }

            return false;
        }

        public async Task<bool> VerifyCaptchaAsync(HttpContext context)
        {
            if (!CaptchaEnabled)
                return true;

            try
            {
                if (!context.Request.HasFormContentType)
                    return false;

                var responseToken = context.Request.Form["cf-turnstile-response"].ToString();
                if (string.IsNullOrWhiteSpace(responseToken))
                    return false;

                var remoteIp = context.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

                var client = _httpClientFactory.CreateClient();
                var postData = new Dictionary<string, string>
                {
                    ["secret"] = _turnstileSecretKey!,
                    ["response"] = responseToken,
                    ["remoteip"] = remoteIp
                };

                using var content = new FormUrlEncodedContent(postData);
                var response = await client.PostAsync(_captchaVerifyUrl!, content);
                if (!response.IsSuccessStatusCode)
                    return false;

                var jsonString = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(jsonString);
                return json["success"]?.Value<bool>() ?? false;
            }
            catch
            {
                return false;
            }
        }

        private class RateLimitCounter
        {
            public int Count { get; set; }
        }
    }
}
