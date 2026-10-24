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
/// Blog yazarlarının yönetimini (listeleme, ekleme, güncelleme, silme) sağlayan denetleyici.
/// </summary>
[Authorize]
public sealed class BlogAuthorsController : Controller
{
    private static readonly CultureInfo TrCulture = new("tr-TR");
    private readonly ApiClient _apiClient;
    private readonly CurrentAdmin _currentAdmin;

    public BlogAuthorsController(ApiClient apiClient, CurrentAdmin currentAdmin)
    {
        _apiClient = apiClient;
        _currentAdmin = currentAdmin;
    }

    /// <summary>
    /// Blog yazarları liste ve yönetim sayfası.
    /// </summary>
    [HttpGet("/icerik/blog/yazarlar")]
    public async Task<IActionResult> Index()
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var result = await _apiClient.GetAsync<List<AuthorDto>>("Author");

        if (result.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }
        if (result.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        var authors = (result.Data ?? new List<AuthorDto>())
            .OrderBy(a => a.AuthorName, StringComparer.Create(TrCulture, true))
            .ToList();

        var vm = new BlogAuthorsViewModel
        {
            Authors = authors,
            ErrorMessage = result.Success ? null : (result.ErrorMessage ?? "Yazarlar alınırken bir hata oluştu.")
        };

        return View("Index", vm);
    }

    /// <summary>
    /// Yeni yazar ekler.
    /// </summary>
    [HttpPost("/icerik/blog/yazarlar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AuthorFormModel form)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var payload = new
        {
            authorName = form.AuthorName ?? string.Empty,
            authorImgUrl = form.AuthorImgUrl ?? string.Empty
        };

        var postResult = await _apiClient.PostAsync<string>("Author", payload);

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
            TempData["Success"] = "Yazar eklendi.";
            return Redirect("/icerik/blog/yazarlar");
        }

        var listResult = await _apiClient.GetAsync<List<AuthorDto>>("Author");
        if (listResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }
        if (listResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        var authors = (listResult.Data ?? new List<AuthorDto>())
            .OrderBy(a => a.AuthorName, StringComparer.Create(TrCulture, true))
            .ToList();

        var fieldErrors = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
        if (postResult.Errors != null)
        {
            foreach (var kvp in postResult.Errors)
            {
                fieldErrors[$"new:{kvp.Key}"] = kvp.Value;
            }
        }

        var vm = new BlogAuthorsViewModel
        {
            Authors = authors,
            NewForm = form,
            FieldErrors = fieldErrors,
            ErrorMessage = postResult.ErrorMessage ?? "Lütfen formdaki hataları kontrol ediniz."
        };

        return View("Index", vm);
    }

    /// <summary>
    /// Mevcut yazarı günceller.
    /// </summary>
    [HttpPost("/icerik/blog/yazarlar/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AuthorFormModel form)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var payload = new
        {
            authorId = id,
            authorName = form.AuthorName ?? string.Empty,
            authorImgUrl = form.AuthorImgUrl ?? string.Empty
        };

        var putResult = await _apiClient.PutAsync<string>("Author", payload);

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
            return Redirect("/icerik/blog/yazarlar");
        }

        if (putResult.Success)
        {
            TempData["Success"] = "Yazar güncellendi.";
            return Redirect("/icerik/blog/yazarlar");
        }

        var listResult = await _apiClient.GetAsync<List<AuthorDto>>("Author");
        if (listResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }
        if (listResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        var authors = (listResult.Data ?? new List<AuthorDto>())
            .OrderBy(a => a.AuthorName, StringComparer.Create(TrCulture, true))
            .ToList();

        var targetAuthor = authors.FirstOrDefault(a => a.AuthorId == id);
        if (targetAuthor != null)
        {
            targetAuthor.AuthorName = form.AuthorName;
            targetAuthor.AuthorImgUrl = form.AuthorImgUrl;
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

        var vm = new BlogAuthorsViewModel
        {
            Authors = authors,
            EditingId = id,
            FieldErrors = fieldErrors,
            ErrorMessage = putResult.ErrorMessage ?? "Lütfen formdaki hataları kontrol ediniz."
        };

        return View("Index", vm);
    }

    /// <summary>
    /// Yazarı siler.
    /// </summary>
    [HttpPost("/icerik/blog/yazarlar/{id:int}/sil")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var result = await _apiClient.DeleteAsync<string>($"Author?id={id}");

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
            return Redirect("/icerik/blog/yazarlar");
        }

        if (result.StatusCode == StatusCodes.Status409Conflict)
        {
            TempData["Error"] = result.ErrorMessage ?? "Bu yazarın yazıları bulunduğu için silinemez.";
            return Redirect("/icerik/blog/yazarlar");
        }

        if (result.Success)
        {
            TempData["Success"] = "Yazar silindi.";
            return Redirect("/icerik/blog/yazarlar");
        }

        TempData["Error"] = result.ErrorMessage ?? "Yazar silinirken bir hata oluştu.";
        return Redirect("/icerik/blog/yazarlar");
    }
}
