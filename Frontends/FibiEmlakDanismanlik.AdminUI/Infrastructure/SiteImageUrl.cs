using Microsoft.Extensions.Configuration;

namespace FibiEmlakDanismanlik.AdminUI.Infrastructure;

/// <summary>
/// Site içi veya harici görsel adreslerini yönetim panelinde önizleme amaçlı tam URL'ye dönüştüren yardımcı servis.
/// </summary>
public sealed class SiteImageUrl
{
    private readonly string? _siteUrl;

    /// <summary>
    /// SiteUrl tanımlıysa 'scheme://host[:port]' origin bilgisi, değilse null.
    /// </summary>
    public string? SiteOrigin { get; }

    public SiteImageUrl(IConfiguration configuration)
    {
        var rawUrl = configuration["Url:SiteUrl"]?.Trim();
        if (!string.IsNullOrEmpty(rawUrl))
        {
            _siteUrl = rawUrl.TrimEnd('/');
            if (Uri.TryCreate(_siteUrl, UriKind.Absolute, out var uri))
            {
                SiteOrigin = uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped);
            }
        }
    }

    /// <summary>
    /// Verilen görsel yolunu önizlenebilir tam veya göreli adrese çevirir.
    /// Boşsa, '//' ile başlıyorsa, '\' içeriyorsa veya http/https harici bir şema (':' içeren) taşıyorsa null döner.
    /// </summary>
    public string? Preview(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return null;
        }

        var trimmed = url.Trim();

        if (trimmed.StartsWith("//", StringComparison.Ordinal))
        {
            return null;
        }

        if (trimmed.Contains('\\'))
        {
            return null;
        }

        if (trimmed.Contains(':') &&
            !trimmed.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !trimmed.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        if (trimmed.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            trimmed.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return trimmed;
        }

        if (trimmed.StartsWith("/", StringComparison.Ordinal))
        {
            return !string.IsNullOrEmpty(_siteUrl) ? $"{_siteUrl}{trimmed}" : trimmed;
        }

        return !string.IsNullOrEmpty(_siteUrl) ? $"{_siteUrl}/{trimmed}" : trimmed;
    }
}
