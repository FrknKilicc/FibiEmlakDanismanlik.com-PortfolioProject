namespace FibiEmlakDanismanlik.AdminUI.Models.Account;

/// <summary>
/// WebApi kimlik doğrulama uç noktalarının ortak yanıt modeli.
/// </summary>
public sealed class LoginResponse
{
    public string? Token { get; set; }
    public DateTime? ExpireDate { get; set; }
    public string? Status { get; set; }
    public string? ChallengeToken { get; set; }
    public string? AgentName { get; set; }
    public string? Role { get; set; }
}

/// <summary>
/// WebApi Auth/2fa/setup uç noktasından dönen yanıt modeli.
/// </summary>
public sealed class TwoFactorSetupResponse
{
    public string? Secret { get; set; }
    public string? OtpAuthUri { get; set; }
}

/// <summary>
/// WebApi Auth/2fa/enable uç noktasından dönen yanıt modeli.
/// </summary>
public sealed class TwoFactorEnableResponse
{
    public List<string>? RecoveryCodes { get; set; }
    public LoginResponse? Login { get; set; }
}
