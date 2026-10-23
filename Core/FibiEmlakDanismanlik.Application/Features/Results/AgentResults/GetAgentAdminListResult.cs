using System;

namespace FibiEmlakDanismanlik.Application.Features.Results.AgentResult
{
    public class GetAgentAdminListResult
    {
        public int AgentId { get; set; }
        public string AgentName { get; set; }
        public string AgentTitle { get; set; }
        public string Mail { get; set; }
        public string AgentPhoneNumber { get; set; }
        public string AgentImgUrl { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public bool ShowOnWebsite { get; set; }
        public bool CanViewAllListings { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool MustChangePassword { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTime? LastLoginAtUtc { get; set; }
    }
}
