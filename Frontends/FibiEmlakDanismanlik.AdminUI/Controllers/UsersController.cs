using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FibiEmlakDanismanlik.AdminUI.Infrastructure;
using FibiEmlakDanismanlik.AdminUI.Models.Users;

namespace FibiEmlakDanismanlik.AdminUI.Controllers;

/// <summary>
/// Kullanıcı yönetimi (Danışman, Yönetici, Sahip) süreçlerini yöneten denetleyici.
/// </summary>
[Authorize]
public sealed class UsersController : Controller
{
    private readonly ApiClient _apiClient;
    private readonly CurrentAdmin _currentAdmin;

    public UsersController(ApiClient apiClient, CurrentAdmin currentAdmin)
    {
        _apiClient = apiClient;
        _currentAdmin = currentAdmin;
    }

    /// <summary>
    /// Kullanıcılar listesi sayfası.
    /// </summary>
    [HttpGet("/kullanicilar")]
    public async Task<IActionResult> Index(string? ara, string? rol, bool pasif = false)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var result = await _apiClient.GetAsync<List<AgentAdminDto>>("Agent/admin");

        if (result.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }

        if (result.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        var users = result.Data ?? new List<AgentAdminDto>();
        var trCulture = new CultureInfo("tr-TR");

        // Arama filtresi: ad, e-posta, telefon
        if (!string.IsNullOrWhiteSpace(ara))
        {
            var search = ara.Trim();
            users = users.Where(u =>
                (u.AgentName != null && trCulture.CompareInfo.IndexOf(u.AgentName, search, CompareOptions.IgnoreCase) >= 0) ||
                (u.Mail != null && trCulture.CompareInfo.IndexOf(u.Mail, search, CompareOptions.IgnoreCase) >= 0) ||
                (u.AgentPhoneNumber != null && trCulture.CompareInfo.IndexOf(u.AgentPhoneNumber, search, CompareOptions.IgnoreCase) >= 0)
            ).ToList();
        }

        // Rol filtresi: Owner, Manager, Consultant
        if (!string.IsNullOrWhiteSpace(rol))
        {
            var targetRole = rol.Trim();
            users = users.Where(u => string.Equals(u.Role, targetRole, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        // Pasif filtresi: pasif false ise yalnızca aktifler
        if (!pasif)
        {
            users = users.Where(u => u.IsActive).ToList();
        }

        // Sıralama: rol (Sahip, Yönetici, Danışman), sonra ad
        static int RoleOrder(string? r) => r switch
        {
            "Owner" => 1,
            "Manager" => 2,
            "Consultant" => 3,
            _ => 4
        };

        users = users
            .OrderBy(u => RoleOrder(u.Role))
            .ThenBy(u => u.AgentName, StringComparer.Create(trCulture, true))
            .ToList();

        var vm = new UserIndexViewModel
        {
            Users = users,
            Ara = ara,
            Rol = rol,
            PasifleriGoster = pasif,
            IsOwner = _currentAdmin.IsOwner,
            CurrentAgentId = _currentAdmin.AgentId,
            ErrorMessage = result.Success ? null : (result.ErrorMessage ?? "Kullanıcılar listelenirken bir hata oluştu.")
        };

        return View("Index", vm);
    }

    /// <summary>
    /// Yeni kullanıcı ekleme formunu görüntüler.
    /// </summary>
    [HttpGet("/kullanicilar/yeni")]
    public IActionResult Create()
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var vm = new UserFormViewModel
        {
            Form = new UserFormModel
            {
                Role = 3,
                IsActive = true,
                ShowOnWebsite = true
            },
            IsNew = true,
            IsOwner = _currentAdmin.IsOwner,
            IsSelf = false
        };

        return View("Form", vm);
    }

    /// <summary>
    /// Yeni kullanıcıyı kaydeder.
    /// </summary>
    [HttpPost("/kullanicilar/yeni")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserFormModel form)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        // Yönetici yalnızca Danışman (Role = 3) ekleyebilir
        if (!_currentAdmin.IsOwner)
        {
            form.Role = 3;
        }

        var payload = new
        {
            agentName = form.AgentName,
            agentTitle = form.AgentTitle,
            agentDescription = form.AgentDescription,
            agentImgUrl = form.AgentImgUrl,
            mail = form.Mail,
            agentPhoneNumber = form.AgentPhoneNumber,
            password = form.Password,
            role = form.Role,
            showOnWebsite = form.ShowOnWebsite,
            canViewAllListings = form.CanViewAllListings
        };

        var postResult = await _apiClient.PostAsync<string>("Agent", payload);

        if (postResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }

        if (postResult.Success)
        {
            TempData["Success"] = "Kullanıcı eklendi. Geçici şifreyi kişiye güvenli bir yoldan iletin; ilk girişte şifresini değiştirip iki adımlı doğrulamayı kuracak.";
            return Redirect("/kullanicilar");
        }

        // Hata durumunda formu yeniden göster (şifre hariç değerler korunur)
        form.Password = null;

        var vm = new UserFormViewModel
        {
            Form = form,
            IsNew = true,
            IsOwner = _currentAdmin.IsOwner,
            IsSelf = false,
            FieldErrors = postResult.Errors != null
                ? new Dictionary<string, string[]>(postResult.Errors, StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        };

        if (postResult.StatusCode == StatusCodes.Status409Conflict)
        {
            var msg = postResult.ErrorMessage ?? "Bu e-posta adresi zaten kullanımda.";
            vm.FieldErrors["Mail"] = new[] { msg };
            vm.ErrorMessage = msg;
        }
        else if (postResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            vm.ErrorMessage = postResult.ErrorMessage ?? "Bu işlem için yetkiniz bulunmuyor.";
        }
        else if (postResult.StatusCode == StatusCodes.Status400BadRequest)
        {
            vm.ErrorMessage = postResult.ErrorMessage ?? "Lütfen formdaki hataları kontrol ediniz.";
        }
        else
        {
            vm.ErrorMessage = postResult.ErrorMessage ?? "Kullanıcı eklenirken bir hata oluştu.";
        }

        return View("Form", vm);
    }

    /// <summary>
    /// Kullanıcı düzenleme sayfasını görüntüler.
    /// </summary>
    [HttpGet("/kullanicilar/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var adminListTask = _apiClient.GetAsync<List<AgentAdminDto>>("Agent/admin");
        var publicDetailTask = _apiClient.GetAsync<AgentPublicDto>($"Agent/{id}");

        await Task.WhenAll(adminListTask, publicDetailTask);

        var adminListResult = await adminListTask;
        var publicDetailResult = await publicDetailTask;

        if (adminListResult.StatusCode == StatusCodes.Status401Unauthorized ||
            publicDetailResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }

        if (adminListResult.StatusCode == StatusCodes.Status403Forbidden ||
            publicDetailResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        var existing = adminListResult.Data?.FirstOrDefault(a => a.AgentId == id);
        if (existing == null)
        {
            return NotFound();
        }

        string? description = publicDetailResult.Success ? publicDetailResult.Data?.AgentDescription : null;

        var form = new UserFormModel
        {
            AgentId = existing.AgentId,
            AgentName = existing.AgentName,
            AgentTitle = existing.AgentTitle,
            AgentDescription = description,
            AgentImgUrl = existing.AgentImgUrl,
            Mail = existing.Mail,
            AgentPhoneNumber = existing.AgentPhoneNumber,
            Password = null,
            Role = UserLabels.RoleValue(existing.Role),
            IsActive = existing.IsActive,
            ShowOnWebsite = existing.ShowOnWebsite,
            CanViewAllListings = existing.CanViewAllListings,
            ResetTwoFactor = false,
            Unlock = false
        };

        var isSelf = _currentAdmin.AgentId.HasValue && _currentAdmin.AgentId.Value == id;
        var vm = new UserFormViewModel
        {
            Form = form,
            Existing = existing,
            IsNew = false,
            IsOwner = _currentAdmin.IsOwner,
            IsSelf = isSelf
        };

        return View("Form", vm);
    }

    /// <summary>
    /// Kullanıcı bilgilerini günceller.
    /// </summary>
    [HttpPost("/kullanicilar/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UserFormModel form)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var isSelf = _currentAdmin.AgentId.HasValue && _currentAdmin.AgentId.Value == id;

        // Yönetici yalnızca Danışman rollerini güncelleyebilir
        if (!_currentAdmin.IsOwner && !isSelf)
        {
            form.Role = 3;
        }

        var payload = new
        {
            agentId = id,
            agentName = form.AgentName,
            agentTitle = form.AgentTitle,
            agentDescription = form.AgentDescription,
            agentImgUrl = form.AgentImgUrl,
            mail = form.Mail,
            agentPhoneNumber = form.AgentPhoneNumber,
            password = string.IsNullOrWhiteSpace(form.Password) ? null : form.Password,
            role = form.Role,
            isActive = form.IsActive,
            showOnWebsite = form.ShowOnWebsite,
            canViewAllListings = form.CanViewAllListings,
            resetTwoFactor = form.ResetTwoFactor,
            unlock = form.Unlock
        };

        var putResult = await _apiClient.PutAsync<string>("Agent", payload);

        if (putResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }

        if (putResult.StatusCode == StatusCodes.Status404NotFound)
        {
            return NotFound();
        }

        if (putResult.Success)
        {
            TempData["Success"] = "Kullanıcı güncellendi.";
            return Redirect($"/kullanicilar/{id}");
        }

        // Hatalarda formu yeniden göster (Existing'i yeniden yükle)
        var adminListResult = await _apiClient.GetAsync<List<AgentAdminDto>>("Agent/admin");
        var existing = adminListResult.Data?.FirstOrDefault(a => a.AgentId == id);

        form.Password = null;

        var vm = new UserFormViewModel
        {
            Form = form,
            Existing = existing,
            IsNew = false,
            IsOwner = _currentAdmin.IsOwner,
            IsSelf = isSelf,
            FieldErrors = putResult.Errors != null
                ? new Dictionary<string, string[]>(putResult.Errors, StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        };

        if (putResult.StatusCode == StatusCodes.Status409Conflict)
        {
            var msg = putResult.ErrorMessage ?? "Bu e-posta adresi zaten kullanımda.";
            vm.FieldErrors["Mail"] = new[] { msg };
            vm.ErrorMessage = msg;
        }
        else if (putResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            vm.ErrorMessage = putResult.ErrorMessage ?? "Bu işlem için yetkiniz bulunmuyor.";
        }
        else if (putResult.StatusCode == StatusCodes.Status400BadRequest)
        {
            vm.ErrorMessage = putResult.ErrorMessage ?? "Lütfen formdaki hataları kontrol ediniz.";
        }
        else
        {
            vm.ErrorMessage = putResult.ErrorMessage ?? "Kullanıcı güncellenirken bir hata oluştu.";
        }

        return View("Form", vm);
    }
}
