using System.Security.Claims;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace FibiEmlakDanismanlik.WebApi.Services;

/// <summary>
/// Geçerli HTTP isteğindeki kullanıcı bilgilerini ve oturum detaylarını sağlayan servis uygulaması.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? AgentId
    {
        get
        {
            var claimValue = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claimValue, out var agentId) ? agentId : null;
        }
    }

    public AgentRole? Role
    {
        get
        {
            var roleClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
            return Enum.TryParse<AgentRole>(roleClaim, true, out var role) ? role : null;
        }
    }

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public string? IpAddress => _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

    public string? UserAgent
    {
        get
        {
            var userAgent = _httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString();
            if (string.IsNullOrEmpty(userAgent))
            {
                return null;
            }

            return userAgent.Length > 512 ? userAgent[..512] : userAgent;
        }
    }
}
