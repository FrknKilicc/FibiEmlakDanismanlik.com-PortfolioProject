using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.AdminUI.Controllers;

/// <summary>
/// Hata ve yetkisiz erişim sayfalarını sunan denetleyici.
/// </summary>
[AllowAnonymous]
public sealed class ErrorController : Controller
{
    [Route("hata/{code:int?}")]
    public IActionResult Index(int? code)
    {
        var statusCode = code ?? HttpContext.Response.StatusCode;
        if (statusCode == StatusCodes.Status200OK)
        {
            statusCode = StatusCodes.Status500InternalServerError;
        }

        Response.StatusCode = statusCode;
        return View("Index", statusCode);
    }

    [Route("yetkisiz")]
    public IActionResult UnauthorizedPage()
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        return View("Index", StatusCodes.Status403Forbidden);
    }
}
