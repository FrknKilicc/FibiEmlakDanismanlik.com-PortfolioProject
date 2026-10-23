using FibiEmlakDanismanlik.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Infrastructure.Security;

/// <summary>
/// Cloudflare Turnstile bot doğrulama servisi.
/// </summary>
public class TurnstileCaptchaVerifier : ICaptchaVerifier
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TurnstileCaptchaVerifier> _logger;

    public TurnstileCaptchaVerifier(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        ILogger<TurnstileCaptchaVerifier> logger)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<bool> VerifyAsync(string? token, string? remoteIp, CancellationToken cancellationToken)
    {
        var enabledStr = _configuration["Captcha:Enabled"];
        var isEnabled = string.IsNullOrEmpty(enabledStr) || (bool.TryParse(enabledStr, out var enabled) ? enabled : true);

        // Captcha devre dışı bırakılmışsa doğrulamayı doğrudan başarılı say
        if (!isEnabled)
            return true;

        var secret = _configuration["Captcha:TurnstileSecretKey"];
        var verifyUrl = _configuration["Captcha:VerifyUrl"];

        // Gizli anahtar veya doğrulama adresi eksikse fail-closed (kapalı başarısızlık)
        if (string.IsNullOrWhiteSpace(secret) || string.IsNullOrWhiteSpace(verifyUrl))
        {
            _logger.LogWarning("Turnstile doğrulaması etkin ancak yapılandırma eksik (SecretKey veya VerifyUrl boş). Doğrulama başarısız kabul edildi.");
            return false;
        }

        // İstekten gelen token boşsa başarısız kabul et
        if (string.IsNullOrWhiteSpace(token))
        {
            _logger.LogWarning("Turnstile belirteci (token) boş veya sağlanmamış. Doğrulama başarısız kabul edildi.");
            return false;
        }

        try
        {
            var client = _httpClientFactory.CreateClient("turnstile");

            var parameters = new Dictionary<string, string>
            {
                { "secret", secret },
                { "response", token }
            };

            if (!string.IsNullOrWhiteSpace(remoteIp))
            {
                parameters["remoteip"] = remoteIp;
            }

            using var content = new FormUrlEncodedContent(parameters);
            using var response = await client.PostAsync(verifyUrl, content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Turnstile uç noktası başarısız HTTP durum kodu döndürdü: {StatusCode}", response.StatusCode);
                return false;
            }

            var jsonStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var result = await JsonSerializer.DeserializeAsync<TurnstileResponse>(jsonStream, cancellationToken: cancellationToken);

            if (result != null && result.Success)
            {
                return true;
            }

            _logger.LogWarning("Turnstile bot doğrulaması başarısız oldu (success=false).");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Turnstile doğrulaması sırasında ağ veya JSON ayrıştırma hatası oluştu.");
            return false;
        }
    }

    private sealed class TurnstileResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
    }
}
