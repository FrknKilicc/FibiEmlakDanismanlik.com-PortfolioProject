#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FibiEmlakDanismanlik.AdminUI.Infrastructure;
using FibiEmlakDanismanlik.AdminUI.Models.Content;

namespace FibiEmlakDanismanlik.AdminUI.Controllers;

/// <summary>
/// Site içerik yönetimi (Müşteri Yorumları, İş Ortakları, Ana Sayfa Görselleri, Şehir Galerisi, Kredi Oranları) denetleyicisi.
/// </summary>
[Authorize]
public sealed class SiteContentController : Controller
{
    private readonly ApiClient _apiClient;
    private readonly CurrentAdmin _currentAdmin;

    public SiteContentController(ApiClient apiClient, CurrentAdmin currentAdmin)
    {
        _apiClient = apiClient;
        _currentAdmin = currentAdmin;
    }

    #region 1. Müşteri Yorumları

    /// <summary>
    /// Müşteri yorumları listesi sayfası.
    /// </summary>
    [HttpGet("/icerik/yorumlar")]
    public async Task<IActionResult> Testimonials()
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var result = await _apiClient.GetAsync<List<TestimonialDto>>("Testimonails/admin");

        if (result.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }
        if (result.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        var items = (result.Data ?? new List<TestimonialDto>())
            .OrderBy(x => x.DisplayOrder)
            .ThenByDescending(x => x.Id)
            .ToList();

        var vm = new SiteContentListViewModel<TestimonialDto>
        {
            Items = items,
            ErrorMessage = result.Success ? null : (result.ErrorMessage ?? "Yorumlar alınırken bir hata oluştu.")
        };

        return View("Testimonials", vm);
    }

    /// <summary>
    /// Yeni müşteri yorumu ekleme sayfası.
    /// </summary>
    [HttpGet("/icerik/yorumlar/yeni")]
    public IActionResult CreateTestimonial()
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var vm = new SiteContentFormViewModel<TestimonialForm>
        {
            Form = new TestimonialForm
            {
                DisplayOrder = 1,
                IsActive = true
            },
            IsNew = true
        };

        return View("TestimonialForm", vm);
    }

    /// <summary>
    /// Yeni müşteri yorumunu kaydeder.
    /// </summary>
    [HttpPost("/icerik/yorumlar/yeni")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTestimonial(TestimonialForm form)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var payload = new
        {
            title = form.Title ?? string.Empty,
            name = form.Name ?? string.Empty,
            logoUrl = string.Empty,
            testimonail = form.Testimonail ?? string.Empty,
            isActive = form.IsActive,
            displayOrder = form.DisplayOrder
        };

        var postResult = await _apiClient.PostAsync<string>("Testimonails", payload);

        if (postResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }
        if (postResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        if (postResult.Success)
        {
            TempData["Success"] = "Yorum eklendi.";
            return Redirect("/icerik/yorumlar");
        }

        var vm = new SiteContentFormViewModel<TestimonialForm>
        {
            Form = form,
            IsNew = true,
            FieldErrors = postResult.Errors != null
                ? new Dictionary<string, string[]>(postResult.Errors, StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase),
            ErrorMessage = postResult.ErrorMessage ?? "Lütfen formdaki hataları kontrol ediniz."
        };

        return View("TestimonialForm", vm);
    }

    /// <summary>
    /// Müşteri yorumu düzenleme sayfası.
    /// </summary>
    [HttpGet("/icerik/yorumlar/{id:int}")]
    public async Task<IActionResult> EditTestimonial(int id)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var listResult = await _apiClient.GetAsync<List<TestimonialDto>>("Testimonails/admin");

        if (listResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }
        if (listResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        var item = listResult.Data?.FirstOrDefault(x => x.Id == id);
        if (item == null)
        {
            return NotFound();
        }

        var form = new TestimonialForm
        {
            Id = item.Id,
            Name = item.Name,
            Title = item.Title,
            Testimonail = item.Testimonail,
            DisplayOrder = item.DisplayOrder,
            IsActive = item.IsActive
        };

        var vm = new SiteContentFormViewModel<TestimonialForm>
        {
            Form = form,
            IsNew = false
        };

        return View("TestimonialForm", vm);
    }

    /// <summary>
    /// Müşteri yorumunu günceller.
    /// </summary>
    [HttpPost("/icerik/yorumlar/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditTestimonial(int id, TestimonialForm form)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        form.Id = id;

        var payload = new
        {
            id = id,
            name = form.Name ?? string.Empty,
            title = form.Title ?? string.Empty,
            testimonail = form.Testimonail ?? string.Empty,
            displayOrder = form.DisplayOrder,
            isActive = form.IsActive,
            logoUrl = string.Empty
        };

        var putResult = await _apiClient.PutAsync<string>("Testimonails", payload);

        if (putResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }
        if (putResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }
        if (putResult.StatusCode == StatusCodes.Status404NotFound)
        {
            return NotFound();
        }

        if (putResult.Success)
        {
            TempData["Success"] = "Yorum güncellendi.";
            return Redirect($"/icerik/yorumlar/{id}");
        }

        var vm = new SiteContentFormViewModel<TestimonialForm>
        {
            Form = form,
            IsNew = false,
            FieldErrors = putResult.Errors != null
                ? new Dictionary<string, string[]>(putResult.Errors, StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase),
            ErrorMessage = putResult.ErrorMessage ?? "Lütfen formdaki hataları kontrol ediniz."
        };

        return View("TestimonialForm", vm);
    }

    /// <summary>
    /// Müşteri yorumunu siler.
    /// </summary>
    [HttpPost("/icerik/yorumlar/{id:int}/sil")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteTestimonial(int id)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var result = await _apiClient.DeleteAsync<string>($"Testimonails?id={id}");

        if (result.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }
        if (result.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        if (result.Success)
        {
            TempData["Success"] = "Yorum silindi.";
            return Redirect("/icerik/yorumlar");
        }

        TempData["Error"] = result.ErrorMessage ?? "Yorum silinirken bir hata oluştu.";
        return Redirect("/icerik/yorumlar");
    }

    #endregion

    #region 2. İş Ortakları

    /// <summary>
    /// İş ortakları listesi sayfası.
    /// </summary>
    [HttpGet("/icerik/is-ortaklari")]
    public async Task<IActionResult> Partners()
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var result = await _apiClient.GetAsync<List<PartnerDto>>("OurPartners/admin");

        if (result.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }
        if (result.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        var items = (result.Data ?? new List<PartnerDto>())
            .OrderBy(x => x.DisplayOrder)
            .ThenByDescending(x => x.Id)
            .ToList();

        var vm = new SiteContentListViewModel<PartnerDto>
        {
            Items = items,
            ErrorMessage = result.Success ? null : (result.ErrorMessage ?? "İş ortakları alınırken bir hata oluştu.")
        };

        return View("Partners", vm);
    }

    /// <summary>
    /// Yeni iş ortağı ekleme sayfası.
    /// </summary>
    [HttpGet("/icerik/is-ortaklari/yeni")]
    public IActionResult CreatePartner()
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var vm = new SiteContentFormViewModel<PartnerForm>
        {
            Form = new PartnerForm
            {
                DisplayOrder = 1,
                IsActive = true
            },
            IsNew = true
        };

        return View("PartnerForm", vm);
    }

    /// <summary>
    /// Yeni iş ortağını kaydeder.
    /// </summary>
    [HttpPost("/icerik/is-ortaklari/yeni")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePartner(PartnerForm form)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var payload = new
        {
            partnerName = form.PartnerName ?? string.Empty,
            partnerLogoUrl = form.PartnerLogoUrl ?? string.Empty,
            displayOrder = form.DisplayOrder,
            isActive = form.IsActive
        };

        var postResult = await _apiClient.PostAsync<string>("OurPartners", payload);

        if (postResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }
        if (postResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        if (postResult.Success)
        {
            TempData["Success"] = "İş ortağı eklendi.";
            return Redirect("/icerik/is-ortaklari");
        }

        var vm = new SiteContentFormViewModel<PartnerForm>
        {
            Form = form,
            IsNew = true,
            FieldErrors = postResult.Errors != null
                ? new Dictionary<string, string[]>(postResult.Errors, StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase),
            ErrorMessage = postResult.ErrorMessage ?? "Lütfen formdaki hataları kontrol ediniz."
        };

        return View("PartnerForm", vm);
    }

    /// <summary>
    /// İş ortağı düzenleme sayfası.
    /// </summary>
    [HttpGet("/icerik/is-ortaklari/{id:int}")]
    public async Task<IActionResult> EditPartner(int id)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var listResult = await _apiClient.GetAsync<List<PartnerDto>>("OurPartners/admin");

        if (listResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }
        if (listResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        var item = listResult.Data?.FirstOrDefault(x => x.Id == id);
        if (item == null)
        {
            return NotFound();
        }

        var form = new PartnerForm
        {
            Id = item.Id,
            PartnerName = item.PartnerName,
            PartnerLogoUrl = item.PartnerLogoUrl,
            DisplayOrder = item.DisplayOrder,
            IsActive = item.IsActive
        };

        var vm = new SiteContentFormViewModel<PartnerForm>
        {
            Form = form,
            IsNew = false
        };

        return View("PartnerForm", vm);
    }

    /// <summary>
    /// İş ortağını günceller.
    /// </summary>
    [HttpPost("/icerik/is-ortaklari/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPartner(int id, PartnerForm form)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        form.Id = id;

        var payload = new
        {
            id = id,
            partnerName = form.PartnerName ?? string.Empty,
            partnerLogoUrl = form.PartnerLogoUrl ?? string.Empty,
            displayOrder = form.DisplayOrder,
            isActive = form.IsActive
        };

        var putResult = await _apiClient.PutAsync<string>("OurPartners", payload);

        if (putResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }
        if (putResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }
        if (putResult.StatusCode == StatusCodes.Status404NotFound)
        {
            return NotFound();
        }

        if (putResult.Success)
        {
            TempData["Success"] = "İş ortağı güncellendi.";
            return Redirect($"/icerik/is-ortaklari/{id}");
        }

        var vm = new SiteContentFormViewModel<PartnerForm>
        {
            Form = form,
            IsNew = false,
            FieldErrors = putResult.Errors != null
                ? new Dictionary<string, string[]>(putResult.Errors, StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase),
            ErrorMessage = putResult.ErrorMessage ?? "Lütfen formdaki hataları kontrol ediniz."
        };

        return View("PartnerForm", vm);
    }

    /// <summary>
    /// İş ortağını siler.
    /// </summary>
    [HttpPost("/icerik/is-ortaklari/{id:int}/sil")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePartner(int id)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var result = await _apiClient.DeleteAsync<string>($"OurPartners?id={id}");

        if (result.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }
        if (result.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        if (result.Success)
        {
            TempData["Success"] = "İş ortağı silindi.";
            return Redirect("/icerik/is-ortaklari");
        }

        TempData["Error"] = result.ErrorMessage ?? "İş ortağı silinirken bir hata oluştu.";
        return Redirect("/icerik/is-ortaklari");
    }

    #endregion

    #region 3. Ana Sayfa Görselleri

    /// <summary>
    /// Ana sayfa görsel yuvaları listesi ve düzenleme sayfası.
    /// </summary>
    [HttpGet("/icerik/ana-sayfa-gorselleri")]
    public async Task<IActionResult> HomeImages()
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var result = await _apiClient.GetAsync<List<HomeSectionImageDto>>("HomeSectionImages");

        if (result.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }
        if (result.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        var vm = new HomeImagesViewModel
        {
            Items = result.Data ?? new List<HomeSectionImageDto>(),
            ErrorMessage = result.Success ? null : (result.ErrorMessage ?? "Ana sayfa görselleri alınırken bir hata oluştu.")
        };

        return View("HomeImages", vm);
    }

    /// <summary>
    /// Belirtilen ana sayfa görsel yuvasını kaydeder (varsa günceller, yoksa ekler).
    /// </summary>
    [HttpPost("/icerik/ana-sayfa-gorselleri/kaydet")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveHomeImage(HomeSectionImageForm form)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        bool isUpdate = form.HomeSectionImageId.HasValue && form.HomeSectionImageId.Value > 0;
        ApiResult<string> apiResult;

        if (isUpdate)
        {
            var updatePayload = new
            {
                homeSectionImageId = form.HomeSectionImageId!.Value,
                sectionKey = form.SectionKey ?? string.Empty,
                slot = form.Slot,
                imageUrl = form.ImageUrl ?? string.Empty,
                altText = form.AltText ?? string.Empty,
                isActive = form.IsActive
            };
            apiResult = await _apiClient.PutAsync<string>("HomeSectionImages", updatePayload);
        }
        else
        {
            var createPayload = new
            {
                sectionKey = form.SectionKey ?? string.Empty,
                slot = form.Slot,
                imageUrl = form.ImageUrl ?? string.Empty,
                altText = form.AltText ?? string.Empty,
                isActive = form.IsActive
            };
            apiResult = await _apiClient.PostAsync<string>("HomeSectionImages", createPayload);
        }

        if (apiResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }
        if (apiResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        if (apiResult.Success)
        {
            TempData["Success"] = "Görsel kaydedildi.";
            return Redirect("/icerik/ana-sayfa-gorselleri");
        }

        // Hata durumunda mevcut listeyi çekip o kartın altında hataları göster
        var listResult = await _apiClient.GetAsync<List<HomeSectionImageDto>>("HomeSectionImages");
        var cardKey = $"{form.SectionKey}-{form.Slot}";

        var vm = new HomeImagesViewModel
        {
            Items = listResult.Data ?? new List<HomeSectionImageDto>(),
            ActiveCardKey = cardKey,
            ErrorMessage = apiResult.ErrorMessage ?? "Lütfen formdaki hataları kontrol ediniz."
        };

        if (apiResult.Errors != null)
        {
            foreach (var (k, v) in apiResult.Errors)
            {
                vm.FieldErrors[$"{cardKey}.{k}"] = v;
                vm.FieldErrors[k] = v;
            }
        }
        if (!string.IsNullOrWhiteSpace(apiResult.ErrorMessage))
        {
            vm.FieldErrors[cardKey] = new[] { apiResult.ErrorMessage };
        }

        return View("HomeImages", vm);
    }

    #endregion

    #region 4. Şehir Galerisi

    /// <summary>
    /// Şehir galerisi listesi sayfası.
    /// </summary>
    [HttpGet("/icerik/sehir-galerisi")]
    public async Task<IActionResult> CityGallery()
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var result = await _apiClient.GetAsync<List<CityGalleryImageDto>>("CityGalleryImage/admin");

        if (result.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }
        if (result.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        var items = (result.Data ?? new List<CityGalleryImageDto>())
            .OrderBy(x => x.CityName)
            .ThenBy(x => x.DisplayOrder)
            .ThenByDescending(x => x.CityGalleryImageId)
            .ToList();

        var vm = new SiteContentListViewModel<CityGalleryImageDto>
        {
            Items = items,
            ErrorMessage = result.Success ? null : (result.ErrorMessage ?? "Şehir galerisi alınırken bir hata oluştu.")
        };

        return View("CityGallery", vm);
    }

    /// <summary>
    /// Yeni şehir görseli ekleme sayfası.
    /// </summary>
    [HttpGet("/icerik/sehir-galerisi/yeni")]
    public async Task<IActionResult> CreateCityGalleryImage()
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var cities = await LoadCitiesAsync();

        var vm = new SiteContentFormViewModel<CityGalleryImageForm>
        {
            Form = new CityGalleryImageForm
            {
                DisplayOrder = 1,
                IsActive = true,
                CityId = cities.FirstOrDefault()?.Id ?? 0
            },
            IsNew = true,
            Cities = cities
        };

        return View("CityGalleryForm", vm);
    }

    /// <summary>
    /// Yeni şehir görselini kaydeder.
    /// </summary>
    [HttpPost("/icerik/sehir-galerisi/yeni")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCityGalleryImage(CityGalleryImageForm form)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var payload = new
        {
            cityId = form.CityId,
            imageUrl = form.ImageUrl ?? string.Empty,
            title = form.Title ?? string.Empty,
            description = form.Description ?? string.Empty,
            displayOrder = form.DisplayOrder,
            isActive = form.IsActive
        };

        var postResult = await _apiClient.PostAsync<string>("CityGalleryImage", payload);

        if (postResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }
        if (postResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        if (postResult.Success)
        {
            TempData["Success"] = "Şehir görseli eklendi.";
            return Redirect("/icerik/sehir-galerisi");
        }

        var cities = await LoadCitiesAsync();

        var vm = new SiteContentFormViewModel<CityGalleryImageForm>
        {
            Form = form,
            IsNew = true,
            Cities = cities,
            FieldErrors = postResult.Errors != null
                ? new Dictionary<string, string[]>(postResult.Errors, StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase),
            ErrorMessage = postResult.ErrorMessage ?? "Lütfen formdaki hataları kontrol ediniz."
        };

        return View("CityGalleryForm", vm);
    }

    /// <summary>
    /// Şehir görseli düzenleme sayfası.
    /// </summary>
    [HttpGet("/icerik/sehir-galerisi/{id:int}")]
    public async Task<IActionResult> EditCityGalleryImage(int id)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var listTask = _apiClient.GetAsync<List<CityGalleryImageDto>>("CityGalleryImage/admin");
        var citiesTask = LoadCitiesAsync();

        await Task.WhenAll(listTask, citiesTask);

        var listResult = await listTask;
        var cities = await citiesTask;

        if (listResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }
        if (listResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        var item = listResult.Data?.FirstOrDefault(x => x.CityGalleryImageId == id);
        if (item == null)
        {
            return NotFound();
        }

        var form = new CityGalleryImageForm
        {
            CityGalleryImageId = item.CityGalleryImageId,
            CityId = item.CityId,
            ImageUrl = item.ImageUrl,
            Title = item.Title,
            Description = item.Description,
            DisplayOrder = item.DisplayOrder,
            IsActive = item.IsActive
        };

        var vm = new SiteContentFormViewModel<CityGalleryImageForm>
        {
            Form = form,
            IsNew = false,
            Cities = cities
        };

        return View("CityGalleryForm", vm);
    }

    /// <summary>
    /// Şehir görselini günceller.
    /// </summary>
    [HttpPost("/icerik/sehir-galerisi/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCityGalleryImage(int id, CityGalleryImageForm form)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        form.CityGalleryImageId = id;

        var payload = new
        {
            cityGalleryImageId = id,
            cityId = form.CityId,
            imageUrl = form.ImageUrl ?? string.Empty,
            title = form.Title ?? string.Empty,
            description = form.Description ?? string.Empty,
            displayOrder = form.DisplayOrder,
            isActive = form.IsActive
        };

        var putResult = await _apiClient.PutAsync<string>("CityGalleryImage", payload);

        if (putResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }
        if (putResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }
        if (putResult.StatusCode == StatusCodes.Status404NotFound)
        {
            return NotFound();
        }

        if (putResult.Success)
        {
            TempData["Success"] = "Şehir görseli güncellendi.";
            return Redirect($"/icerik/sehir-galerisi/{id}");
        }

        var cities = await LoadCitiesAsync();

        var vm = new SiteContentFormViewModel<CityGalleryImageForm>
        {
            Form = form,
            IsNew = false,
            Cities = cities,
            FieldErrors = putResult.Errors != null
                ? new Dictionary<string, string[]>(putResult.Errors, StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase),
            ErrorMessage = putResult.ErrorMessage ?? "Lütfen formdaki hataları kontrol ediniz."
        };

        return View("CityGalleryForm", vm);
    }

    /// <summary>
    /// Şehir görselini siler.
    /// </summary>
    [HttpPost("/icerik/sehir-galerisi/{id:int}/sil")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCityGalleryImage(int id)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var result = await _apiClient.DeleteAsync<string>($"CityGalleryImage?id={id}");

        if (result.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }
        if (result.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        if (result.Success)
        {
            TempData["Success"] = "Şehir görseli silindi.";
            return Redirect("/icerik/sehir-galerisi");
        }

        TempData["Error"] = result.ErrorMessage ?? "Şehir görseli silinirken bir hata oluştu.";
        return Redirect("/icerik/sehir-galerisi");
    }

    #endregion

    #region 5. Kredi Oranları

    /// <summary>
    /// Banka kredi faiz oranları listesi sayfası.
    /// </summary>
    [HttpGet("/icerik/kredi-oranlari")]
    public async Task<IActionResult> BankRates()
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var result = await _apiClient.GetAsync<List<BankLoanRateDto>>("BankLoanRates/admin");

        if (result.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }
        if (result.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        var items = (result.Data ?? new List<BankLoanRateDto>())
            .OrderBy(x => x.BankName)
            .ThenByDescending(x => x.BankLoanRateId)
            .ToList();

        var vm = new SiteContentListViewModel<BankLoanRateDto>
        {
            Items = items,
            ErrorMessage = result.Success ? null : (result.ErrorMessage ?? "Kredi oranları alınırken bir hata oluştu.")
        };

        return View("BankRates", vm);
    }

    /// <summary>
    /// Yeni banka kredi faiz oranı ekleme sayfası.
    /// </summary>
    [HttpGet("/icerik/kredi-oranlari/yeni")]
    public IActionResult CreateBankRate()
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var vm = new SiteContentFormViewModel<BankLoanRateForm>
        {
            Form = new BankLoanRateForm
            {
                MaxTermMonths = 120,
                AllocationFeeRate = 0.5m,
                IsActive = true
            },
            IsNew = true
        };

        return View("BankRateForm", vm);
    }

    /// <summary>
    /// Yeni banka kredi faiz oranını kaydeder.
    /// </summary>
    [HttpPost("/icerik/kredi-oranlari/yeni")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateBankRate(BankLoanRateForm form)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var payload = new
        {
            bankName = form.BankName ?? string.Empty,
            logoUrl = form.LogoUrl ?? string.Empty,
            housingLoanMonthlyRate = form.HousingLoanMonthlyRate,
            commercialLoanMonthlyRate = form.CommercialLoanMonthlyRate,
            maxTermMonths = form.MaxTermMonths,
            allocationFeeRate = form.AllocationFeeRate,
            isActive = form.IsActive
        };

        var postResult = await _apiClient.PostAsync<string>("BankLoanRates", payload);

        if (postResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }
        if (postResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        if (postResult.Success)
        {
            TempData["Success"] = "Kredi oranı eklendi.";
            return Redirect("/icerik/kredi-oranlari");
        }

        var vm = new SiteContentFormViewModel<BankLoanRateForm>
        {
            Form = form,
            IsNew = true,
            FieldErrors = postResult.Errors != null
                ? new Dictionary<string, string[]>(postResult.Errors, StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase),
            ErrorMessage = postResult.ErrorMessage ?? "Lütfen formdaki hataları kontrol ediniz."
        };

        return View("BankRateForm", vm);
    }

    /// <summary>
    /// Banka kredi faiz oranı düzenleme sayfası.
    /// </summary>
    [HttpGet("/icerik/kredi-oranlari/{id:int}")]
    public async Task<IActionResult> EditBankRate(int id)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var listResult = await _apiClient.GetAsync<List<BankLoanRateDto>>("BankLoanRates/admin");

        if (listResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }
        if (listResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        var item = listResult.Data?.FirstOrDefault(x => x.BankLoanRateId == id);
        if (item == null)
        {
            return NotFound();
        }

        var form = new BankLoanRateForm
        {
            BankLoanRateId = item.BankLoanRateId,
            BankName = item.BankName,
            LogoUrl = item.LogoUrl,
            HousingLoanMonthlyRate = item.HousingLoanMonthlyRate,
            CommercialLoanMonthlyRate = item.CommercialLoanMonthlyRate,
            MaxTermMonths = item.MaxTermMonths,
            AllocationFeeRate = item.AllocationFeeRate,
            IsActive = item.IsActive
        };

        var vm = new SiteContentFormViewModel<BankLoanRateForm>
        {
            Form = form,
            IsNew = false
        };

        return View("BankRateForm", vm);
    }

    /// <summary>
    /// Banka kredi faiz oranını günceller.
    /// </summary>
    [HttpPost("/icerik/kredi-oranlari/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditBankRate(int id, BankLoanRateForm form)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        form.BankLoanRateId = id;

        var payload = new
        {
            bankLoanRateId = id,
            bankName = form.BankName ?? string.Empty,
            logoUrl = form.LogoUrl ?? string.Empty,
            housingLoanMonthlyRate = form.HousingLoanMonthlyRate,
            commercialLoanMonthlyRate = form.CommercialLoanMonthlyRate,
            maxTermMonths = form.MaxTermMonths,
            allocationFeeRate = form.AllocationFeeRate,
            isActive = form.IsActive
        };

        var putResult = await _apiClient.PutAsync<string>("BankLoanRates", payload);

        if (putResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }
        if (putResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }
        if (putResult.StatusCode == StatusCodes.Status404NotFound)
        {
            return NotFound();
        }

        if (putResult.Success)
        {
            TempData["Success"] = "Kredi oranı güncellendi.";
            return Redirect($"/icerik/kredi-oranlari/{id}");
        }

        var vm = new SiteContentFormViewModel<BankLoanRateForm>
        {
            Form = form,
            IsNew = false,
            FieldErrors = putResult.Errors != null
                ? new Dictionary<string, string[]>(putResult.Errors, StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase),
            ErrorMessage = putResult.ErrorMessage ?? "Lütfen formdaki hataları kontrol ediniz."
        };

        return View("BankRateForm", vm);
    }

    /// <summary>
    /// Banka kredi faiz oranını siler.
    /// </summary>
    [HttpPost("/icerik/kredi-oranlari/{id:int}/sil")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteBankRate(int id)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var result = await _apiClient.DeleteAsync<string>($"BankLoanRates?id={id}");

        if (result.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }
        if (result.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        if (result.Success)
        {
            TempData["Success"] = "Kredi oranı silindi.";
            return Redirect("/icerik/kredi-oranlari");
        }

        TempData["Error"] = result.ErrorMessage ?? "Kredi oranı silinirken bir hata oluştu.";
        return Redirect("/icerik/kredi-oranlari");
    }

    #endregion

    #region Ortak Yardımcılar

    private async Task<List<CityOptionDto>> LoadCitiesAsync()
    {
        var result = await _apiClient.GetAnonymousAsync<List<CityOptionDto>>("Location/cities");
        return result.Data ?? new List<CityOptionDto>();
    }

    #endregion
}
