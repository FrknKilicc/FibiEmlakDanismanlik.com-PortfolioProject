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
/// İçerik yönetimi (Hakkımızda, İletişim, Sosyal Medya, SSS, Banner) süreçlerini yöneten denetleyici.
/// </summary>
[Authorize]
public sealed class ContentController : Controller
{
    private const string CacheNotice = "Sitede en geç 5 dakika içinde görünür.";
    private readonly ApiClient _apiClient;
    private readonly CurrentAdmin _currentAdmin;
    private readonly SiteImageUrl _siteImageUrl;

    public ContentController(ApiClient apiClient, CurrentAdmin currentAdmin, SiteImageUrl siteImageUrl)
    {
        _apiClient = apiClient;
        _currentAdmin = currentAdmin;
        _siteImageUrl = siteImageUrl;
    }

    /// <summary>
    /// İçerik ana sayfası (sekme boşsa Hub, doluysa ilgili sekme içeriği).
    /// </summary>
    [HttpGet("/icerik")]
    public async Task<IActionResult> Index(string? sekme)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        if (string.IsNullOrWhiteSpace(sekme))
        {
            return await Hub();
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

    /// <summary>
    /// İçerik Merkezi (Hub) sayfası.
    /// </summary>
    private async Task<IActionResult> Hub()
    {
        var aboutTask = _apiClient.GetAsync<AboutUsDto>("AboutUs/current");
        var contactTask = _apiClient.GetAsync<List<ContactDto>>("Contact");
        var socialTask = _apiClient.GetAsync<List<SocialMediaDto>>("SocialMedia");
        var faqTask = _apiClient.GetAsync<List<FaqDto>>("FrequentlyAskedQuestion");
        var bannerTask = _apiClient.GetAsync<List<BannerDto>>("MainBanner");
        var testimonialsTask = _apiClient.GetAsync<List<TestimonialDto>>("Testimonails/admin");
        var partnersTask = _apiClient.GetAsync<List<PartnerDto>>("OurPartners/admin");
        var homeImagesTask = _apiClient.GetAsync<List<HomeSectionImageDto>>("HomeSectionImages");
        var cityGalleryTask = _apiClient.GetAsync<List<CityGalleryImageDto>>("CityGalleryImage/admin");
        var bankLoansTask = _apiClient.GetAsync<List<BankLoanRateDto>>("BankLoanRates/admin");
        var blogTask = _apiClient.GetAsync<List<BlogDto>>("Blog");

        await Task.WhenAll(
            aboutTask,
            contactTask,
            socialTask,
            faqTask,
            bannerTask,
            testimonialsTask,
            partnersTask,
            homeImagesTask,
            cityGalleryTask,
            bankLoansTask,
            blogTask);

        var aboutRes = await aboutTask;
        var contactRes = await contactTask;
        var socialRes = await socialTask;
        var faqRes = await faqTask;
        var bannerRes = await bannerTask;
        var testimonialsRes = await testimonialsTask;
        var partnersRes = await partnersTask;
        var homeImagesRes = await homeImagesTask;
        var cityGalleryRes = await cityGalleryTask;
        var bankLoansRes = await bankLoansTask;
        var blogRes = await blogTask;

        if (aboutRes.StatusCode == StatusCodes.Status401Unauthorized ||
            contactRes.StatusCode == StatusCodes.Status401Unauthorized ||
            socialRes.StatusCode == StatusCodes.Status401Unauthorized ||
            faqRes.StatusCode == StatusCodes.Status401Unauthorized ||
            bannerRes.StatusCode == StatusCodes.Status401Unauthorized ||
            testimonialsRes.StatusCode == StatusCodes.Status401Unauthorized ||
            partnersRes.StatusCode == StatusCodes.Status401Unauthorized ||
            homeImagesRes.StatusCode == StatusCodes.Status401Unauthorized ||
            cityGalleryRes.StatusCode == StatusCodes.Status401Unauthorized ||
            bankLoansRes.StatusCode == StatusCodes.Status401Unauthorized ||
            blogRes.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }

        if (aboutRes.StatusCode == StatusCodes.Status403Forbidden ||
            contactRes.StatusCode == StatusCodes.Status403Forbidden ||
            socialRes.StatusCode == StatusCodes.Status403Forbidden ||
            faqRes.StatusCode == StatusCodes.Status403Forbidden ||
            bannerRes.StatusCode == StatusCodes.Status403Forbidden ||
            testimonialsRes.StatusCode == StatusCodes.Status403Forbidden ||
            partnersRes.StatusCode == StatusCodes.Status403Forbidden ||
            homeImagesRes.StatusCode == StatusCodes.Status403Forbidden ||
            cityGalleryRes.StatusCode == StatusCodes.Status403Forbidden ||
            bankLoansRes.StatusCode == StatusCodes.Status403Forbidden ||
            blogRes.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        // 1. Kurumsal istatistikleri ve uyarıları
        string aboutStat = !aboutRes.Success
            ? "—"
            : (!string.IsNullOrWhiteSpace(aboutRes.Data?.AboutUsTitle) ? "Başlık var" : "Başlık yok");

        string contactStat = !contactRes.Success
            ? "—"
            : (contactRes.Data?.Any(c => !string.IsNullOrWhiteSpace(c.PhoneNumber1) || !string.IsNullOrWhiteSpace(c.EMail1) || !string.IsNullOrWhiteSpace(c.OfficeAddress)) == true
                ? "Tanımlı"
                : "Tanımsız");

        string socialStat = "—";
        string? socialWarning = null;
        if (socialRes.Success)
        {
            var socials = socialRes.Data ?? new List<SocialMediaDto>();
            var activeSocial = socials.Count(s => s.IsActive);
            var passiveSocial = socials.Count(s => !s.IsActive);
            socialStat = $"{activeSocial} aktif · {passiveSocial} pasif";
            if (passiveSocial > 0)
            {
                socialWarning = "Pasif bağlantı var";
            }
        }

        // 2. Ana Sayfa istatistikleri ve uyarıları
        string bannerStat = !bannerRes.Success
            ? "—"
            : $"{bannerRes.Data?.Count ?? 0} banner";

        string homeImagesStat = "—";
        string? homeImagesWarning = null;
        if (homeImagesRes.Success)
        {
            var homeImgs = homeImagesRes.Data ?? new List<HomeSectionImageDto>();
            var filledSlots = HomeSectionSlots.All.Count(slot =>
                homeImgs.Any(i => string.Equals(i.SectionKey, slot.SectionKey, StringComparison.OrdinalIgnoreCase) &&
                                 i.Slot == slot.Slot &&
                                 !string.IsNullOrWhiteSpace(i.ImageUrl)));
            homeImagesStat = $"{filledSlots}/3 yuva dolu";
            if (filledSlots < 3)
            {
                homeImagesWarning = "Boş yuva";
            }
        }

        string testimonialsStat = "—";
        string? testimonialsWarning = null;
        if (testimonialsRes.Success)
        {
            var testimonials = testimonialsRes.Data ?? new List<TestimonialDto>();
            var activeT = testimonials.Count(t => t.IsActive);
            var passiveT = testimonials.Count(t => !t.IsActive);
            testimonialsStat = $"{activeT} yayında · {passiveT} gizli";
            var hasDemo = testimonials.Any(t =>
                (!string.IsNullOrWhiteSpace(t.Testimonail) && t.Testimonail.Contains("Deneme", StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(t.Title) && t.Title.Contains("Deneme", StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(t.Name) && t.Name.Contains("Deneme", StringComparison.OrdinalIgnoreCase)));
            if (hasDemo)
            {
                testimonialsWarning = "Demo içerik var";
            }
        }

        string partnersStat = !partnersRes.Success
            ? "—"
            : $"{partnersRes.Data?.Count(p => p.IsActive) ?? 0} aktif · {partnersRes.Data?.Count(p => !p.IsActive) ?? 0} pasif";

        // 3. Bilgi ve Blog istatistikleri ve uyarıları
        string faqStat = !faqRes.Success
            ? "—"
            : $"{faqRes.Data?.Count ?? 0} soru";

        string blogStat = !blogRes.Success
            ? "—"
            : $"{blogRes.Data?.Count ?? 0} yazı";

        string authorStat = "—";
        string categoryStat = "—";
        if (blogRes.Success)
        {
            var blogs = blogRes.Data ?? new List<BlogDto>();
            var authorCount = blogs.Select(b => b.AuthorId).Where(id => id > 0).Distinct().Count();
            authorStat = authorCount > 0 ? $"{authorCount} yazar" : "Yazarları yönet";

            var categoryCount = blogs.Where(b => b.BlogCategoryId.HasValue).Select(b => b.BlogCategoryId!.Value).Distinct().Count();
            categoryStat = categoryCount > 0 ? $"{categoryCount} kategori" : "Kategorileri yönet";
        }

        // 4. Araçlar ve Sayfalar istatistikleri
        string cityGalleryStat = "—";
        if (cityGalleryRes.Success)
        {
            var cg = cityGalleryRes.Data ?? new List<CityGalleryImageDto>();
            var cityCount = cg.Select(c => c.CityId).Distinct().Count();
            cityGalleryStat = $"{cg.Count} görsel · {cityCount} il";
        }

        string bankLoansStat = "—";
        if (bankLoansRes.Success)
        {
            var loans = bankLoansRes.Data ?? new List<BankLoanRateDto>();
            var bankCount = loans
                .Select(l => l.BankName)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count();
            bankLoansStat = $"{bankCount} banka";
        }

        var viewModel = new ContentHubViewModel
        {
            Groups = new List<ContentHubGroup>
            {
                new ContentHubGroup
                {
                    Title = "Kurumsal",
                    Description = "Kurumsal kimlik, iletişim kanalları ve sosyal medya hesapları.",
                    Tiles = new List<ContentHubTile>
                    {
                        new ContentHubTile
                        {
                            Key = "hakkimizda",
                            Title = "Hakkımızda",
                            Description = "Şirket tanıtımı, vizyon, misyon ve kurumsal görsel.",
                            Icon = "bi-info-circle",
                            Url = "/icerik?sekme=hakkimizda",
                            SiteUrl = BuildSiteUrl("/AboutUs"),
                            Stat = aboutStat,
                            Warning = null
                        },
                        new ContentHubTile
                        {
                            Key = "iletisim",
                            Title = "İletişim Bilgileri",
                            Description = "Telefon, e-posta, adres ve ofis ulaşım bilgileri.",
                            Icon = "bi-telephone",
                            Url = "/icerik?sekme=iletisim",
                            SiteUrl = BuildSiteUrl("/Contact"),
                            Stat = contactStat,
                            Warning = null
                        },
                        new ContentHubTile
                        {
                            Key = "sosyal-medya",
                            Title = "Sosyal Medya",
                            Description = "Sosyal medya bağlantıları, ikonlar ve yönlendirmeler.",
                            Icon = "bi-share",
                            Url = "/icerik?sekme=sosyal-medya",
                            SiteUrl = null,
                            Stat = socialStat,
                            Warning = socialWarning
                        }
                    }
                },
                new ContentHubGroup
                {
                    Title = "Ana Sayfa",
                    Description = "Ana sayfada öne çıkan görsel, banner, yorum ve iş ortakları alanları.",
                    Tiles = new List<ContentHubTile>
                    {
                        new ContentHubTile
                        {
                            Key = "banner",
                            Title = "Banner",
                            Description = "Ana sayfa tepe bölümündeki manşet ve karşılama görselleri.",
                            Icon = "bi-image",
                            Url = "/icerik?sekme=banner",
                            SiteUrl = BuildSiteUrl("/"),
                            Stat = bannerStat,
                            Warning = null
                        },
                        new ContentHubTile
                        {
                            Key = "site-metinleri",
                            Title = "Site Metinleri",
                            Description = "Ana sayfa bölüm başlıkları, Neden Biz kartları, çağrı bandı, ilan sayıları ve benzer ilanlar.",
                            Icon = "bi-fonts",
                            Url = "/icerik/site-metinleri",
                            SiteUrl = BuildSiteUrl("/"),
                            Stat = "Başlıklar ve sayılar",
                            Warning = null
                        },
                        new ContentHubTile
                        {
                            Key = "ana-sayfa-gorselleri",
                            Title = "Ana Sayfa Görselleri",
                            Description = "Popüler yerler ve müşteri yorumları sabit bölüm görselleri.",
                            Icon = "bi-images",
                            Url = "/icerik/ana-sayfa-gorselleri",
                            SiteUrl = null,
                            Stat = homeImagesStat,
                            Warning = homeImagesWarning
                        },
                        new ContentHubTile
                        {
                            Key = "yorumlar",
                            Title = "Müşteri Yorumları",
                            Description = "Ana sayfada listelenen müşteri referans ve değerlendirmeleri.",
                            Icon = "bi-chat-quote",
                            Url = "/icerik/yorumlar",
                            SiteUrl = null,
                            Stat = testimonialsStat,
                            Warning = testimonialsWarning
                        },
                        new ContentHubTile
                        {
                            Key = "is-ortaklari",
                            Title = "İş Ortakları",
                            Description = "Kurumsal iş ortakları, çözüm ortakları ve logoları.",
                            Icon = "bi-building",
                            Url = "/icerik/is-ortaklari",
                            SiteUrl = null,
                            Stat = partnersStat,
                            Warning = null
                        }
                    }
                },
                new ContentHubGroup
                {
                    Title = "Bilgi ve Blog",
                    Description = "Sıkça sorulan sorular, blog makaleleri, kategoriler ve yazarlar.",
                    Tiles = new List<ContentHubTile>
                    {
                        new ContentHubTile
                        {
                            Key = "sss",
                            Title = "SSS",
                            Description = "Kullanıcıların en çok merak ettiği sorular ve cevapları.",
                            Icon = "bi-question-circle",
                            Url = "/icerik?sekme=sss",
                            SiteUrl = BuildSiteUrl("/FrequentlyAskedQuestion"),
                            Stat = faqStat,
                            Warning = null
                        },
                        new ContentHubTile
                        {
                            Key = "blog",
                            Title = "Blog",
                            Description = "Emlak sektörü, rehberler ve güncel blog makaleleri.",
                            Icon = "bi-journal-text",
                            Url = "/icerik/blog",
                            SiteUrl = BuildSiteUrl("/blog"),
                            Stat = blogStat,
                            Warning = null
                        },
                        new ContentHubTile
                        {
                            Key = "yazarlar",
                            Title = "Yazarlar",
                            Description = "Blog içeriklerini oluşturan yazarlar ve profilleri.",
                            Icon = "bi-people",
                            Url = "/icerik/blog/yazarlar",
                            SiteUrl = null,
                            Stat = authorStat,
                            Warning = null
                        },
                        new ContentHubTile
                        {
                            Key = "kategoriler",
                            Title = "Kategoriler",
                            Description = "Blog yazılarını sınıflandıran konu başlıkları.",
                            Icon = "bi-tags",
                            Url = "/icerik/blog/kategoriler",
                            SiteUrl = null,
                            Stat = categoryStat,
                            Warning = null
                        }
                    }
                },
                new ContentHubGroup
                {
                    Title = "Araçlar ve Sayfalar",
                    Description = "Şehir vitrini ve güncel banka konut kredisi oranları.",
                    Tiles = new List<ContentHubTile>
                    {
                        new ContentHubTile
                        {
                            Key = "sehir-galerisi",
                            Title = "Şehir Galerisi",
                            Description = "Şehirlere özel tanıtım görselleri ve galeri yönetimi.",
                            Icon = "bi-geo-alt",
                            Url = "/icerik/sehir-galerisi",
                            SiteUrl = BuildSiteUrl("/Contact"),
                            Stat = cityGalleryStat,
                            Warning = null
                        },
                        new ContentHubTile
                        {
                            Key = "kredi-oranlari",
                            Title = "Kredi Oranları",
                            Description = "Bankaların güncel konut ve ticari kredi faiz oranları tablosu.",
                            Icon = "bi-bank",
                            Url = "/icerik/kredi-oranlari",
                            SiteUrl = null,
                            Stat = bankLoansStat,
                            Warning = null
                        }
                    }
                }
            }
        };

        return View("Hub", viewModel);
    }

    private string? BuildSiteUrl(string path)
    {
        var origin = _siteImageUrl.SiteOrigin;
        if (string.IsNullOrWhiteSpace(origin))
        {
            return null;
        }

        var cleanOrigin = origin.TrimEnd('/');
        var cleanPath = path.StartsWith('/') ? path : "/" + path;
        return cleanOrigin + cleanPath;
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
            aboutUsTitle = form.AboutUsTitle ?? string.Empty,
            aboutUsDesc = form.AboutUsDesc ?? string.Empty,
            aboutUsImgUrl = form.AboutUsImgUrl ?? string.Empty
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
            eMail1 = form.EMail1 ?? string.Empty,
            eMail2 = form.EMail2 ?? string.Empty,
            phoneNumber1 = form.PhoneNumber1 ?? string.Empty,
            phoneNumber2 = form.PhoneNumber2 ?? string.Empty,
            officeAddress = form.OfficeAddress ?? string.Empty
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
            name = form.Name ?? string.Empty,
            url = form.Url ?? string.Empty,
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
            name = form.Name ?? string.Empty,
            url = form.Url ?? string.Empty,
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
            question = form.Question ?? string.Empty,
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
            question = form.Question ?? string.Empty,
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
            mainBannerUrl = form.MainBannerUrl ?? string.Empty,
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
            mainBannerUrl = form.MainBannerUrl ?? string.Empty,
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
