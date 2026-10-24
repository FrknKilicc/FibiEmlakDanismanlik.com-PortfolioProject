using System;

namespace FibiEmlakDanismanlik.Application.Features.Results.MeResults
{
    /// <summary>
    /// Giriş yapmış çalışanın profil ve güvenlik durumu bilgilerini içeren sonuç modeli.
    /// </summary>
    public class GetMyProfileResult
    {
        public int AgentId { get; set; }
        public string AgentName { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public string AgentPhoneNumber { get; set; } = string.Empty;
        public string? AgentTitle { get; set; }
        public string? AgentDescription { get; set; }
        public string? AgentImgUrl { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool TwoFactorEnabled { get; set; }
        public int UnusedRecoveryCodeCount { get; set; }
        public DateTime? LastLoginAtUtc { get; set; }
        public DateTime? PasswordChangedAtUtc { get; set; }
    }
}
