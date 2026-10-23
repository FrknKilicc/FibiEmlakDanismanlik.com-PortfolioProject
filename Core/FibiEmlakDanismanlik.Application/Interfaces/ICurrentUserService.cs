using FibiEmlakDanismanlik.Domain.Enums;

namespace FibiEmlakDanismanlik.Application.Interfaces;

/// <summary>
/// Mevcut HTTP isteğini gerçekleştiren kullanıcının kimlik ve oturum bilgilerine erişim sağlayan servis arayüzü.
/// </summary>
public interface ICurrentUserService
{
    int? AgentId { get; }
    AgentRole? Role { get; }
    bool IsAuthenticated { get; }
    string? IpAddress { get; }
    string? UserAgent { get; }
}
