using FibiEmlakDanismanlik.Domain.Enums;

namespace FibiEmlakDanismanlik.Application.Constants;

/// <summary>
/// Kimlik doğrulama ve yetkilendirme süreçlerinde kullanılan sabit değerler ve önbellek anahtarları.
/// </summary>
public static class AuthConstants
{
    // Politika adları
    public const string PolicyOwnerOnly = "OwnerOnly";
    public const string PolicyManagerOrAbove = "ManagerOrAbove";
    public const string PolicyStaff = "Staff";

    // Rol adları (JWT rol claim değeri = enum adı)
    public static readonly string RoleOwner = nameof(AgentRole.Owner);
    public static readonly string RoleManager = nameof(AgentRole.Manager);
    public static readonly string RoleConsultant = nameof(AgentRole.Consultant);

    // Güvenlik damgası claim tipi
    public const string SecurityStampClaimType = "sstamp";

    // Oturum durumu önbellek süresi (saniye)
    public const int AuthStateCacheSeconds = 30;

    /// <summary>
    /// Danışmanın oturum durumu önbellek anahtarını üretir.
    /// </summary>
    public static string AuthStateCacheKey(int agentId) => $"auth-state:{agentId}";
}
