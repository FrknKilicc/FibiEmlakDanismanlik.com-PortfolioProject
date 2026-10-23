using FibiEmlakDanismanlik.Application.Constants;
using System;

namespace FibiEmlakDanismanlik.Application.Features.Results.AuthResults
{
    public class GetLoginResult
    {
        public string? Token { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string Status { get; set; } = AuthConstants.LoginStatusSuccess;
        public string? ChallengeToken { get; set; }
        public string? AgentName { get; set; }
        public string? Role { get; set; }
    }
}
