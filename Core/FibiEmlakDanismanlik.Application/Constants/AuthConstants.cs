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

    // Giriş güvenlik ve kısıtlama sabitleri
    public const int MaxFailedLoginAttempts = 5;
    public const int LockoutMinutes = 15;
    public const int ChallengeTokenMinutes = 10;
    public const int MinPasswordLength = 12;

    // Challenge token doğrulaması için audience ve claim tipi
    public const string ChallengeAudience = "FibiEmlakDanismanlik.Challenge";
    public const string ChallengePurposeClaimType = "purpose";

    // Challenge amaçları
    public const string PurposePasswordChange = "pwd";
    public const string PurposeTwoFactor = "2fa";
    public const string PurposeTwoFactorSetup = "2fa-setup";

    // Giriş sonuç durumları
    public const string LoginStatusSuccess = "Success";
    public const string LoginStatusPasswordChangeRequired = "PasswordChangeRequired";
    public const string LoginStatusTwoFactorRequired = "TwoFactorRequired";
    public const string LoginStatusTwoFactorSetupRequired = "TwoFactorSetupRequired";

    // Kimlik doğrulama istek sınırı (Rate limit)
    public const string AuthRateLimitPolicy = "auth";
    public const int AuthRateLimitPermitCount = 10;
    public const int AuthRateLimitWindowMinutes = 1;

    // Giriş denetim kaydı başarısızlık nedenleri
    public const string FailureInvalidCredentials = "InvalidCredentials";
    public const string FailureLockedOut = "LockedOut";
    public const string FailureInactive = "Inactive";
    public const string FailureCaptcha = "Captcha";
    public const string FailureInvalidChallenge = "InvalidChallenge";

    /// <summary>
    /// Danışmanın oturum durumu önbellek anahtarını üretir.
    /// </summary>
    public static string AuthStateCacheKey(int agentId) => $"auth-state:{agentId}";
}
