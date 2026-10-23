using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.AdminUI.Controllers;

/// <summary>
/// Yönetim paneli ana kontrol paneli (Dashboard) ekranını sunan denetleyici.
/// </summary>
public sealed class DashboardController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}
