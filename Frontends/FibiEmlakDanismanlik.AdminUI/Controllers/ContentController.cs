using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FibiEmlakDanismanlik.AdminUI.Infrastructure;
using FibiEmlakDanismanlik.AdminUI.Models.Content;

namespace FibiEmlakDanismanlik.AdminUI.Controllers;

/// <summary>
/// İçerik yönetimi (Hakkımızda, İletişim, Sosyal Medya, SSS, Banner) süreçlerini yöneten denetleyici.
/// </summary>
[Authorize]
public sealed class ContentController : Controller
{
    private const string CacheNotice = "Sitede en geç 5 dakika içinde görünür.";
    private readonly ApiClient _apiClient;
    private readonly CurrentAdmin _currentAdmin;

    public ContentController(ApiClient apiClient, CurrentAdmin currentAdmin)
    {
        _apiClient = apiClient;
        _currentAdmin = currentAdmin;
    }

    /// <summary>
    /// İçerik ana sayfası.
    /// </summary>
    [HttpGet("/icerik")]
    public async Task<IActionResult> Index(string? sekme)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var (vm, authRedirect) = await LoadViewModelAsync(sekme);
        if (authRedirect == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }
        if (authRedirect == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        return View("Index", vm);
    }

    #region Hakkımızda

    /// <summary>
    /// Hakkımızda içeriğini günceller.
    /// </summary>
    [HttpPost("/icerik/hakkimizda")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateAboutUs(AboutUsDto form)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var payload = new
        {
            aboutUsId = form.AboutUsId,
            aboutUsTitle = form.AboutUsTitle,
            aboutUsDesc = form.AboutUsDesc,
            aboutUsImgUrl = form.AboutUsImgUrl
        };

        var result = await _apiClient.PutAsync<string>("AboutUs", payload);

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
            TempData["Success"] = $"Hakkımızda güncellendi. {CacheNotice}";
            return Redirect("/icerik?sekme=hakkimizda");
        }

        if (result.StatusCode == StatusCodes.Status400BadRequest)
        {
            var (vm, authRedirect) = await LoadViewModelAsync("hakkimizda");
            if (authRedirect == StatusCodes.Status401Unauthorized) return Redirect("/giris");
            if (authRedirect == StatusCodes.Status403Forbidden) return Redirect("/yetkisiz");

            vm.AboutUs = form;
            vm.FormKey = "about";
            if (result.Errors != null)
            {
                vm.FieldErrors = new Dictionary<string, string[]>(result.Errors, StringComparer.OrdinalIgnoreCase);
            }
            vm.ErrorMessage = result.ErrorMessage ?? "Lütfen formdaki hataları kontrol ediniz.";
            return View("Index", vm);
        }

        if (result.StatusCode == StatusCodes.Status404NotFound)
        {
            TempData["Error"] = "Kayıt bulunamadı.";
            return Redirect("/icerik?sekme=hakkimizda");
        }

        TempData["Error"] = result.ErrorMessage ?? "Hakkımızda güncellenirken bir hata oluştu.";
        return Redirect("/icerik?sekme=hakkimizda");
    }

    #endregion

    #region İletişim

    /// <summary>
    /// İletişim bilgilerini günceller.
    /// </summary>
    [HttpPost("/icerik/iletisim")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateContact(ContactDto form)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var payload = new
        {
            contactId = form.ContactId,
            eMail1 = form.EMail1,
            eMail2 = form.EMail2,
            phoneNumber1 = form.PhoneNumber1,
            phoneNumber2 = form.PhoneNumber2,
            officeAddress = form.OfficeAddress
        };

        var result = await _apiClient.PutAsync<string>("Contact", payload);

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
            TempData["Success"] = $"İletişim bilgileri güncellendi. {CacheNotice}";
            return Redirect("/icerik?sekme=iletisim");
        }

        if (result.StatusCode == StatusCodes.Status400BadRequest)
        {
            var (vm, authRedirect) = await LoadViewModelAsync("iletisim");
            if (authRedirect == StatusCodes.Status401Unauthorized) return Redirect("/giris");
            if (authRedirect == StatusCodes.Status403Forbidden) return Redirect("/yetkisiz");

            vm.Contact = form;
            vm.FormKey = "contact";
            if (result.Errors != null)
            {
                vm.FieldErrors = new Dictionary<string, string[]>(result.Errors, StringComparer.OrdinalIgnoreCase);
            }
            vm.ErrorMessage = result.ErrorMessage ?? "Lütfen formdaki hataları kontrol ediniz.";
            return View("Index", vm);
        }

        if (result.StatusCode == StatusCodes.Status404NotFound)
        {
            TempData["Error"] = "Kayıt bulunamadı.";
            return Redirect("/icerik?sekme=iletisim");
        }

        TempData["Error"] = result.ErrorMessage ?? "İletişim bilgileri güncellenirken bir hata oluştu.";
        return Redirect("/icerik?sekme=iletisim");
    }

    #endregion

    #region Sosyal Medya

    /// <summary>
    /// Yeni sosyal medya hesabı ekler.
    /// </summary>
    [HttpPost("/icerik/sosyal-medya")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSocialMedia(SocialMediaDto form)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var payload = new
        {
            name = form.Name,
            url = form.Url,
            icon = form.Icon,
            order = form.Order,
            isActive = form.IsActive
        };

        var result = await _apiClient.PostAsync<string>("SocialMedia", payload);

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
            TempData["Success"] = $"Sosyal medya hesabı eklendi. {CacheNotice}";
            return Redirect("/icerik?sekme=sosyal-medya");
        }

        if (result.StatusCode == StatusCodes.Status400BadRequest)
        {
            var (vm, authRedirect) = await LoadViewModelAsync("sosyal-medya");
            if (authRedirect == StatusCodes.Status401Unauthorized) return Redirect("/giris");
            if (authRedirect == StatusCodes.Status403Forbidden) return Redirect("/yetkisiz");

            vm.NewSocial = form;
            vm.FormKey = "social-new";
            if (result.Errors != null)
            {
                vm.FieldErrors = new Dictionary<string, string[]>(result.Errors, StringComparer.OrdinalIgnoreCase);
            }
            vm.ErrorMessage = result.ErrorMessage ?? "Lütfen formdaki hataları kontrol ediniz.";
            return View("Index", vm);
        }

        TempData["Error"] = result.ErrorMessage ?? "Sosyal medya hesabı eklenirken bir hata oluştu.";
        return Redirect("/icerik?sekme=sosyal-medya");
    }

    /// <summary>
    /// Sosyal medya hesabını günceller.
    /// </summary>
    [HttpPost("/icerik/sosyal-medya/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateSocialMedia(int id, SocialMediaDto form)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        form.Id = id;
        var payload = new
        {
            id = form.Id,
            name = form.Name,
            url = form.Url,
            icon = form.Icon,
            order = form.Order,
            isActive = form.IsActive
        };

        var result = await _apiClient.PutAsync<string>("SocialMedia", payload);

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
            TempData["Success"] = $"Sosyal medya hesabı güncellendi. {CacheNotice}";
            return Redirect("/icerik?sekme=sosyal-medya");
        }

        if (result.StatusCode == StatusCodes.Status400BadRequest)
        {
            var (vm, authRedirect) = await LoadViewModelAsync("sosyal-medya");
            if (authRedirect == StatusCodes.Status401Unauthorized) return Redirect("/giris");
            if (authRedirect == StatusCodes.Status403Forbidden) return Redirect("/yetkisiz");

            var idx = vm.SocialMedia.FindIndex(s => s.Id == id);
            if (idx >= 0)
            {
                vm.SocialMedia[idx] = form;
            }
            vm.FormKey = $"social-{id}";
            if (result.Errors != null)
            {
                vm.FieldErrors = new Dictionary<string, string[]>(result.Errors, StringComparer.OrdinalIgnoreCase);
            }
            vm.ErrorMessage = result.ErrorMessage ?? "Lütfen formdaki hataları kontrol ediniz.";
            return View("Index", vm);
        }

        if (result.StatusCode == StatusCodes.Status404NotFound)
        {
            TempData["Error"] = "Kayıt bulunamadı.";
            return Redirect("/icerik?sekme=sosyal-medya");
        }

        TempData["Error"] = result.ErrorMessage ?? "Sosyal medya hesabı güncellenirken bir hata oluştu.";
        return Redirect("/icerik?sekme=sosyal-medya");
    }

    /// <summary>
    /// Sosyal medya hesabını siler.
    /// </summary>
    [HttpPost("/icerik/sosyal-medya/{id:int}/sil")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSocialMedia(int id)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var result = await _apiClient.DeleteAsync<string>($"SocialMedia?id={id}");

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
            TempData["Success"] = $"Sosyal medya hesabı silindi. {CacheNotice}";
            return Redirect("/icerik?sekme=sosyal-medya");
        }

        if (result.StatusCode == StatusCodes.Status404NotFound)
        {
            TempData["Error"] = "Kayıt bulunamadı.";
            return Redirect("/icerik?sekme=sosyal-medya");
        }

        TempData["Error"] = result.ErrorMessage ?? "Sosyal medya hesabı silinirken bir hata oluştu.";
        return Redirect("/icerik?sekme=sosyal-medya");
    }

    #endregion

    #region SSS

    /// <summary>
    /// Yeni soru ekler.
    /// </summary>
    [HttpPost("/icerik/sss")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateFaq(FaqDto form)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var payload = new
        {
            question = form.Question,
            answer = form.Answer
        };

        var result = await _apiClient.PostAsync<string>("FrequentlyAskedQuestion", payload);

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
            TempData["Success"] = $"Soru eklendi. {CacheNotice}";
            return Redirect("/icerik?sekme=sss");
        }

        if (result.StatusCode == StatusCodes.Status400BadRequest)
        {
            var (vm, authRedirect) = await LoadViewModelAsync("sss");
            if (authRedirect == StatusCodes.Status401Unauthorized) return Redirect("/giris");
            if (authRedirect == StatusCodes.Status403Forbidden) return Redirect("/yetkisiz");

            vm.NewFaq = form;
            vm.FormKey = "faq-new";
            if (result.Errors != null)
            {
                vm.FieldErrors = new Dictionary<string, string[]>(result.Errors, StringComparer.OrdinalIgnoreCase);
            }
            vm.ErrorMessage = result.ErrorMessage ?? "Lütfen formdaki hataları kontrol ediniz.";
            return View("Index", vm);
        }

        TempData["Error"] = result.ErrorMessage ?? "Soru eklenirken bir hata oluştu.";
        return Redirect("/icerik?sekme=sss");
    }

    /// <summary>
    /// Soruyu günceller.
    /// </summary>
    [HttpPost("/icerik/sss/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateFaq(int id, FaqDto form)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        form.FrequentlyAskedQuestionId = id;
        var payload = new
        {
            frequentlyAskedQuestionId = form.FrequentlyAskedQuestionId,
            question = form.Question,
            answer = form.Answer
        };

        var result = await _apiClient.PutAsync<string>("FrequentlyAskedQuestion", payload);

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
            TempData["Success"] = $"Soru güncellendi. {CacheNotice}";
            return Redirect("/icerik?sekme=sss");
        }

        if (result.StatusCode == StatusCodes.Status400BadRequest)
        {
            var (vm, authRedirect) = await LoadViewModelAsync("sss");
            if (authRedirect == StatusCodes.Status401Unauthorized) return Redirect("/giris");
            if (authRedirect == StatusCodes.Status403Forbidden) return Redirect("/yetkisiz");

            var idx = vm.Faqs.FindIndex(f => f.FrequentlyAskedQuestionId == id);
            if (idx >= 0)
            {
                vm.Faqs[idx] = form;
            }
            vm.FormKey = $"faq-{id}";
            if (result.Errors != null)
            {
                vm.FieldErrors = new Dictionary<string, string[]>(result.Errors, StringComparer.OrdinalIgnoreCase);
            }
            vm.ErrorMessage = result.ErrorMessage ?? "Lütfen formdaki hataları kontrol ediniz.";
            return View("Index", vm);
        }

        if (result.StatusCode == StatusCodes.Status404NotFound)
        {
            TempData["Error"] = "Kayıt bulunamadı.";
            return Redirect("/icerik?sekme=sss");
        }

        TempData["Error"] = result.ErrorMessage ?? "Soru güncellenirken bir hata oluştu.";
        return Redirect("/icerik?sekme=sss");
    }

    /// <summary>
    /// Soruyu siler.
    /// </summary>
    [HttpPost("/icerik/sss/{id:int}/sil")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteFaq(int id)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var result = await _apiClient.DeleteAsync<string>($"FrequentlyAskedQuestion?id={id}");

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
            TempData["Success"] = $"Soru silindi. {CacheNotice}";
            return Redirect("/icerik?sekme=sss");
        }

        if (result.StatusCode == StatusCodes.Status404NotFound)
        {
            TempData["Error"] = "Kayıt bulunamadı.";
            return Redirect("/icerik?sekme=sss");
        }

        TempData["Error"] = result.ErrorMessage ?? "Soru silinirken bir hata oluştu.";
        return Redirect("/icerik?sekme=sss");
    }

    #endregion

    #region Banner

    /// <summary>
    /// Yeni banner ekler.
    /// </summary>
    [HttpPost("/icerik/banner")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateBanner(BannerDto form)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var payload = new
        {
            mainBannerUrl = form.MainBannerUrl,
            mainBannerDesc = form.MainBannerDesc
        };

        var result = await _apiClient.PostAsync<string>("MainBanner", payload);

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
            TempData["Success"] = $"Banner eklendi. {CacheNotice}";
            return Redirect("/icerik?sekme=banner");
        }

        if (result.StatusCode == StatusCodes.Status400BadRequest)
        {
            var (vm, authRedirect) = await LoadViewModelAsync("banner");
            if (authRedirect == StatusCodes.Status401Unauthorized) return Redirect("/giris");
            if (authRedirect == StatusCodes.Status403Forbidden) return Redirect("/yetkisiz");

            vm.NewBanner = form;
            vm.FormKey = "banner-new";
            if (result.Errors != null)
            {
                vm.FieldErrors = new Dictionary<string, string[]>(result.Errors, StringComparer.OrdinalIgnoreCase);
            }
            vm.ErrorMessage = result.ErrorMessage ?? "Lütfen formdaki hataları kontrol ediniz.";
            return View("Index", vm);
        }

        TempData["Error"] = result.ErrorMessage ?? "Banner eklenirken bir hata oluştu.";
        return Redirect("/icerik?sekme=banner");
    }

    /// <summary>
    /// Bannerı günceller.
    /// </summary>
    [HttpPost("/icerik/banner/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateBanner(int id, BannerDto form)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        form.MainBannerId = id;
        var payload = new
        {
            mainBannerId = form.MainBannerId,
            mainBannerUrl = form.MainBannerUrl,
            mainBannerDesc = form.MainBannerDesc
        };

        var result = await _apiClient.PutAsync<string>("MainBanner", payload);

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
            TempData["Success"] = $"Banner güncellendi. {CacheNotice}";
            return Redirect("/icerik?sekme=banner");
        }

        if (result.StatusCode == StatusCodes.Status400BadRequest)
        {
            var (vm, authRedirect) = await LoadViewModelAsync("banner");
            if (authRedirect == StatusCodes.Status401Unauthorized) return Redirect("/giris");
            if (authRedirect == StatusCodes.Status403Forbidden) return Redirect("/yetkisiz");

            var idx = vm.Banners.FindIndex(b => b.MainBannerId == id);
            if (idx >= 0)
            {
                vm.Banners[idx] = form;
            }
            vm.FormKey = $"banner-{id}";
            if (result.Errors != null)
            {
                vm.FieldErrors = new Dictionary<string, string[]>(result.Errors, StringComparer.OrdinalIgnoreCase);
            }
            vm.ErrorMessage = result.ErrorMessage ?? "Lütfen formdaki hataları kontrol ediniz.";
            return View("Index", vm);
        }

        if (result.StatusCode == StatusCodes.Status404NotFound)
        {
            TempData["Error"] = "Kayıt bulunamadı.";
            return Redirect("/icerik?sekme=banner");
        }

        TempData["Error"] = result.ErrorMessage ?? "Banner güncellenirken bir hata oluştu.";
        return Redirect("/icerik?sekme=banner");
    }

    /// <summary>
    /// Bannerı siler.
    /// </summary>
    [HttpPost("/icerik/banner/{id:int}/sil")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteBanner(int id)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var result = await _apiClient.DeleteAsync<string>($"MainBanner?id={id}");

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
            TempData["Success"] = $"Banner silindi. {CacheNotice}";
            return Redirect("/icerik?sekme=banner");
        }

        if (result.StatusCode == StatusCodes.Status404NotFound)
        {
            TempData["Error"] = "Kayıt bulunamadı.";
            return Redirect("/icerik?sekme=banner");
        }

        TempData["Error"] = result.ErrorMessage ?? "Banner silinirken bir hata oluştu.";
        return Redirect("/icerik?sekme=banner");
    }

    #endregion

    private async Task<(ContentIndexViewModel ViewModel, int? AuthRedirectStatusCode)> LoadViewModelAsync(string? sekme)
    {
        var aboutTask = _apiClient.GetAsync<AboutUsDto>("AboutUs/current");
        var contactTask = _apiClient.GetAsync<List<ContactDto>>("Contact");
        var socialTask = _apiClient.GetAsync<List<SocialMediaDto>>("SocialMedia");
        var faqTask = _apiClient.GetAsync<List<FaqDto>>("FrequentlyAskedQuestion");
        var bannerTask = _apiClient.GetAsync<List<BannerDto>>("MainBanner");

        await Task.WhenAll(aboutTask, contactTask, socialTask, faqTask, bannerTask);

        var aboutResult = await aboutTask;
        var contactResult = await contactTask;
        var socialResult = await socialTask;
        var faqResult = await faqTask;
        var bannerResult = await bannerTask;

        if (aboutResult.StatusCode == StatusCodes.Status401Unauthorized ||
            contactResult.StatusCode == StatusCodes.Status401Unauthorized ||
            socialResult.StatusCode == StatusCodes.Status401Unauthorized ||
            faqResult.StatusCode == StatusCodes.Status401Unauthorized ||
            bannerResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return (new ContentIndexViewModel(), StatusCodes.Status401Unauthorized);
        }

        if (aboutResult.StatusCode == StatusCodes.Status403Forbidden ||
            contactResult.StatusCode == StatusCodes.Status403Forbidden ||
            socialResult.StatusCode == StatusCodes.Status403Forbidden ||
            faqResult.StatusCode == StatusCodes.Status403Forbidden ||
            bannerResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            return (new ContentIndexViewModel(), StatusCodes.Status403Forbidden);
        }

        var vm = new ContentIndexViewModel
        {
            Sekme = NormalizeSekme(sekme),
            AboutUs = aboutResult.Data,
            Contact = contactResult.Data?.FirstOrDefault(),
            SocialMedia = socialResult.Data?.OrderBy(s => s.Order).ToList() ?? new List<SocialMediaDto>(),
            Faqs = faqResult.Data ?? new List<FaqDto>(),
            Banners = bannerResult.Data ?? new List<BannerDto>()
        };

        return (vm, null);
    }

    private static string NormalizeSekme(string? sekme) => sekme?.Trim().ToLowerInvariant() switch
    {
        "iletisim" => "iletisim",
        "sosyal-medya" => "sosyal-medya",
        "sss" => "sss",
        "banner" => "banner",
        _ => "hakkimizda"
    };
}
