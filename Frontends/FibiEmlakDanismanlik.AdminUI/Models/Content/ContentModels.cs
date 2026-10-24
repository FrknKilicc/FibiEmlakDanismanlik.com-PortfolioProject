namespace FibiEmlakDanismanlik.AdminUI.Models.Content;

/// <summary>
/// Hakkımızda içeriği veri transfer nesnesi.
/// </summary>
public sealed class AboutUsDto
{
    public int AboutUsId { get; set; }
    public string? AboutUsTitle { get; set; }
    public string? AboutUsDesc { get; set; }
    public string? AboutUsImgUrl { get; set; }
}

/// <summary>
/// İletişim bilgileri veri transfer nesnesi.
/// </summary>
public sealed class ContactDto
{
    public int ContactId { get; set; }
    public string? EMail1 { get; set; }
    public string? EMail2 { get; set; }
    public string? PhoneNumber1 { get; set; }
    public string? PhoneNumber2 { get; set; }
    public string? OfficeAddress { get; set; }
}

/// <summary>
/// Sosyal medya hesabı veri transfer nesnesi.
/// </summary>
public sealed class SocialMediaDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Url { get; set; }
    public string? Icon { get; set; }
    public int Order { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? CreatedDate { get; set; }
}

/// <summary>
/// Sıkça sorulan soru veri transfer nesnesi.
/// </summary>
public sealed class FaqDto
{
    public int FrequentlyAskedQuestionId { get; set; }
    public string? Question { get; set; }
    public string? Answer { get; set; }
}

/// <summary>
/// Ana sayfa banner görseli veri transfer nesnesi.
/// </summary>
public sealed class BannerDto
{
    public int MainBannerId { get; set; }
    public string? MainBannerUrl { get; set; }
    public string? MainBannerDesc { get; set; }
}

/// <summary>
/// İçerik yönetim sayfası model görünümü.
/// </summary>
public sealed class ContentIndexViewModel
{
    public AboutUsDto? AboutUs { get; set; }
    public ContactDto? Contact { get; set; }
    public List<SocialMediaDto> SocialMedia { get; set; } = new();
    public List<FaqDto> Faqs { get; set; } = new();
    public List<BannerDto> Banners { get; set; } = new();

    public SocialMediaDto? NewSocial { get; set; }
    public FaqDto? NewFaq { get; set; }
    public BannerDto? NewBanner { get; set; }

    public string? Sekme { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, string[]> FieldErrors { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Hangi formun hatalı döndüğünü belirtir:
    /// "about", "contact", "social-new", "social-{id}", "faq-new", "faq-{id}", "banner-new", "banner-{id}".
    /// Hata alanları yalnızca o formda gösterilir.
    /// </summary>
    public string? FormKey { get; set; }

    public bool IsForm(string formKey) =>
        string.Equals(FormKey, formKey, StringComparison.OrdinalIgnoreCase);

    public string? GetFieldError(string key)
    {
        if (FieldErrors.TryGetValue(key, out var errs) && errs.Length > 0)
        {
            return errs[0];
        }
        return null;
    }

    public string? GetFieldError(string formKey, string key)
    {
        if (!IsForm(formKey))
        {
            return null;
        }
        return GetFieldError(key);
    }
}
