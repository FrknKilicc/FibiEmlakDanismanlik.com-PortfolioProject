using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using FibiEmlakDanismanlik.AdminUI.Infrastructure;
using FibiEmlakDanismanlik.AdminUI.Models.Listings;

namespace FibiEmlakDanismanlik.AdminUI.Controllers;

/// <summary>
/// İlan listesi ve filtreleme işlemlerini yöneten denetleyici.
/// </summary>
[Authorize]
public sealed class ListingsController : Controller
{
    private const int PageSize = 20;
    private readonly ApiClient _apiClient;

    public ListingsController(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    /// <summary>
    /// Yönetim paneli ilan listesi sayfası.
    /// </summary>
    [HttpGet("/ilanlar")]
    public async Task<IActionResult> Index(
        string? tur,
        string? kategori,
        string? durum,
        int? danisman,
        string? ara,
        string? sirala,
        int sayfa = 1)
    {
        var pageNumber = Math.Max(1, sayfa);

        // API sorgu parametrelerini oluştur
        var queryParams = new Dictionary<string, string?>
        {
            ["page"] = pageNumber.ToString(),
            ["pageSize"] = PageSize.ToString()
        };

        // Tur filtresi: satilik -> false, kiralik -> true
        if (string.Equals(tur, "satilik", StringComparison.OrdinalIgnoreCase))
        {
            queryParams["isRent"] = "false";
        }
        else if (string.Equals(tur, "kiralik", StringComparison.OrdinalIgnoreCase))
        {
            queryParams["isRent"] = "true";
        }

        // Kategori filtresi: konut -> housing, arsa -> land, isyeri -> commercial
        if (!string.IsNullOrWhiteSpace(kategori))
        {
            var normalizedCat = kategori.Trim().ToLowerInvariant() switch
            {
                "konut" or "housing" => "housing",
                "arsa" or "land" => "land",
                "isyeri" or "commercial" => "commercial",
                _ => null
            };

            if (normalizedCat != null)
            {
                queryParams["category"] = normalizedCat;
            }
        }

        if (!string.IsNullOrWhiteSpace(durum))
        {
            queryParams["status"] = durum.Trim();
        }

        if (danisman.HasValue && danisman.Value > 0)
        {
            queryParams["agentId"] = danisman.Value.ToString();
        }

        if (!string.IsNullOrWhiteSpace(ara))
        {
            queryParams["search"] = ara.Trim();
        }

        if (!string.IsNullOrWhiteSpace(sirala))
        {
            queryParams["sort"] = sirala.Trim();
        }

        var apiPath = QueryHelpers.AddQueryString("AdminListings", queryParams);

        // İlanları ve Lookup verilerini paralel olarak çağır
        var listingsTask = _apiClient.GetAsync<AdminListingListResponse>(apiPath);
        var lookupsTask = _apiClient.GetAsync<AdminListingLookupsDto>("AdminListings/lookups");

        await Task.WhenAll(listingsTask, lookupsTask);

        var listingsResult = await listingsTask;
        var lookupsResult = await lookupsTask;

        // Yetkisiz erişim (401) durumunda giriş sayfasına yönlendir
        if (listingsResult.StatusCode == StatusCodes.Status401Unauthorized ||
            lookupsResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }

        var viewModel = new ListingIndexViewModel
        {
            Tur = tur,
            Kategori = kategori,
            Durum = durum,
            DanismanId = danisman,
            Ara = ara,
            Sirala = sirala,
            Sayfa = pageNumber
        };

        if (listingsResult.Success && listingsResult.Data != null)
        {
            viewModel.Response = listingsResult.Data;
        }
        else
        {
            viewModel.ErrorMessage = listingsResult.ErrorMessage ?? "İlanlar listelenirken bir hata oluştu.";
        }

        if (lookupsResult.Success && lookupsResult.Data != null)
        {
            viewModel.Lookups = lookupsResult.Data;
        }
        else if (string.IsNullOrWhiteSpace(viewModel.ErrorMessage))
        {
            viewModel.ErrorMessage = lookupsResult.ErrorMessage ?? "Filtre verileri yüklenirken bir hata oluştu.";
        }

        return View(viewModel);
    }
}
