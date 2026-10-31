namespace FibiEmlakDanismanlik.AdminUI.Infrastructure;

/// <summary>
/// Güvenlik başlıklarını (CSP, X-Content-Type-Options, vb.) her HTTP yanıtına ekleyen ara katman.
/// </summary>
public sealed class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly SiteImageUrl _siteImageUrl;

    public SecurityHeadersMiddleware(RequestDelegate next, SiteImageUrl siteImageUrl)
    {
        _next = next;
        _siteImageUrl = siteImageUrl;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            var headers = context.Response.Headers;

            var imgSrc = "img-src 'self' data: blob: https:";
            if (!string.IsNullOrEmpty(_siteImageUrl.SiteOrigin) &&
                _siteImageUrl.SiteOrigin.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            {
                imgSrc += $" {_siteImageUrl.SiteOrigin}";
            }

            headers["X-Content-Type-Options"] = "nosniff";
            headers["X-Frame-Options"] = "DENY";
            headers["Referrer-Policy"] = "no-referrer";
            headers["X-Robots-Tag"] = "noindex, nofollow, noarchive";
            headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=(), payment=()";
            headers["Cross-Origin-Opener-Policy"] = "same-origin";
            headers["Content-Security-Policy"] =
                "default-src 'self'; " +
                "script-src 'self' https://challenges.cloudflare.com; " +
                "frame-src https://challenges.cloudflare.com; " +
                "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
                "font-src 'self' https://fonts.gstatic.com data:; " +
                imgSrc + "; " +
                "connect-src 'self'; " +
                "base-uri 'self'; " +
                "form-action 'self'; " +
                "frame-ancestors 'none'";

            return Task.CompletedTask;
        });

        await _next(context);
    }
}
