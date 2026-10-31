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
/// Blog kategorilerinin yönetimini (listeleme, ekleme, güncelleme, silme) sağlayan denetleyici.
/// </summary>
[Authorize]
public sealed class BlogCategoriesController : Controller
{
    private static readonly CultureInfo TrCulture = new("tr-TR");
    private readonly ApiClient _apiClient;
    private readonly CurrentAdmin _currentAdmin;

    public BlogCategoriesController(ApiClient apiClient, CurrentAdmin currentAdmin)
    {
        _apiClient = apiClient;
        _currentAdmin = currentAdmin;
    }

    /// <summary>
    /// Blog kategorileri liste ve yönetim sayfası.
    /// </summary>
    [HttpGet("/icerik/blog/kategoriler")]
    public async Task<IActionResult> Index()
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var result = await _apiClient.GetAsync<List<BlogCategoryDto>>("Blog/categories");

        if (result.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }
        if (result.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        var categories = (result.Data ?? new List<BlogCategoryDto>())
            .OrderBy(c => c.CategoryName, StringComparer.Create(TrCulture, true))
            .ToList();

        var vm = new BlogCategoriesViewModel
        {
            Categories = categories,
            ErrorMessage = result.Success ? null : (result.ErrorMessage ?? "Kategoriler alınırken bir hata oluştu.")
        };

        return View("Index", vm);
    }

    /// <summary>
    /// Yeni kategori ekler.
    /// </summary>
    [HttpPost("/icerik/blog/kategoriler")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BlogCategoryFormModel form)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var payload = new
        {
            categoryName = form.CategoryName ?? string.Empty
        };

        var postResult = await _apiClient.PostAsync<string>("Blog/categories", payload);

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
            TempData["Success"] = "Kategori eklendi.";
            return Redirect("/icerik/blog/kategoriler");
        }

        var listResult = await _apiClient.GetAsync<List<BlogCategoryDto>>("Blog/categories");
        if (listResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }
        if (listResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        var categories = (listResult.Data ?? new List<BlogCategoryDto>())
            .OrderBy(c => c.CategoryName, StringComparer.Create(TrCulture, true))
            .ToList();

        var fieldErrors = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
        if (postResult.Errors != null)
        {
            foreach (var kvp in postResult.Errors)
            {
                fieldErrors[$"new:{kvp.Key}"] = kvp.Value;
            }
        }

        var vm = new BlogCategoriesViewModel
        {
            Categories = categories,
            NewForm = form,
            FieldErrors = fieldErrors,
            ErrorMessage = postResult.ErrorMessage ?? "Lütfen formdaki hataları kontrol ediniz."
        };

        return View("Index", vm);
    }

    /// <summary>
    /// Mevcut kategoriyi günceller.
    /// </summary>
    [HttpPost("/icerik/blog/kategoriler/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BlogCategoryFormModel form)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var payload = new
        {
            blogCategoryId = id,
            categoryName = form.CategoryName ?? string.Empty
        };

        var putResult = await _apiClient.PutAsync<string>("Blog/categories", payload);

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
            TempData["Error"] = "Kayıt bulunamadı.";
            return Redirect("/icerik/blog/kategoriler");
        }

        if (putResult.Success)
        {
            TempData["Success"] = "Kategori güncellendi.";
            return Redirect("/icerik/blog/kategoriler");
        }

        var listResult = await _apiClient.GetAsync<List<BlogCategoryDto>>("Blog/categories");
        if (listResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }
        if (listResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        var categories = (listResult.Data ?? new List<BlogCategoryDto>())
            .OrderBy(c => c.CategoryName, StringComparer.Create(TrCulture, true))
            .ToList();

        var targetCategory = categories.FirstOrDefault(c => c.BlogCategoryId == id);
        if (targetCategory != null)
        {
            targetCategory.CategoryName = form.CategoryName;
        }

        var fieldErrors = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
        if (putResult.Errors != null)
        {
            foreach (var kvp in putResult.Errors)
            {
                fieldErrors[$"edit:{kvp.Key}"] = kvp.Value;
                fieldErrors[$"edit:{id}:{kvp.Key}"] = kvp.Value;
            }
        }

        var vm = new BlogCategoriesViewModel
        {
            Categories = categories,
            EditingId = id,
            FieldErrors = fieldErrors,
            ErrorMessage = putResult.ErrorMessage ?? "Lütfen formdaki hataları kontrol ediniz."
        };

        return View("Index", vm);
    }

    /// <summary>
    /// Kategoriyi siler.
    /// </summary>
    [HttpPost("/icerik/blog/kategoriler/{id:int}/sil")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var result = await _apiClient.DeleteAsync<string>($"Blog/categories?id={id}");

        if (result.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }
        if (result.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        if (result.StatusCode == StatusCodes.Status404NotFound)
        {
            TempData["Error"] = "Kayıt bulunamadı.";
            return Redirect("/icerik/blog/kategoriler");
        }

        if (result.StatusCode == StatusCodes.Status409Conflict)
        {
            TempData["Error"] = result.ErrorMessage ?? "Bu kategoride yazılar bulunduğu için silinemez.";
            return Redirect("/icerik/blog/kategoriler");
        }

        if (result.Success)
        {
            TempData["Success"] = "Kategori silindi.";
            return Redirect("/icerik/blog/kategoriler");
        }

        TempData["Error"] = result.ErrorMessage ?? "Kategori silinirken bir hata oluştu.";
        return Redirect("/icerik/blog/kategoriler");
    }
}
