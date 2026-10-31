using System.Security.Claims;

namespace FibiEmlakDanismanlik.AdminUI.Infrastructure;

/// <summary>
/// Geçerli HTTP isteğindeki oturum açmış yöneticiye ait bilgileri sağlayan servis.
/// </summary>
public sealed class CurrentAdmin
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentAdmin(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    /// <summary>
    /// Danışman / Yönetici tekil kimliği (NameIdentifier).
    /// </summary>
    public int? AgentId
    {
        get
        {
            var claimValue = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claimValue, out var id) ? id : null;
        }
    }

    /// <summary>
    /// Yöneticinin adı soyadı.
    /// </summary>
    public string Name => User?.Identity?.Name
        ?? User?.FindFirst(ClaimTypes.Name)?.Value
        ?? string.Empty;

    /// <summary>
    /// Yöneticinin sistemdeki rolü ("Owner", "Manager", "Consultant").
    /// </summary>
    public string Role => User?.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

    /// <summary>
    /// Arayüzde görüntülenecek Türkçe rol unvanı.
    /// </summary>
    public string RoleDisplayName => Role switch
    {
        "Owner" => "Sahip",
        "Manager" => "Yönetici",
        "Consultant" => "Danışman",
        _ => Role
    };

    /// <summary>
    /// Yöneticinin Sahip (Owner) yetkisine sahip olup olmadığı.
    /// </summary>
    public bool IsOwner => string.Equals(Role, "Owner", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Yöneticinin Sahip (Owner) veya Yönetici (Manager) yetkisine sahip olup olmadığı.
    /// </summary>
    public bool IsManagerOrAbove => IsOwner || string.Equals(Role, "Manager", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// WebApi çağrılarında kullanılacak JWT erişim belirteci (access token).
    /// </summary>
    public string? AccessToken => User?.FindFirst(AdminClaimTypes.AccessToken)?.Value;
}
