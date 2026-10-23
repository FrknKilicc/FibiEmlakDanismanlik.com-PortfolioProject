namespace FibiEmlakDanismanlik.AdminUI.Infrastructure;

/// <summary>
/// Yönetici oturumunda kullanılan özel talep (claim) türleri.
/// </summary>
public static class AdminClaimTypes
{
    public const string AccessToken = "access_token";
    public const string AccessTokenExpiresUtc = "access_token_expires_utc";
    public const string IssuedAtUtc = "issued_at_utc";
}
