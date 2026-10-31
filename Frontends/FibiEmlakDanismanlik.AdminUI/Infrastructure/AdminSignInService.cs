using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace FibiEmlakDanismanlik.AdminUI.Infrastructure;

/// <summary>
/// Başarılı kimlik doğrulama sonrasında yönetici oturumunu başlatan ve kimlik talep (claim) çerezini oluşturan servis.
/// </summary>
public sealed class AdminSignInService
{
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    /// <summary>
    /// Verilen JWT belirteci ve kullanıcı bilgileriyle güvenli yönetici oturumu açar.
    /// </summary>
    /// <param name="httpContext">Geçerli HTTP bağlamı.</param>
    /// <param name="token">WebApi tarafından üretilen JWT erişim belirteci.</param>
    /// <param name="expireDateUtc">Belirtecin son geçerlilik tarihi (UTC).</param>
    /// <param name="agentName">Yöneticinin adı soyadı.</param>
    /// <param name="role">Yöneticinin sistemdeki rolü.</param>
    public async Task SignInAsync(HttpContext httpContext, string token, DateTime expireDateUtc, string agentName, string role)
    {
        if (httpContext == null)
        {
            throw new ArgumentNullException(nameof(httpContext));
        }

        string nameIdentifier = string.Empty;
        if (!string.IsNullOrWhiteSpace(token))
        {
            try
            {
                var jwt = _tokenHandler.ReadJwtToken(token);
                nameIdentifier = jwt.Claims.FirstOrDefault(c =>
                    c.Type == ClaimTypes.NameIdentifier ||
                    c.Type == "nameid" ||
                    c.Type == "sub")?.Value ?? string.Empty;
            }
            catch
            {
                // Token ayrıştırılamazsa hata fırlatma; nameIdentifier boş kalır ve fallback kullanılır
            }
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, !string.IsNullOrWhiteSpace(nameIdentifier) ? nameIdentifier : agentName),
            new(ClaimTypes.Name, agentName),
            new(ClaimTypes.Role, role),
            new(AdminClaimTypes.AccessToken, token),
            new(AdminClaimTypes.AccessTokenExpiresUtc, expireDateUtc.ToString("o", CultureInfo.InvariantCulture)),
            new(AdminClaimTypes.IssuedAtUtc, DateTimeOffset.UtcNow.ToString("o", CultureInfo.InvariantCulture))
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        var properties = new AuthenticationProperties
        {
            IsPersistent = false,
            AllowRefresh = true
        };

        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
    }
}
