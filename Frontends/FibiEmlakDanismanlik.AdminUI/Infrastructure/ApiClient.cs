using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace FibiEmlakDanismanlik.AdminUI.Infrastructure;

/// <summary>
/// WebApi uç noktalarına güvenli HTTP çağrıları yapan istemci servisi.
/// </summary>
public sealed class ApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly CurrentAdmin _currentAdmin;

    public ApiClient(
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CurrentAdmin currentAdmin)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
        _currentAdmin = currentAdmin;
    }

    /// <summary>
    /// Kimlik doğrulamalı GET isteği gönderir.
    /// </summary>
    public Task<ApiResult<T>> GetAsync<T>(string path) =>
        SendAsync<T>(HttpMethod.Get, path, body: null, isAnonymous: false);

    /// <summary>
    /// Kimlik doğrulamalı POST isteği gönderir.
    /// </summary>
    public Task<ApiResult<T>> PostAsync<T>(string path, object? body) =>
        SendAsync<T>(HttpMethod.Post, path, body, isAnonymous: false);

    /// <summary>
    /// Kimlik doğrulamalı PUT isteği gönderir.
    /// </summary>
    public Task<ApiResult<T>> PutAsync<T>(string path, object? body) =>
        SendAsync<T>(HttpMethod.Put, path, body, isAnonymous: false);

    /// <summary>
    /// Kimlik doğrulamalı DELETE isteği gönderir.
    /// </summary>
    public Task<ApiResult<T>> DeleteAsync<T>(string path) =>
        SendAsync<T>(HttpMethod.Delete, path, body: null, isAnonymous: false);

    /// <summary>
    /// Kimlik doğrulama gerektirmeyen (anonim) GET isteği gönderir.
    /// </summary>
    public Task<ApiResult<T>> GetAnonymousAsync<T>(string path) =>
        SendAsync<T>(HttpMethod.Get, path, body: null, isAnonymous: true);

    /// <summary>
    /// Kimlik doğrulama gerektirmeyen (anonim) POST isteği gönderir.
    /// </summary>
    public Task<ApiResult<T>> PostAnonymousAsync<T>(string path, object body) =>
        SendAsync<T>(HttpMethod.Post, path, body, isAnonymous: true);

    /// <summary>
    /// Çok parçalı (multipart/form-data) dosya yükleme POST isteği gönderir.
    /// </summary>
    public async Task<ApiResult<T>> PostMultipartAsync<T>(string path, IEnumerable<IFormFile> files, string fieldName = "files")
    {
        var client = _httpClientFactory.CreateClient("api");
        using var request = new HttpRequestMessage(HttpMethod.Post, path);

        // Ziyaretçi bilgilerini ve yetkilendirme başlıklarını ekle
        ApplyHeaders(request, isAnonymous: false);

        var multipartContent = new MultipartFormDataContent();
        var openedStreams = new List<Stream>();

        try
        {
            foreach (var file in files)
            {
                if (file == null || file.Length == 0)
                {
                    continue;
                }

                var stream = file.OpenReadStream();
                openedStreams.Add(stream);
                var streamContent = new StreamContent(stream);

                if (!string.IsNullOrWhiteSpace(file.ContentType))
                {
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                }

                multipartContent.Add(streamContent, fieldName, file.FileName);
            }

            request.Content = multipartContent;

            HttpResponseMessage response;
            try
            {
                response = await client.SendAsync(request);
            }
            catch (Exception ex)
            {
                return ApiResult<T>.Fail($"API sunucusuna ulaşılamadı: {ex.Message}", StatusCodes.Status503ServiceUnavailable);
            }

            using (response)
            {
                var statusCode = (int)response.StatusCode;
                var rawContent = await response.Content.ReadAsStringAsync();

                // Yetkili çağrılarda 401 gelirse oturumu kapat
                if (statusCode == StatusCodes.Status401Unauthorized)
                {
                    var httpContext = _httpContextAccessor.HttpContext;
                    if (httpContext != null)
                    {
                        await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    }

                    var (errorMsg, _) = ExtractErrorDetails(rawContent);
                    if (string.IsNullOrWhiteSpace(errorMsg))
                    {
                        errorMsg = "Oturum süreniz sona erdi. Lütfen tekrar giriş yapın.";
                    }

                    return ApiResult<T>.Fail(errorMsg, statusCode);
                }

                if (!response.IsSuccessStatusCode)
                {
                    var (errorMsg, fieldErrors) = ExtractErrorDetails(rawContent);
                    if (string.IsNullOrWhiteSpace(errorMsg))
                    {
                        errorMsg = $"İşlem başarısız oldu (HTTP {statusCode}).";
                    }

                    return ApiResult<T>.Fail(errorMsg, statusCode, errors: fieldErrors);
                }

                if (string.IsNullOrWhiteSpace(rawContent))
                {
                    return ApiResult<T>.Ok(default, statusCode);
                }

                try
                {
                    var data = JsonSerializer.Deserialize<T>(rawContent, JsonOptions);
                    return ApiResult<T>.Ok(data, statusCode);
                }
                catch (JsonException ex)
                {
                    return ApiResult<T>.Fail($"API yanıtı işlenirken hata oluştu: {ex.Message}", statusCode);
                }
            }
        }
        finally
        {
            foreach (var stream in openedStreams)
            {
                await stream.DisposeAsync();
            }
        }
    }

    private async Task<ApiResult<T>> SendAsync<T>(HttpMethod method, string path, object? body, bool isAnonymous)
    {
        var client = _httpClientFactory.CreateClient("api");
        using var request = new HttpRequestMessage(method, path);

        // Ziyaretçi bilgilerini ve yetkilendirme başlıklarını ekle
        ApplyHeaders(request, isAnonymous);

        // Gövde varsa JSON olarak serileştir
        if (body != null)
        {
            var json = JsonSerializer.Serialize(body, JsonOptions);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        HttpResponseMessage response;
        try
        {
            response = await client.SendAsync(request);
        }
        catch (Exception ex)
        {
            return ApiResult<T>.Fail($"API sunucusuna ulaşılamadı: {ex.Message}", StatusCodes.Status503ServiceUnavailable);
        }

        using (response)
        {
            var statusCode = (int)response.StatusCode;
            var rawContent = await response.Content.ReadAsStringAsync();

            // Yetkili çağrılarda 401 gelirse oturumu kapat
            if (statusCode == StatusCodes.Status401Unauthorized && !isAnonymous)
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                }

                var (errorMsg, _) = ExtractErrorDetails(rawContent);
                if (string.IsNullOrWhiteSpace(errorMsg))
                {
                    errorMsg = "Oturum süreniz sona erdi. Lütfen tekrar giriş yapın.";
                }

                return ApiResult<T>.Fail(errorMsg, statusCode);
            }

            if (!response.IsSuccessStatusCode)
            {
                var (errorMsg, fieldErrors) = ExtractErrorDetails(rawContent);
                if (string.IsNullOrWhiteSpace(errorMsg))
                {
                    errorMsg = $"İşlem başarısız oldu (HTTP {statusCode}).";
                }

                return ApiResult<T>.Fail(errorMsg, statusCode, errors: fieldErrors);
            }

            if (string.IsNullOrWhiteSpace(rawContent))
            {
                return ApiResult<T>.Ok(default, statusCode);
            }

            if (typeof(T) == typeof(string) || typeof(T) == typeof(object))
            {
                var mediaType = response.Content.Headers.ContentType?.MediaType;
                if (mediaType == null || !mediaType.Contains("json", StringComparison.OrdinalIgnoreCase))
                {
                    return ApiResult<T>.Ok((T)(object)rawContent, statusCode);
                }
            }

            try
            {
                var data = JsonSerializer.Deserialize<T>(rawContent, JsonOptions);
                return ApiResult<T>.Ok(data, statusCode);
            }
            catch (JsonException ex)
            {
                return ApiResult<T>.Fail($"API yanıtı işlenirken hata oluştu: {ex.Message}", statusCode);
            }
        }
    }

    private void ApplyHeaders(HttpRequestMessage request, bool isAnonymous)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            // Ziyaretçinin IP adresi
            var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString();
            if (!string.IsNullOrWhiteSpace(remoteIp))
            {
                request.Headers.TryAddWithoutValidation("X-Forwarded-For", remoteIp);
            }

            // Ziyaretçinin User-Agent bilgisi
            var userAgent = httpContext.Request.Headers.UserAgent.ToString();
            if (!string.IsNullOrWhiteSpace(userAgent))
            {
                request.Headers.TryAddWithoutValidation("User-Agent", userAgent);
            }
        }

        // Yetkili çağrılarda Bearer token ekle
        if (!isAnonymous)
        {
            var token = _currentAdmin.AccessToken;
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }

    private static string ExtractErrorMessage(string rawResponse) =>
        ExtractErrorDetails(rawResponse).ErrorMessage;

    private static (string ErrorMessage, Dictionary<string, string[]>? Errors) ExtractErrorDetails(string rawResponse)
    {
        if (string.IsNullOrWhiteSpace(rawResponse))
        {
            return (string.Empty, null);
        }

        try
        {
            using var doc = JsonDocument.Parse(rawResponse);
            var root = doc.RootElement;
            Dictionary<string, string[]>? fieldErrors = null;
            var errorMessages = new List<string>();

            // ValidationProblemDetails (errors sözlüğü) kontrolü
            if (root.TryGetProperty("errors", out var errorsProp) && errorsProp.ValueKind == JsonValueKind.Object)
            {
                fieldErrors = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
                foreach (var property in errorsProp.EnumerateObject())
                {
                    var messages = new List<string>();
                    if (property.Value.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in property.Value.EnumerateArray())
                        {
                            var str = item.GetString();
                            if (!string.IsNullOrWhiteSpace(str))
                            {
                                messages.Add(str);
                                errorMessages.Add(str);
                            }
                        }
                    }
                    else if (property.Value.ValueKind == JsonValueKind.String)
                    {
                        var str = property.Value.GetString();
                        if (!string.IsNullOrWhiteSpace(str))
                        {
                            messages.Add(str);
                            errorMessages.Add(str);
                        }
                    }

                    if (messages.Count > 0)
                    {
                        fieldErrors[property.Name] = messages.ToArray();
                    }
                }
            }

            // ProblemDetails formatındaki detail alanı
            if (root.TryGetProperty("detail", out var detailProp) && detailProp.ValueKind == JsonValueKind.String)
            {
                var detail = detailProp.GetString();
                if (!string.IsNullOrWhiteSpace(detail))
                {
                    return (detail, fieldErrors);
                }
            }

            if (errorMessages.Count > 0)
            {
                return (string.Join(" ", errorMessages), fieldErrors);
            }

            // ProblemDetails formatındaki title alanı
            if (root.TryGetProperty("title", out var titleProp) && titleProp.ValueKind == JsonValueKind.String)
            {
                var title = titleProp.GetString();
                if (!string.IsNullOrWhiteSpace(title))
                {
                    return (title, fieldErrors);
                }
            }

            // Genel message alanı
            if (root.TryGetProperty("message", out var messageProp) && messageProp.ValueKind == JsonValueKind.String)
            {
                var message = messageProp.GetString();
                if (!string.IsNullOrWhiteSpace(message))
                {
                    return (message, fieldErrors);
                }
            }

            return (string.Empty, fieldErrors);
        }
        catch (JsonException)
        {
            // JSON ayrıştırılamadıysa düz metin olarak kabul et
            return (rawResponse.Trim(), null);
        }
    }
}
