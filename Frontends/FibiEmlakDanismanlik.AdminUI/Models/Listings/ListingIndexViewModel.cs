using System.Globalization;
using Microsoft.AspNetCore.WebUtilities;

namespace FibiEmlakDanismanlik.AdminUI.Models.Listings;

/// <summary>
/// İlan listesi sayfası görünüm modeli.
/// </summary>
public sealed class ListingIndexViewModel
{
    private static readonly CultureInfo TurkishCulture = new("tr-TR");

    // Filtre Alanları
    public string? Tur { get; set; }
    public string? Kategori { get; set; }
    public string? Durum { get; set; }
    public int? DanismanId { get; set; }
    public string? Ara { get; set; }
    public string? Sirala { get; set; }
    public int Sayfa { get; set; } = 1;

    // API Verileri
    public AdminListingListResponse Response { get; set; } = new();
    public AdminListingLookupsDto Lookups { get; set; } = new();
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Toplam sayfa sayısı.
    /// </summary>
    public int TotalPages => Response.PageSize > 0
        ? (int)Math.Ceiling((double)Response.Total / Response.PageSize)
        : 0;

    /// <summary>
    /// Fiyatı tr-TR kültüründe biçimlendirir. Kiralık ilanlarda sonuna " / ay" ekler.
    /// </summary>
    public string PriceText(AdminListingRowDto row)
    {
        var formatted = row.Price.ToString("N0", TurkishCulture) + " ₺";
        return row.IsRent ? formatted + " / ay" : formatted;
    }

    /// <summary>
    /// Durum değerine göre Bootstrap rozet (badge) sınıfını döndürür.
    /// </summary>
    public string StatusBadgeClass(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return "secondary";
        }

        var normalized = status.Trim();

        if (string.Equals(normalized, "Aktif", StringComparison.OrdinalIgnoreCase))
        {
            return "success";
        }

        if (string.Equals(normalized, "Opsiyonlu", StringComparison.OrdinalIgnoreCase))
        {
            return "warning";
        }

        if (string.Equals(normalized, "Pasif", StringComparison.OrdinalIgnoreCase))
        {
            return "secondary";
        }

        if (string.Equals(normalized, "Satıldı", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(normalized, "Satildi", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(normalized, "Kiralandı", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(normalized, "Kiralandi", StringComparison.OrdinalIgnoreCase))
        {
            return "dark";
        }

        return "secondary";
    }

    /// <summary>
    /// İlan düzenleme veya detay URL'ini oluşturur.
    /// </summary>
    public string EditUrl(AdminListingRowDto row)
    {
        return $"/ilanlar/{row.ListingTypeId}/{row.ListingId}";
    }

    /// <summary>
    /// Mevcut filtreleri koruyarak verilen sayfa numarası için URL üretir.
    /// </summary>
    public string PageUrl(int page)
    {
        var query = new Dictionary<string, string?>();

        if (!string.IsNullOrWhiteSpace(Tur))
        {
            query["tur"] = Tur;
        }

        if (!string.IsNullOrWhiteSpace(Kategori))
        {
            query["kategori"] = Kategori;
        }

        if (!string.IsNullOrWhiteSpace(Durum))
        {
            query["durum"] = Durum;
        }

        if (DanismanId.HasValue && DanismanId.Value > 0)
        {
            query["danisman"] = DanismanId.Value.ToString();
        }

        if (!string.IsNullOrWhiteSpace(Ara))
        {
            query["ara"] = Ara;
        }

        if (!string.IsNullOrWhiteSpace(Sirala))
        {
            query["sirala"] = Sirala;
        }

        if (page > 1)
        {
            query["sayfa"] = page.ToString();
        }

        return QueryHelpers.AddQueryString("/ilanlar", query);
    }
}
