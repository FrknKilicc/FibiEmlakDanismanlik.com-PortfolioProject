using System;

namespace FibiEmlakDanismanlik.Application.Features.Results.AgentResult
{
    public class GetAgentAdminListResult
    {
        public int AgentId { get; set; }
        public string AgentName { get; set; } = string.Empty;
        public string AgentTitle { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public string AgentPhoneNumber { get; set; } = string.Empty;
        public string AgentImgUrl { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool ShowOnWebsite { get; set; }
        public bool CanViewAllListings { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool MustChangePassword { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTime? LastLoginAtUtc { get; set; }
    }
}
