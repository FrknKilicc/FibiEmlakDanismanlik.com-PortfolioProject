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
    private readonly CurrentAdmin _currentAdmin;

    public ListingsController(ApiClient apiClient, CurrentAdmin currentAdmin)
    {
        _apiClient = apiClient;
        _currentAdmin = currentAdmin;
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

    /// <summary>
    /// Yeni ilan ekleme formu sayfası.
    /// </summary>
    [HttpGet("/ilanlar/yeni")]
    public async Task<IActionResult> Create(int tur)
    {
        // Tür 1, 2, 3, 7, 8, 9 değilse /ilanlar sayfasına yönlendir
        if (tur is not (1 or 2 or 3 or 7 or 8 or 9))
        {
            return Redirect("/ilanlar");
        }

        var lookupsResult = await _apiClient.GetAsync<AdminListingLookupsDto>("AdminListings/lookups");
        if (lookupsResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }

        var form = new ListingFormModel
        {
            ListingTypeId = tur
        };

        // Eğer kullanıcı bir danışman ise kendi AgentId değerini varsayılan yap
        if (_currentAdmin.AgentId.HasValue && !_currentAdmin.IsManagerOrAbove)
        {
            form.AgentId = _currentAdmin.AgentId.Value;
        }

        var viewModel = new ListingFormViewModel
        {
            Form = form,
            Lookups = lookupsResult.Data ?? new AdminListingLookupsDto(),
            CanEdit = true
        };

        return View("Edit", viewModel);
    }

    /// <summary>
    /// İlan düzenleme sayfası.
    /// </summary>
    [HttpGet("/ilanlar/{listingTypeId:int}/{listingId:int}")]
    public async Task<IActionResult> Edit(int listingTypeId, int listingId)
    {
        var detailTask = _apiClient.GetAsync<AdminListingDetailResponse>($"AdminListings/{listingTypeId}/{listingId}");
        var lookupsTask = _apiClient.GetAsync<AdminListingLookupsDto>("AdminListings/lookups");

        await Task.WhenAll(detailTask, lookupsTask);

        var detailResult = await detailTask;
        var lookupsResult = await lookupsTask;

        if (detailResult.StatusCode == StatusCodes.Status401Unauthorized ||
            lookupsResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }

        if (detailResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        if (detailResult.StatusCode == StatusCodes.Status404NotFound || detailResult.Data?.Listing == null)
        {
            return NotFound();
        }

        var viewModel = new ListingFormViewModel
        {
            Form = detailResult.Data.Listing,
            Lookups = lookupsResult.Data ?? new AdminListingLookupsDto(),
            CanEdit = detailResult.Data.CanEdit
        };

        return View("Edit", viewModel);
    }

    /// <summary>
    /// İlan kaydetme (POST/PUT) işlemi.
    /// </summary>
    [HttpPost("/ilanlar/kaydet")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(ListingFormModel form)
    {
        var isNew = !form.ListingId.HasValue || form.ListingId.Value <= 0;

        if (!ModelState.IsValid)
        {
            var lookups = await LoadLookupsAsync();
            var vm = BuildViewModel(form, lookups, canEdit: true, errorMessage: "Lütfen formdaki eksik veya hatalı alanları düzeltin.");
            foreach (var state in ModelState)
            {
                if (state.Value.Errors.Count > 0)
                {
                    vm.FieldErrors[state.Key] = state.Value.Errors.Select(e => e.ErrorMessage).ToArray();
                }
            }
            return View("Edit", vm);
        }

        ApiResult<SaveListingResponse> saveResult;
        if (isNew)
        {
            saveResult = await _apiClient.PostAsync<SaveListingResponse>("AdminListings", form);
        }
        else
        {
            saveResult = await _apiClient.PutAsync<SaveListingResponse>("AdminListings", form);
        }

        if (saveResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }

        if (saveResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            var lookups = await LoadLookupsAsync();
            var vm = BuildViewModel(form, lookups, canEdit: false, errorMessage: "Bu ilanı düzenleme yetkiniz yok.");
            return View("Edit", vm);
        }

        if (saveResult.StatusCode == StatusCodes.Status400BadRequest)
        {
            var lookups = await LoadLookupsAsync();
            var vm = BuildViewModel(form, lookups, canEdit: true, errorMessage: saveResult.ErrorMessage ?? "Lütfen formdaki hataları kontrol ediniz.");
            if (saveResult.Errors != null)
            {
                foreach (var (key, msgs) in saveResult.Errors)
                {
                    vm.FieldErrors[key] = msgs;
                    ModelState.AddModelError(key, string.Join(" ", msgs));
                }
            }
            return View("Edit", vm);
        }

        if (!saveResult.Success)
        {
            var lookups = await LoadLookupsAsync();
            var vm = BuildViewModel(form, lookups, canEdit: true, errorMessage: saveResult.ErrorMessage ?? "İlan kaydedilirken bir hata oluştu.");
            return View("Edit", vm);
        }

        TempData["Success"] = "İlan kaydedildi.";
        var targetTypeId = saveResult.Data?.ListingTypeId > 0 ? saveResult.Data.ListingTypeId : form.ListingTypeId;
        var targetId = saveResult.Data?.ListingId > 0 ? saveResult.Data.ListingId : form.ListingId ?? 0;

        return Redirect($"/ilanlar/{targetTypeId}/{targetId}");
    }

    /// <summary>
    /// Konum illeri listesi proxy ucu.
    /// </summary>
    [HttpGet("/ilanlar/konum/iller")]
    public async Task<IActionResult> GetCities()
    {
        var result = await _apiClient.GetAnonymousAsync<List<LocationOptionDto>>("Location/cities");
        return Json(result.Data ?? new List<LocationOptionDto>());
    }

    /// <summary>
    /// Konum ilçeleri listesi proxy ucu.
    /// </summary>
    [HttpGet("/ilanlar/konum/ilceler")]
    public async Task<IActionResult> GetDistricts(int cityId)
    {
        var result = await _apiClient.GetAnonymousAsync<List<LocationOptionDto>>($"Location/districts?cityId={cityId}");
        return Json(result.Data ?? new List<LocationOptionDto>());
    }

    /// <summary>
    /// Konum mahalleleri listesi proxy ucu.
    /// </summary>
    [HttpGet("/ilanlar/konum/mahalleler")]
    public async Task<IActionResult> GetNeighborhoods(int districtId)
    {
        var result = await _apiClient.GetAnonymousAsync<List<LocationOptionDto>>($"Location/neighborhoods?districtId={districtId}");
        return Json(result.Data ?? new List<LocationOptionDto>());
    }

    private async Task<AdminListingLookupsDto> LoadLookupsAsync()
    {
        var lookupsResult = await _apiClient.GetAsync<AdminListingLookupsDto>("AdminListings/lookups");
        return lookupsResult.Data ?? new AdminListingLookupsDto();
    }

    private static ListingFormViewModel BuildViewModel(ListingFormModel form, AdminListingLookupsDto lookups, bool canEdit, string? errorMessage)
    {
        return new ListingFormViewModel
        {
            Form = form,
            Lookups = lookups,
            CanEdit = canEdit,
            ErrorMessage = errorMessage
        };
    }
}
