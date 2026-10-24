#nullable enable
using System;
using System.Collections.Generic;

namespace FibiEmlakDanismanlik.AdminUI.Models.Content;

#region API DTOs

/// <summary>
/// Müşteri yorumu API veri transfer nesnesi.
/// </summary>
public sealed class TestimonialDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Name { get; set; }
    public string? LogoUrl { get; set; }
    public string? Testimonail { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
}

/// <summary>
/// İş ortağı API veri transfer nesnesi.
/// </summary>
public sealed class PartnerDto
{
    public int Id { get; set; }
    public string? PartnerName { get; set; }
    public string? PartnerLogoUrl { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
}

/// <summary>
/// Ana sayfa bölüm görseli API veri transfer nesnesi.
/// </summary>
public sealed class HomeSectionImageDto
{
    public int HomeSectionImageId { get; set; }
    public string SectionKey { get; set; } = string.Empty;
    public int Slot { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// Şehir galerisi görseli API veri transfer nesnesi.
/// </summary>
public sealed class CityGalleryImageDto
{
    public int CityGalleryImageId { get; set; }
    public int CityId { get; set; }
    public string CityName { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// Banka kredi faiz oranı API veri transfer nesnesi.
/// </summary>
public sealed class BankLoanRateDto
{
    public int BankLoanRateId { get; set; }
    public string BankName { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string DataSource { get; set; } = string.Empty;
    public decimal HousingLoanMonthlyRate { get; set; }
    public decimal CommercialLoanMonthlyRate { get; set; }
    public int MaxTermMonths { get; set; }
    public decimal AllocationFeeRate { get; set; }
    public bool IsActive { get; set; }
    public DateTime UpdatedDate { get; set; }
    public string? ProviderKey { get; set; }
}

/// <summary>
/// İl seçim listesi öğesi veri transfer nesnesi (Location/cities yanıtı).
/// API hem "id"+"text" hem "id"+"name" biçiminde dönebileceği için ikisi de desteklenir.
/// </summary>
public sealed class CityOptionDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Text { get; set; }

    /// <summary>
    /// Şehir adını döndürür.
    /// </summary>
    public string CityName => !string.IsNullOrWhiteSpace(Text)
        ? Text
        : (!string.IsNullOrWhiteSpace(Name) ? Name : $"İl #{Id}");
}

#endregion

#region Form Models

/// <summary>
/// Müşteri yorumu ekleme/düzenleme form modeli.
/// </summary>
public sealed class TestimonialForm
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public string? Title { get; set; }
    public string? Testimonail { get; set; }
    public int DisplayOrder { get; set; } = 1;
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// İş ortağı ekleme/düzenleme form modeli.
/// </summary>
public sealed class PartnerForm
{
    public int? Id { get; set; }
    public string? PartnerName { get; set; }
    public string? PartnerLogoUrl { get; set; }
    public int DisplayOrder { get; set; } = 1;
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Ana sayfa bölüm görseli form modeli.
/// </summary>
public sealed class HomeSectionImageForm
{
    public int? HomeSectionImageId { get; set; }
    public string? SectionKey { get; set; }
    public int Slot { get; set; }
    public string? ImageUrl { get; set; }
    public string? AltText { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Şehir galerisi görseli form modeli.
/// </summary>
public sealed class CityGalleryImageForm
{
    public int? CityGalleryImageId { get; set; }
    public int CityId { get; set; }
    public string? ImageUrl { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int DisplayOrder { get; set; } = 1;
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Banka kredi faiz oranı form modeli.
/// </summary>
public sealed class BankLoanRateForm
{
    public int? BankLoanRateId { get; set; }
    public string? BankName { get; set; }
    public string? LogoUrl { get; set; }
    public decimal HousingLoanMonthlyRate { get; set; }
    public decimal CommercialLoanMonthlyRate { get; set; }
    public int MaxTermMonths { get; set; } = 120;
    public decimal AllocationFeeRate { get; set; } = 0.5m;
    public bool IsActive { get; set; } = true;
}

#endregion

#region View Models

/// <summary>
/// İçerik listeleri için genel görünüm modeli.
/// </summary>
public sealed class SiteContentListViewModel<T>
{
    public List<T> Items { get; set; } = new();
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// İçerik formları için genel görünüm modeli.
/// </summary>
public sealed class SiteContentFormViewModel<TForm>
{
    public TForm Form { get; set; } = default!;
    public bool IsNew { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, string[]> FieldErrors { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public List<CityOptionDto> Cities { get; set; } = new();

    public string? GetFieldError(string fieldName)
    {
        if (FieldErrors.TryGetValue(fieldName, out var errors) && errors.Length > 0)
        {
            return errors[0];
        }
        return null;
    }
}

/// <summary>
/// Ana sayfa görselleri yönetim ekranı görünüm modeli.
/// </summary>
public sealed class HomeImagesViewModel
{
    public List<HomeSectionImageDto> Items { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public Dictionary<string, string[]> FieldErrors { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public string? ActiveCardKey { get; set; }

    public string? GetFieldError(string cardKey, string fieldName)
    {
        var compoundKey = $"{cardKey}.{fieldName}";
        if (FieldErrors.TryGetValue(compoundKey, out var errs) && errs.Length > 0)
        {
            return errs[0];
        }
        if (FieldErrors.TryGetValue(fieldName, out var errs2) && errs2.Length > 0)
        {
            return errs2[0];
        }
        return null;
    }

    public string? GetCardError(string cardKey)
    {
        if (FieldErrors.TryGetValue(cardKey, out var errs) && errs.Length > 0)
        {
            return errs[0];
        }
        return null;
    }
}

#endregion

#region Slots Definition

/// <summary>
/// Ana sayfa sabit görsel yuvaları tanımları.
/// </summary>
public static class HomeSectionSlots
{
    public sealed record SlotDefinition(string SectionKey, int Slot, string Title);

    public static readonly IReadOnlyList<SlotDefinition> All = new List<SlotDefinition>
    {
        new("PopularPlaces", 1, "Popüler Yerler — sol görsel"),
        new("PopularPlaces", 2, "Popüler Yerler — sağ görsel"),
        new("Testimonials", 1, "Müşteri Yorumları — arka plan")
    };
}

#endregion
