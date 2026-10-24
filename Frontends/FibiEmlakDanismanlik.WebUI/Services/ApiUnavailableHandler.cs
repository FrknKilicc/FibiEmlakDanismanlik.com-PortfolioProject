using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.WebUI.Services
{
    /// <summary>
    /// API hizmetine bağlanılamadığında veya istek zaman aşımına uğradığında oluşan bağlantı hatalarını
    /// 503 (Service Unavailable) yanıtına çevirir; böylece çağıran bileşenlerin mevcut IsSuccessStatusCode
    /// kontrolleri boş içerik yoluna düşer ve sayfaların 500 hatası vermesi önlenir.
    /// Bağlantı hatası durumunda 15 saniyelik bir devre kesici (circuit breaker) devreye girerek
    /// bu süre zarfında API'ye tekrar istek atılmadan doğrudan 503 döndürülür.
    /// </summary>
    public class ApiUnavailableHandler : DelegatingHandler
    {
        private static readonly ConcurrentDictionary<string, DateTime> _unavailableUntilUtc = new();
        private static readonly TimeSpan BreakDuration = TimeSpan.FromSeconds(15);

        private readonly ILogger<ApiUnavailableHandler> _logger;

        public ApiUnavailableHandler(ILogger<ApiUnavailableHandler> logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authority = request.RequestUri?.GetLeftPart(UriPartial.Authority);
            if (authority != null && _unavailableUntilUtc.TryGetValue(authority, out var untilUtc) && untilUtc > DateTime.UtcNow)
            {
                return CreateUnavailableResponse(request);
            }

            try
            {
                var response = await base.SendAsync(request, cancellationToken);
                if (authority != null)
                {
                    _unavailableUntilUtc.TryRemove(authority, out _);
                }
                return response;
            }
            catch (HttpRequestException ex)
            {
                if (authority != null)
                {
                    _unavailableUntilUtc[authority] = DateTime.UtcNow + BreakDuration;
                }

                var path = request.RequestUri?.GetLeftPart(UriPartial.Path);
                _logger.LogWarning(ex, "API hizmetine bağlanılamadı (15 sn boyunca istek gönderilmeyecek): {Method} {Path}", request.Method, path);

                return CreateUnavailableResponse(request);
            }
            catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                var path = request.RequestUri?.GetLeftPart(UriPartial.Path);
                _logger.LogWarning(ex, "API isteği zaman aşımına uğradı: {Method} {Path}", request.Method, path);

                return CreateUnavailableResponse(request);
            }
        }

        private static HttpResponseMessage CreateUnavailableResponse(HttpRequestMessage request)
        {
            return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
            {
                RequestMessage = request,
                Content = new StringContent(string.Empty)
            };
        }
    }
}
