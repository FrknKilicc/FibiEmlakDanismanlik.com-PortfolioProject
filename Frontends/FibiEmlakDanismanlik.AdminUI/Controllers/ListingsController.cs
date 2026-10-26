using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using FibiEmlakDanismanlik.AdminUI.Infrastructure;
using FibiEmlakDanismanlik.AdminUI.Models.Deals;
using FibiEmlakDanismanlik.AdminUI.Models.Leads;
using FibiEmlakDanismanlik.AdminUI.Models.ListingAlerts;
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
        // Tür 1, 2, 3, 7, 8, 9 değilse tür seçim sayfasını göster
        if (tur is not (1 or 2 or 3 or 7 or 8 or 9))
        {
            return View("New");
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
        var photosTask = _apiClient.GetAsync<List<ListingImageDto>>($"ListingImages/{listingTypeId}/{listingId}");
        var leadsTask = _apiClient.GetAsync<List<LeadRowDto>>($"AdminLeads/listing/{listingTypeId}/{listingId}");
        var alertsTask = _apiClient.GetAsync<ListingAlertsForListingDto>($"AdminListingAlerts/listing/{listingTypeId}/{listingId}");
        Task<ApiResult<List<ListingAccessGrantDto>>>? grantsTask = null;
        Task<ApiResult<List<DealRowDto>>>? dealsTask = null;
        if (_currentAdmin.IsManagerOrAbove)
        {
            grantsTask = _apiClient.GetAsync<List<ListingAccessGrantDto>>($"AdminListings/{listingTypeId}/{listingId}/grants");
            dealsTask = _apiClient.GetAsync<List<DealRowDto>>($"AdminDeals/listing/{listingTypeId}/{listingId}");
        }

        if (grantsTask != null)
        {
            var tasks = new List<Task> { detailTask, lookupsTask, photosTask, grantsTask, leadsTask, alertsTask };
            if (dealsTask != null)
            {
                tasks.Add(dealsTask);
            }
            await Task.WhenAll(tasks);
        }
        else
        {
            var tasks = new List<Task> { detailTask, lookupsTask, photosTask, leadsTask, alertsTask };
            if (dealsTask != null)
            {
                tasks.Add(dealsTask);
            }
            await Task.WhenAll(tasks);
        }

        var detailResult = await detailTask;
        var lookupsResult = await lookupsTask;
        var photosResult = await photosTask;
        var leadsResult = await leadsTask;
        var alertsResult = await alertsTask;

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

        var grantsResult = grantsTask != null ? await grantsTask : null;
        var dealsResult = dealsTask != null ? await dealsTask : null;

        var viewModel = new ListingFormViewModel
        {
            Form = detailResult.Data.Listing,
            Lookups = lookupsResult.Data ?? new AdminListingLookupsDto(),
            CanEdit = detailResult.Data.CanEdit,
            Images = photosResult.Data ?? new List<ListingImageDto>(),
            Grants = grantsResult?.Data ?? new List<ListingAccessGrantDto>(),
            Leads = (leadsResult.Success && leadsResult.Data != null) ? leadsResult.Data : new List<LeadRowDto>(),
            Alerts = (alertsResult.Success && alertsResult.Data != null) ? alertsResult.Data : new ListingAlertsForListingDto(),
            Deals = dealsResult?.Data ?? new List<DealRowDto>(),
            CurrentStatus = detailResult.Data.Listing.PropertyStatus,
            BestDeals = detailResult.Data.Listing.BestDeals ?? detailResult.Data.BestDeals ?? false,
            Latitude = detailResult.Data.Listing.Latitude ?? detailResult.Data.Latitude,
            Longitude = detailResult.Data.Listing.Longitude ?? detailResult.Data.Longitude,
            GeoSource = detailResult.Data.Listing.GeoSource ?? detailResult.Data.GeoSource
        };

        if (viewModel.Lookups?.Amenities != null)
        {
            ListingFormViewModel.ApplyAmenitiesToToggles(viewModel.Form, viewModel.Lookups.Amenities);
        }

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

        // Cephe parçaları seçilmişse virgülle ayrılmış tek metin haline getir
        if (form.FacadeParts != null)
        {
            var parts = form.FacadeParts
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Select(p => p.Trim())
                .Distinct(StringComparer.CurrentCultureIgnoreCase);
            var joined = string.Join(", ", parts);
            form.Facade = string.IsNullOrEmpty(joined) ? null : joined;
        }

        var lookups = await LoadLookupsAsync();
        if (lookups.Amenities != null)
        {
            ListingFormViewModel.ApplyTogglesToAmenities(form, lookups.Amenities);
        }

        if (!ModelState.IsValid)
        {
            var vm = await BuildViewModelAsync(form, lookups, canEdit: true, errorMessage: "Lütfen formdaki eksik veya hatalı alanları düzeltin.");
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
            var vm = await BuildViewModelAsync(form, lookups, canEdit: false, errorMessage: "Bu ilanı düzenleme yetkiniz yok.");
            return View("Edit", vm);
        }

        if (saveResult.StatusCode == StatusCodes.Status400BadRequest)
        {
            var vm = await BuildViewModelAsync(form, lookups, canEdit: true, errorMessage: saveResult.ErrorMessage ?? "Lütfen formdaki hataları kontrol ediniz.");
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
            var vm = await BuildViewModelAsync(form, lookups, canEdit: true, errorMessage: saveResult.ErrorMessage ?? "İlan kaydedilirken bir hata oluştu.");
            return View("Edit", vm);
        }

        TempData["Success"] = "İlan kaydedildi.";
        var targetTypeId = saveResult.Data?.ListingTypeId > 0 ? saveResult.Data.ListingTypeId : form.ListingTypeId;
        var targetId = saveResult.Data?.ListingId > 0 ? saveResult.Data.ListingId : form.ListingId ?? 0;

        if (isNew && form.Latitude.HasValue && form.Longitude.HasValue)
        {
            var lat = form.Latitude.Value;
            var lng = form.Longitude.Value;
            if (lat >= -90m && lat <= 90m && lng >= -180m && lng <= 180m)
            {
                var locResult = await _apiClient.PutAsync<object>($"AdminListings/{targetTypeId}/{targetId}/location", new UpdateLocationRequest { Latitude = form.Latitude, Longitude = form.Longitude });
                if (!locResult.Success)
                {
                    TempData["Success"] += " Konum kaydedilemedi; haritadan tekrar deneyin.";
                }
            }
            else
            {
                TempData["Success"] += " Konum kaydedilemedi; haritadan tekrar deneyin.";
            }
        }

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

    /// <summary>
    /// İlanın fotoğraflarını listeler.
    /// </summary>
    [HttpGet("/ilanlar/{t:int}/{id:int}/fotograflar")]
    public async Task<IActionResult> GetPhotos(int t, int id)
    {
        var result = await _apiClient.GetAsync<List<ListingImageDto>>($"ListingImages/{t}/{id}");
        if (result.Success)
        {
            return Ok(new { ok = true, message = string.Empty, data = result.Data ?? new List<ListingImageDto>() });
        }

        return StatusCode(result.StatusCode, new { ok = false, message = result.ErrorMessage ?? "Fotoğraflar alınamadı.", data = (object?)null });
    }

    /// <summary>
    /// İlana yeni fotoğraflar yükler.
    /// </summary>
    [HttpPost("/ilanlar/{t:int}/{id:int}/fotograflar")]
    [RequestSizeLimit(320 * 1024 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = 320 * 1024 * 1024)]
    public async Task<IActionResult> UploadPhotos(int t, int id, List<IFormFile> files)
    {
        if (files == null || files.Count == 0)
        {
            return BadRequest(new { ok = false, message = "Lütfen yüklenecek fotoğraf seçin.", data = (object?)null });
        }

        if (files.Count > 20)
        {
            return BadRequest(new { ok = false, message = "Tek seferde en fazla 20 fotoğraf yükleyebilirsiniz.", data = (object?)null });
        }

        var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".webp"
        };

        foreach (var file in files)
        {
            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
            {
                return BadRequest(new { ok = false, message = $"Desteklenmeyen dosya türü: {file.FileName}. Sadece JPG, PNG veya WebP yükleyebilirsiniz.", data = (object?)null });
            }
        }

        var result = await _apiClient.PostMultipartAsync<List<ListingImageDto>>($"ListingImages/{t}/{id}", files, "files");
        if (result.Success)
        {
            return Ok(new { ok = true, message = "Fotoğraflar başarıyla yüklendi.", data = result.Data ?? new List<ListingImageDto>() });
        }

        return StatusCode(result.StatusCode, new { ok = false, message = result.ErrorMessage ?? "Fotoğraflar yüklenemedi.", data = (object?)null });
    }

    /// <summary>
    /// Belirtilen fotoğrafı siler.
    /// </summary>
    [HttpDelete("/ilanlar/fotograf/{imageId:int}")]
    public async Task<IActionResult> DeletePhoto(int imageId)
    {
        var result = await _apiClient.DeleteAsync<object>($"ListingImages/{imageId}");
        if (result.Success)
        {
            return Ok(new { ok = true, message = "Fotoğraf başarıyla silindi.", data = (object?)null });
        }

        return StatusCode(result.StatusCode, new { ok = false, message = result.ErrorMessage ?? "Fotoğraf silinemedi.", data = (object?)null });
    }

    /// <summary>
    /// Fotoğrafın türünü ve başlığını günceller.
    /// </summary>
    [HttpPut("/ilanlar/fotograf/{imageId:int}/tur")]
    public async Task<IActionResult> UpdatePhotoKind(int imageId, [FromBody] UpdatePhotoKindInputModel body)
    {
        var result = await _apiClient.PutAsync<object>($"ListingImages/{imageId}/kind", new { kind = body?.Kind ?? 0, caption = body?.Caption ?? string.Empty });
        if (result.Success)
        {
            return Ok(new { ok = true, message = "Görsel türü güncellendi.", data = (object?)null });
        }

        return StatusCode(result.StatusCode, new { ok = false, message = result.ErrorMessage ?? "Görsel türü güncellenemedi.", data = (object?)null });
    }

    /// <summary>
    /// Fotoğraf sıralamasını ve kapak fotoğrafını günceller.
    /// </summary>
    [HttpPut("/ilanlar/{t:int}/{id:int}/fotograf-sirasi")]
    public async Task<IActionResult> ReorderPhotos(int t, int id, [FromBody] ReorderPhotosInputModel body)
    {
        var request = new ReorderImagesRequest
        {
            ListingTypeId = t,
            ListingId = id,
            OrderedImageIds = body?.OrderedImageIds ?? new List<int>(),
            CoverImageId = body?.CoverImageId
        };

        var result = await _apiClient.PutAsync<object>("ListingImages/order", request);
        if (result.Success)
        {
            return Ok(new { ok = true, message = "Fotoğraf sıralaması güncellendi.", data = (object?)null });
        }

        return StatusCode(result.StatusCode, new { ok = false, message = result.ErrorMessage ?? "Fotoğraf sıralaması güncellenemedi.", data = (object?)null });
    }

    /// <summary>
    /// İlanın harita koordinatlarını günceller.
    /// </summary>
    [HttpPut("/ilanlar/{t:int}/{id:int}/konum")]
    public async Task<IActionResult> UpdateLocation(int t, int id, [FromBody] UpdateLocationInputModel body)
    {
        var request = new UpdateLocationRequest
        {
            Latitude = body?.Latitude,
            Longitude = body?.Longitude
        };

        var result = await _apiClient.PutAsync<object>($"AdminListings/{t}/{id}/location", request);
        if (result.Success)
        {
            return Ok(new { ok = true, message = "Konum bilgisi başarıyla güncellendi.", data = (object?)null });
        }

        return StatusCode(result.StatusCode, new { ok = false, message = result.ErrorMessage ?? "Konum bilgisi güncellenemedi.", data = (object?)null });
    }

    /// <summary>
    /// İlanın yakın yerlerini ve kategori listesini döner.
    /// </summary>
    [HttpGet("/ilanlar/{t:int}/{id:int}/yakin-yerler")]
    public async Task<IActionResult> GetNearbyPlaces(int t, int id)
    {
        var result = await _apiClient.GetAsync<AdminListingNearbyResponse>($"AdminListings/{t}/{id}/nearby");
        if (result.Success)
        {
            return Ok(new { ok = true, data = result.Data });
        }

        return StatusCode(result.StatusCode, new { ok = false, message = result.ErrorMessage ?? "Yakın yerler yüklenemedi." });
    }

    /// <summary>
    /// İlanın yakın yer listesini toptan günceller.
    /// </summary>
    [HttpPut("/ilanlar/{t:int}/{id:int}/yakin-yerler")]
    public async Task<IActionResult> UpdateNearbyPlaces(int t, int id, [FromBody] UpdateNearbyPlacesRequest body)
    {
        var result = await _apiClient.PutAsync<object>($"AdminListings/{t}/{id}/nearby", body ?? new UpdateNearbyPlacesRequest());
        if (result.Success)
        {
            return Ok(new { ok = true, message = "Yakın yerler kaydedildi." });
        }

        return StatusCode(result.StatusCode, new { ok = false, message = result.ErrorMessage ?? "Yakın yerler kaydedilemedi." });
    }

    /// <summary>
    /// İlan durumunu günceller.
    /// </summary>
    [HttpPost("/ilanlar/{t:int}/{id:int}/durum")]
    public async Task<IActionResult> UpdateStatus(int t, int id, [FromForm] string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            TempData["Error"] = "Lütfen geçerli bir durum seçiniz.";
            return Redirect($"/ilanlar/{t}/{id}");
        }

        var request = new UpdateStatusRequest { Status = status.Trim() };
        var result = await _apiClient.PutAsync<object>($"AdminListings/{t}/{id}/status", request);

        if (result.Success)
        {
            TempData["Success"] = "İlan durumu başarıyla güncellendi.";
        }
        else
        {
            TempData["Error"] = result.ErrorMessage ?? "İlan durumu güncellenemedi.";
        }

        return Redirect($"/ilanlar/{t}/{id}");
    }

    /// <summary>
    /// İlanın vitrin (best-deal) durumunu günceller (Sahip veya Yönetici).
    /// </summary>
    [HttpPost("/ilanlar/{t:int}/{id:int}/vitrin")]
    public async Task<IActionResult> UpdateBestDeal(int t, int id, [FromForm] bool isBestDeal)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var request = new UpdateBestDealRequest { IsBestDeal = isBestDeal };
        var result = await _apiClient.PutAsync<object>($"AdminListings/{t}/{id}/best-deal", request);

        if (result.Success)
        {
            TempData["Success"] = isBestDeal ? "İlan başarıyla vitrine eklendi." : "İlan vitrinden çıkarıldı.";
        }
        else
        {
            TempData["Error"] = result.ErrorMessage ?? "Vitrin durumu güncellenemedi.";
        }

        return Redirect($"/ilanlar/{t}/{id}");
    }

    /// <summary>
    /// İlan için danışman erişim izni tanımlar (Sahip veya Yönetici).
    /// </summary>
    [HttpPost("/ilanlar/{t:int}/{id:int}/izinler")]
    public async Task<IActionResult> AddGrant(int t, int id, [FromForm] int agentId, [FromForm] string? access)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        if (agentId <= 0 || string.IsNullOrWhiteSpace(access))
        {
            TempData["Error"] = "Lütfen danışman ve erişim düzeyini seçiniz.";
            return Redirect($"/ilanlar/{t}/{id}");
        }

        var request = new CreateListingAccessGrantRequest
        {
            AgentId = agentId,
            Access = access.Trim()
        };

        var result = await _apiClient.PostAsync<object>($"AdminListings/{t}/{id}/grants", request);

        if (result.Success)
        {
            TempData["Success"] = "Erişim izni başarıyla eklendi.";
        }
        else
        {
            TempData["Error"] = result.ErrorMessage ?? "Erişim izni eklenemedi.";
        }

        return Redirect($"/ilanlar/{t}/{id}");
    }

    /// <summary>
    /// Tanımlı danışman erişim iznini kaldırır (Sahip veya Yönetici).
    /// </summary>
    [HttpPost("/ilanlar/izinler/{grantId:int}/kaldir")]
    public async Task<IActionResult> RemoveGrant(int grantId, [FromForm] int t, [FromForm] int id)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var result = await _apiClient.DeleteAsync<object>($"AdminListings/grants/{grantId}");

        if (result.Success)
        {
            TempData["Success"] = "Erişim izni başarıyla kaldırıldı.";
        }
        else
        {
            TempData["Error"] = result.ErrorMessage ?? "Erişim izni kaldırılamadı.";
        }

        return Redirect($"/ilanlar/{t}/{id}");
    }

    /// <summary>
    /// İlanı ve bağlı tüm verilerini kalıcı olarak siler (Yalnızca Sahip).
    /// </summary>
    [HttpPost("/ilanlar/{t:int}/{id:int}/sil")]
    public async Task<IActionResult> DeleteListing(int t, int id, [FromForm] string? confirmText)
    {
        if (!_currentAdmin.IsOwner)
        {
            return Redirect("/yetkisiz");
        }

        if (!string.Equals(confirmText?.Trim(), "SİL", StringComparison.Ordinal))
        {
            TempData["Error"] = "İlanı kalıcı olarak silmek için onay kutusuna 'SİL' yazmalısınız.";
            return Redirect($"/ilanlar/{t}/{id}");
        }

        var result = await _apiClient.DeleteAsync<object>($"AdminListings/{t}/{id}");

        if (result.Success)
        {
            TempData["Success"] = "İlan ve bağlı fotoğraflar kalıcı olarak silindi.";
            return Redirect("/ilanlar");
        }

        TempData["Error"] = result.ErrorMessage ?? "İlan silinirken bir hata oluştu.";
        return Redirect($"/ilanlar/{t}/{id}");
    }

    private async Task<AdminListingLookupsDto> LoadLookupsAsync()
    {
        var lookupsResult = await _apiClient.GetAsync<AdminListingLookupsDto>("AdminListings/lookups");
        return lookupsResult.Data ?? new AdminListingLookupsDto();
    }

    private async Task<ListingFormViewModel> BuildViewModelAsync(ListingFormModel form, AdminListingLookupsDto lookups, bool canEdit, string? errorMessage)
    {
        var vm = new ListingFormViewModel
        {
            Form = form,
            Lookups = lookups,
            CanEdit = canEdit,
            ErrorMessage = errorMessage,
            CurrentStatus = form.PropertyStatus,
            BestDeals = form.BestDeals ?? false,
            Latitude = form.Latitude,
            Longitude = form.Longitude,
            GeoSource = form.GeoSource
        };

        if (form.ListingId.HasValue && form.ListingId.Value > 0)
        {
            var photosResult = await _apiClient.GetAsync<List<ListingImageDto>>($"ListingImages/{form.ListingTypeId}/{form.ListingId.Value}");
            vm.Images = photosResult.Data ?? new List<ListingImageDto>();

            if (_currentAdmin.IsManagerOrAbove)
            {
                var grantsResult = await _apiClient.GetAsync<List<ListingAccessGrantDto>>($"AdminListings/{form.ListingTypeId}/{form.ListingId.Value}/grants");
                vm.Grants = grantsResult.Data ?? new List<ListingAccessGrantDto>();
            }
        }

        return vm;
    }
}
