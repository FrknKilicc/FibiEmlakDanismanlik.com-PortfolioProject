#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FibiEmlakDanismanlik.AdminUI.Infrastructure;
using FibiEmlakDanismanlik.AdminUI.Models.Content;

namespace FibiEmlakDanismanlik.AdminUI.Controllers;

/// <summary>
/// Blog içerik yönetimi (liste, yeni, düzenle, sil) süreçlerini yöneten denetleyici.
/// </summary>
[Authorize]
public sealed class BlogAdminController : Controller
{
    private const int PageSize = 20;
    private static readonly CultureInfo TrCulture = new("tr-TR");
    private readonly ApiClient _apiClient;
    private readonly CurrentAdmin _currentAdmin;

    public BlogAdminController(ApiClient apiClient, CurrentAdmin currentAdmin)
    {
        _apiClient = apiClient;
        _currentAdmin = currentAdmin;
    }

    /// <summary>
    /// Blog yazıları listesi sayfası.
    /// </summary>
    [HttpGet("/icerik/blog")]
    public async Task<IActionResult> Index(string? ara, int? kategori, int sayfa = 1)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var blogsTask = _apiClient.GetAsync<List<BlogDto>>("Blog");
        var categoriesTask = _apiClient.GetAsync<List<BlogCategoryDto>>("Blog/categories");
        var authorsTask = _apiClient.GetAsync<List<AuthorDto>>("Author");

        await Task.WhenAll(blogsTask, categoriesTask, authorsTask);

        var blogsResult = await blogsTask;
        var categoriesResult = await categoriesTask;
        var authorsResult = await authorsTask;

        if (blogsResult.StatusCode == StatusCodes.Status401Unauthorized ||
            categoriesResult.StatusCode == StatusCodes.Status401Unauthorized ||
            authorsResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }

        if (blogsResult.StatusCode == StatusCodes.Status403Forbidden ||
            categoriesResult.StatusCode == StatusCodes.Status403Forbidden ||
            authorsResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        var blogs = blogsResult.Data ?? new List<BlogDto>();

        // Arama filtresi: başlıkta tr-TR büyük-küçük harf duyarsız
        if (!string.IsNullOrWhiteSpace(ara))
        {
            var search = ara.Trim();
            blogs = blogs.Where(b =>
                !string.IsNullOrEmpty(b.BlogTitle) &&
                TrCulture.CompareInfo.IndexOf(b.BlogTitle, search, CompareOptions.IgnoreCase) >= 0
            ).ToList();
        }

        // Kategori filtresi
        if (kategori.HasValue)
        {
            blogs = blogs.Where(b => b.BlogCategoryId == kategori.Value).ToList();
        }

        // Sıralama: tarihe göre azalan
        blogs = blogs.OrderByDescending(b => b.CreatedDate).ToList();

        // Sayfalama (sayfa başına 20)
        int totalCount = blogs.Count;
        int totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
        if (totalPages < 1)
        {
            totalPages = 1;
        }

        int currentPage = sayfa < 1 ? 1 : sayfa;
        if (currentPage > totalPages)
        {
            currentPage = totalPages;
        }

        var pagedBlogs = blogs
            .Skip((currentPage - 1) * PageSize)
            .Take(PageSize)
            .ToList();

        var vm = new BlogIndexViewModel
        {
            Blogs = pagedBlogs,
            Categories = categoriesResult.Data ?? new List<BlogCategoryDto>(),
            Authors = authorsResult.Data ?? new List<AuthorDto>(),
            Ara = ara,
            Kategori = kategori,
            Sayfa = currentPage,
            TotalPages = totalPages,
            ErrorMessage = blogsResult.Success ? null : (blogsResult.ErrorMessage ?? "Blog yazıları alınırken bir hata oluştu.")
        };

        return View("Index", vm);
    }

    /// <summary>
    /// Yeni blog yazısı ekleme sayfasını görüntüler.
    /// </summary>
    [HttpGet("/icerik/blog/yeni")]
    public async Task<IActionResult> Create()
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var (categories, authors, tags, authRedirect) = await LoadFormDataAsync();
        if (authRedirect == StatusCodes.Status401Unauthorized) return Redirect("/giris");
        if (authRedirect == StatusCodes.Status403Forbidden) return Redirect("/yetkisiz");

        int defaultAuthorId = authors.FirstOrDefault()?.AuthorId ?? 0;

        var vm = new BlogFormViewModel
        {
            Form = new BlogFormModel
            {
                CreatedDate = DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                AuthorId = defaultAuthorId,
                TopNews = false
            },
            Categories = categories,
            Authors = authors,
            AllTags = tags,
            IsNew = true
        };

        return View("Form", vm);
    }

    /// <summary>
    /// Yeni blog yazısını kaydeder.
    /// </summary>
    [HttpPost("/icerik/blog/yeni")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BlogFormModel form)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var (categories, authors, tags, authRedirect) = await LoadFormDataAsync();
        if (authRedirect == StatusCodes.Status401Unauthorized) return Redirect("/giris");
        if (authRedirect == StatusCodes.Status403Forbidden) return Redirect("/yetkisiz");

        // CreatedDate format doğrulaması (yyyy-MM-dd)
        bool dateParsed = DateTime.TryParseExact(
            form.CreatedDate?.Trim(),
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var createdDate);

        if (!dateParsed)
        {
            var vmDateError = new BlogFormViewModel
            {
                Form = form,
                Categories = categories,
                Authors = authors,
                AllTags = tags,
                IsNew = true,
                FieldErrors = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["CreatedDate"] = new[] { "Lütfen geçerli bir tarih seçiniz (yyyy-MM-dd)." }
                },
                ErrorMessage = "Lütfen formdaki hataları kontrol ediniz."
            };
            return View("Form", vmDateError);
        }

        var payload = new
        {
            blogTitle = form.BlogTitle ?? string.Empty,
            blogDescription = form.BlogDescription ?? string.Empty,
            blogImgUrl = form.BlogImgUrl ?? string.Empty,
            createdDate = createdDate,
            topNews = form.TopNews,
            authorId = form.AuthorId,
            blogCategoryId = form.BlogCategoryId,
            tags = ParseTags(form.TagsText)
        };

        var postResult = await _apiClient.PostAsync<string>("Blog", payload);

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
            TempData["Success"] = "Blog yazısı eklendi. Sitede en geç 5 dakika içinde görünür.";
            return Redirect("/icerik/blog");
        }

        var vm = new BlogFormViewModel
        {
            Form = form,
            Categories = categories,
            Authors = authors,
            AllTags = tags,
            IsNew = true,
            FieldErrors = postResult.Errors != null
                ? new Dictionary<string, string[]>(postResult.Errors, StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase),
            ErrorMessage = postResult.ErrorMessage ?? "Lütfen formdaki hataları kontrol ediniz."
        };

        return View("Form", vm);
    }

    /// <summary>
    /// Blog yazısı düzenleme sayfasını görüntüler.
    /// </summary>
    [HttpGet("/icerik/blog/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var blogTask = _apiClient.GetAsync<BlogDto>($"Blog/{id}");
        var categoriesTask = _apiClient.GetAsync<List<BlogCategoryDto>>("Blog/categories");
        var authorsTask = _apiClient.GetAsync<List<AuthorDto>>("Author");
        var tagsTask = _apiClient.GetAsync<List<string>>("Blog/tags");

        await Task.WhenAll(blogTask, categoriesTask, authorsTask, tagsTask);

        var blogResult = await blogTask;
        var categoriesResult = await categoriesTask;
        var authorsResult = await authorsTask;
        var tagsResult = await tagsTask;

        if (blogResult.StatusCode == StatusCodes.Status401Unauthorized ||
            categoriesResult.StatusCode == StatusCodes.Status401Unauthorized ||
            authorsResult.StatusCode == StatusCodes.Status401Unauthorized ||
            tagsResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }

        if (blogResult.StatusCode == StatusCodes.Status403Forbidden ||
            categoriesResult.StatusCode == StatusCodes.Status403Forbidden ||
            authorsResult.StatusCode == StatusCodes.Status403Forbidden ||
            tagsResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        if (blogResult.StatusCode == StatusCodes.Status404NotFound || blogResult.Data == null)
        {
            return NotFound();
        }

        var existing = blogResult.Data;
        var form = new BlogFormModel
        {
            BlogId = existing.BlogId,
            BlogTitle = existing.BlogTitle,
            BlogDescription = existing.BlogDescription,
            BlogImgUrl = existing.BlogImgUrl,
            CreatedDate = existing.CreatedDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            TopNews = existing.TopNews,
            AuthorId = existing.AuthorId,
            BlogCategoryId = existing.BlogCategoryId,
            TagsText = existing.Tags != null && existing.Tags.Any() ? string.Join(", ", existing.Tags) : string.Empty
        };

        var allTags = tagsResult.Success && tagsResult.Data != null ? tagsResult.Data : new List<string>();

        var vm = new BlogFormViewModel
        {
            Form = form,
            Categories = categoriesResult.Data ?? new List<BlogCategoryDto>(),
            Authors = authorsResult.Data ?? new List<AuthorDto>(),
            AllTags = allTags,
            IsNew = false
        };

        return View("Form", vm);
    }

    /// <summary>
    /// Blog yazısını günceller.
    /// </summary>
    [HttpPost("/icerik/blog/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BlogFormModel form)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var (categories, authors, tags, authRedirect) = await LoadFormDataAsync();
        if (authRedirect == StatusCodes.Status401Unauthorized) return Redirect("/giris");
        if (authRedirect == StatusCodes.Status403Forbidden) return Redirect("/yetkisiz");

        form.BlogId = id;

        // CreatedDate format doğrulaması (yyyy-MM-dd)
        bool dateParsed = DateTime.TryParseExact(
            form.CreatedDate?.Trim(),
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var createdDate);

        if (!dateParsed)
        {
            var vmDateError = new BlogFormViewModel
            {
                Form = form,
                Categories = categories,
                Authors = authors,
                AllTags = tags,
                IsNew = false,
                FieldErrors = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["CreatedDate"] = new[] { "Lütfen geçerli bir tarih seçiniz (yyyy-MM-dd)." }
                },
                ErrorMessage = "Lütfen formdaki hataları kontrol ediniz."
            };
            return View("Form", vmDateError);
        }

        var payload = new
        {
            blogId = id,
            blogTitle = form.BlogTitle ?? string.Empty,
            blogDescription = form.BlogDescription ?? string.Empty,
            blogImgUrl = form.BlogImgUrl ?? string.Empty,
            createdDate = createdDate,
            topNews = form.TopNews,
            authorId = form.AuthorId,
            blogCategoryId = form.BlogCategoryId,
            tags = ParseTags(form.TagsText)
        };

        var putResult = await _apiClient.PutAsync<string>("Blog", payload);

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
            TempData["Success"] = "Blog yazısı güncellendi.";
            return Redirect($"/icerik/blog/{id}");
        }

        var vm = new BlogFormViewModel
        {
            Form = form,
            Categories = categories,
            Authors = authors,
            AllTags = tags,
            IsNew = false,
            FieldErrors = putResult.Errors != null
                ? new Dictionary<string, string[]>(putResult.Errors, StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase),
            ErrorMessage = putResult.ErrorMessage ?? "Lütfen formdaki hataları kontrol ediniz."
        };

        return View("Form", vm);
    }

    /// <summary>
    /// Blog yazısını siler.
    /// </summary>
    [HttpPost("/icerik/blog/{id:int}/sil")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var result = await _apiClient.DeleteAsync<string>($"Blog?id={id}");

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
            TempData["Success"] = "Blog yazısı silindi.";
            return Redirect("/icerik/blog");
        }

        if (result.StatusCode == StatusCodes.Status404NotFound)
        {
            TempData["Error"] = "Kayıt bulunamadı.";
            return Redirect("/icerik/blog");
        }

        TempData["Error"] = result.ErrorMessage ?? "Blog yazısı silinirken bir hata oluştu.";
        return Redirect($"/icerik/blog/{id}");
    }

    private static List<string> ParseTags(string? tagsText)
    {
        if (string.IsNullOrWhiteSpace(tagsText))
        {
            return new List<string>();
        }

        return tagsText
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim())
            .Where(t => !string.IsNullOrEmpty(t))
            .Distinct(StringComparer.Create(TrCulture, true))
            .ToList();
    }

    private async Task<(List<BlogCategoryDto> Categories, List<AuthorDto> Authors, List<string> Tags, int? AuthRedirectStatusCode)> LoadFormDataAsync()
    {
        var categoriesTask = _apiClient.GetAsync<List<BlogCategoryDto>>("Blog/categories");
        var authorsTask = _apiClient.GetAsync<List<AuthorDto>>("Author");
        var tagsTask = _apiClient.GetAsync<List<string>>("Blog/tags");

        await Task.WhenAll(categoriesTask, authorsTask, tagsTask);

        var categoriesResult = await categoriesTask;
        var authorsResult = await authorsTask;
        var tagsResult = await tagsTask;

        if (categoriesResult.StatusCode == StatusCodes.Status401Unauthorized ||
            authorsResult.StatusCode == StatusCodes.Status401Unauthorized ||
            tagsResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return (new List<BlogCategoryDto>(), new List<AuthorDto>(), new List<string>(), StatusCodes.Status401Unauthorized);
        }

        if (categoriesResult.StatusCode == StatusCodes.Status403Forbidden ||
            authorsResult.StatusCode == StatusCodes.Status403Forbidden ||
            tagsResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            return (new List<BlogCategoryDto>(), new List<AuthorDto>(), new List<string>(), StatusCodes.Status403Forbidden);
        }

        var tags = tagsResult.Success && tagsResult.Data != null ? tagsResult.Data : new List<string>();

        return (
            categoriesResult.Data ?? new List<BlogCategoryDto>(),
            authorsResult.Data ?? new List<AuthorDto>(),
            tags,
            null
        );
    }
}
