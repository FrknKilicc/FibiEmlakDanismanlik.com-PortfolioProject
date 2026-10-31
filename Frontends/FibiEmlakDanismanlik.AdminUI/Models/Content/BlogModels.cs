#nullable enable
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FibiEmlakDanismanlik.AdminUI.Models.Content;

/// <summary>
/// API'den dönen blog yazısı veri transfer nesnesi.
/// </summary>
public sealed class BlogDto
{
    public int BlogId { get; set; }
    public string? BlogTitle { get; set; }
    public string? BlogDescription { get; set; }
    public string? BlogImgUrl { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool TopNews { get; set; }
    public int AuthorId { get; set; }
    public int? BlogCategoryId { get; set; }
    public List<string> Tags { get; set; } = new();
}

/// <summary>
/// Blog kategorisi veri transfer nesnesi.
/// </summary>
public sealed class BlogCategoryDto
{
    public int BlogCategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int BlogCount { get; set; }
}

/// <summary>
/// Yazar veri transfer nesnesi.
/// </summary>
public sealed class AuthorDto
{
    public int AuthorId { get; set; }
    public string? AuthorName { get; set; }
    public string? AuthorImgUrl { get; set; }
    public int BlogCount { get; set; }
}

/// <summary>
/// Blog ekleme/düzenleme form modeli.
/// </summary>
public sealed class BlogFormModel
{
    public int? BlogId { get; set; }
    public string? BlogTitle { get; set; }
    public string? BlogDescription { get; set; }
    public string? BlogImgUrl { get; set; }
    public string? CreatedDate { get; set; }
    public bool TopNews { get; set; }
    public int AuthorId { get; set; }
    public int? BlogCategoryId { get; set; }
    public string? TagsText { get; set; }
}

/// <summary>
/// Blog liste sayfası görünüm modeli.
/// </summary>
public sealed class BlogIndexViewModel
{
    public List<BlogDto> Blogs { get; set; } = new();
    public List<BlogCategoryDto> Categories { get; set; } = new();
    public List<AuthorDto> Authors { get; set; } = new();
    public string? Ara { get; set; }
    public int? Kategori { get; set; }
    public int Sayfa { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Blog form sayfası görünüm modeli.
/// </summary>
public sealed class BlogFormViewModel
{
    public BlogFormModel Form { get; set; } = new();
    public List<BlogCategoryDto> Categories { get; set; } = new();
    public List<AuthorDto> Authors { get; set; } = new();
    public List<string> AllTags { get; set; } = new();
    public bool IsNew { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, string[]> FieldErrors { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public string? GetFieldError(string key)
    {
        if (FieldErrors != null)
        {
            if (FieldErrors.TryGetValue(key, out var errors) && errors != null && errors.Length > 0)
            {
                return errors[0];
            }

            if (string.Equals(key, "Tags", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(key, "TagsText", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var kvp in FieldErrors)
                {
                    if ((kvp.Key.StartsWith("Tags[", StringComparison.OrdinalIgnoreCase) ||
                         kvp.Key.StartsWith("TagsText[", StringComparison.OrdinalIgnoreCase) ||
                         string.Equals(kvp.Key, "Tags", StringComparison.OrdinalIgnoreCase) ||
                         string.Equals(kvp.Key, "TagsText", StringComparison.OrdinalIgnoreCase)) &&
                        kvp.Value != null && kvp.Value.Length > 0)
                    {
                        return kvp.Value[0];
                    }
                }
            }
        }
        return null;
    }
}

/// <summary>
/// Yazar ekleme/düzenleme form modeli.
/// </summary>
public sealed class AuthorFormModel
{
    public int? AuthorId { get; set; }
    public string? AuthorName { get; set; }
    public string? AuthorImgUrl { get; set; }
}

/// <summary>
/// Blog kategorisi ekleme/düzenleme form modeli.
/// </summary>
public sealed class BlogCategoryFormModel
{
    public int? BlogCategoryId { get; set; }
    public string? CategoryName { get; set; }
}

/// <summary>
/// Blog yazarları yönetim sayfası görünüm modeli.
/// </summary>
public sealed class BlogAuthorsViewModel
{
    public List<AuthorDto> Authors { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public int? EditingId { get; set; }
    public AuthorFormModel NewForm { get; set; } = new();
    public Dictionary<string, string[]> FieldErrors { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public string? GetFieldError(string key)
    {
        if (FieldErrors != null && FieldErrors.TryGetValue(key, out var errors) && errors != null && errors.Length > 0)
        {
            return errors[0];
        }
        return null;
    }

    public string? GetFieldError(string formPrefix, string key)
    {
        return GetFieldError($"{formPrefix}:{key}");
    }
}

/// <summary>
/// Blog kategorileri yönetim sayfası görünüm modeli.
/// </summary>
public sealed class BlogCategoriesViewModel
{
    public List<BlogCategoryDto> Categories { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public int? EditingId { get; set; }
    public BlogCategoryFormModel NewForm { get; set; } = new();
    public Dictionary<string, string[]> FieldErrors { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public string? GetFieldError(string key)
    {
        if (FieldErrors != null && FieldErrors.TryGetValue(key, out var errors) && errors != null && errors.Length > 0)
        {
            return errors[0];
        }
        return null;
    }

    public string? GetFieldError(string formPrefix, string key)
    {
        return GetFieldError($"{formPrefix}:{key}");
    }
}

/// <summary>
/// Blog metin yardımcı sınıfı.
/// </summary>
public static class BlogText
{
    private static readonly Regex TagRegex = new("<[^>]+>", RegexOptions.Compiled);
    private static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions.Compiled);

    /// <summary>
    /// HTML etiketlerini temizleyip belirtilen karakter sınırında özet üretir.
    /// </summary>
    public static string Excerpt(string? html, int max = 140)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return string.Empty;
        }

        var stripped = TagRegex.Replace(html, " ");
        var clean = WhitespaceRegex.Replace(stripped, " ").Trim();
        if (clean.Length <= max)
        {
            return clean;
        }

        return clean.Substring(0, max) + "...";
    }
}
