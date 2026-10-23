using Microsoft.AspNetCore.WebUtilities;
using FibiEmlakDanismanlik.AdminUI.Models.Listings;

namespace FibiEmlakDanismanlik.AdminUI.Models.ListingAlerts;

/// <summary>
/// Haber bekleyenler listesi sayfası görünüm modeli.
/// </summary>
public sealed class ListingAlertIndexViewModel
{
    // Filtre Alanları
    public int? Durum { get; set; } = 1;
    public int? DanismanId { get; set; }
    public bool? Atanmamis { get; set; }
    public bool Bekleyen { get; set; }
    public string? Ara { get; set; }
    public int Sayfa { get; set; } = 1;

    // API Yanıt ve Lookup Verileri
    public ListingAlertListResponse Response { get; set; } = new();
    public List<AgentLookupDto> Agents { get; set; } = new();
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Toplam sayfa sayısı.
    /// </summary>
    public int TotalPages => Response.PageSize > 0
        ? (int)Math.Ceiling((double)Response.Total / Response.PageSize)
        : 0;

    /// <summary>
    /// Mevcut filtreleri koruyarak verilen sayfa numarası için URL üretir.
    /// </summary>
    public string PageUrl(int page)
    {
        var query = new Dictionary<string, string?>();

        if (Durum.HasValue)
        {
            query["durum"] = Durum.Value.ToString();
        }

        if (DanismanId.HasValue && DanismanId.Value > 0)
        {
            query["danisman"] = DanismanId.Value.ToString();
        }

        if (Atanmamis.HasValue && Atanmamis.Value)
        {
            query["atanmamis"] = "true";
        }

        if (Bekleyen)
        {
            query["bekleyen"] = "true";
        }

        if (!string.IsNullOrWhiteSpace(Ara))
        {
            query["ara"] = Ara;
        }

        if (page > 1)
        {
            query["sayfa"] = page.ToString();
        }

        return QueryHelpers.AddQueryString("/haber-bekleyenler", query);
    }

    /// <summary>
    /// Diğer filtreleri koruyarak belirli bir durum için filtreleme URL'i üretir.
    /// </summary>
    public string StatusFilterUrl(int? status)
    {
        var query = new Dictionary<string, string?>();

        if (status.HasValue)
        {
            query["durum"] = status.Value.ToString();
        }

        if (DanismanId.HasValue && DanismanId.Value > 0)
        {
            query["danisman"] = DanismanId.Value.ToString();
        }

        if (Atanmamis.HasValue && Atanmamis.Value)
        {
            query["atanmamis"] = "true";
        }

        if (Bekleyen)
        {
            query["bekleyen"] = "true";
        }

        if (!string.IsNullOrWhiteSpace(Ara))
        {
            query["ara"] = Ara;
        }

        return QueryHelpers.AddQueryString("/haber-bekleyenler", query);
    }
}
