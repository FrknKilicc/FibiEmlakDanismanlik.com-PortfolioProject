using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.AuthCommands;
using FibiEmlakDanismanlik.Application.Features.Results.AuthResults;
using FibiEmlakDanismanlik.Application.Helpers;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.AgentInterfaces;
using FibiEmlakDanismanlik.Application.Services.Auth;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.AuthHandlers
{
    /// <summary>
    /// İki adımlı doğrulama aşamasında TOTP kodu veya kurtarma kodunu doğrulayarak girişi tamamlayan komut işleyicisi.
    /// </summary>
    public class VerifyTwoFactorCommandHandler : IRequestHandler<VerifyTwoFactorCommand, GetLoginResult?>
    {
        private readonly IRepository<Agent> _agentRepository;
        private readonly IAgentRepository _agentCustomRepository;
        private readonly ILoginFlowService _loginFlowService;
        private readonly ITwoFactorService _twoFactorService;
        private readonly IConfiguration _configuration;

        public VerifyTwoFactorCommandHandler(
            IRepository<Agent> agentRepository,
            IAgentRepository agentCustomRepository,
            ILoginFlowService loginFlowService,
            ITwoFactorService twoFactorService,
            IConfiguration configuration)
        {
            _agentRepository = agentRepository;
            _agentCustomRepository = agentCustomRepository;
            _loginFlowService = loginFlowService;
            _twoFactorService = twoFactorService;
            _configuration = configuration;
        }

        public async Task<GetLoginResult?> Handle(VerifyTwoFactorCommand request, CancellationToken cancellationToken)
        {
            // Challenge token'ı '2fa' amacıyla doğrula
            var agentId = ChallengeTokenHelper.Validate(
                request.ChallengeToken,
                AuthConstants.PurposeTwoFactor,
                _configuration,
                out var tokenSecurityStamp);

            if (agentId == null)
            {
                await _loginFlowService.LogAsync(string.Empty, null, succeeded: false, failureReason: AuthConstants.FailureInvalidChallenge);
                return null;
            }

            var agent = await _agentRepository.GetByIdAsync(agentId.Value);
            if (agent == null || !agent.IsActive || !string.Equals(agent.SecurityStamp, tokenSecurityStamp, StringComparison.Ordinal))
            {
                await _loginFlowService.LogAsync(agent?.Mail ?? string.Empty, agent?.AgentId, succeeded: false, failureReason: AuthConstants.FailureInvalidChallenge);
                return null;
            }

            // Hesap kilitli mi denetle
            if (_loginFlowService.IsLockedOut(agent))
            {
                await _loginFlowService.LogAsync(agent.Mail ?? string.Empty, agent.AgentId, succeeded: false, failureReason: AuthConstants.FailureLockedOut);
                return null;
            }

            bool isVerified = false;

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                // TOTP kodu doğrulaması
                var secret = _twoFactorService.Unprotect(agent.TwoFactorSecretProtected);
                if (!string.IsNullOrEmpty(secret))
                {
                    isVerified = _twoFactorService.VerifyCodeOnce(agent.AgentId, secret, request.Code);
                }
            }
            else if (!string.IsNullOrWhiteSpace(request.RecoveryCode))
            {
                // Kurtarma kodu doğrulaması (büyük harfe çevir, boşlukları sil)
                var cleanRecoveryCode = request.RecoveryCode.Replace(" ", "").ToUpperInvariant();
                var unusedCodes = await _agentCustomRepository.GetUnusedRecoveryCodesAsync(agent.AgentId);

                foreach (var unusedCode in unusedCodes)
                {
                    bool matches = false;
                    try
                    {
                        matches = BCrypt.Net.BCrypt.Verify(cleanRecoveryCode, unusedCode.CodeHash);
                        if (!matches && cleanRecoveryCode.Length == 8 && !cleanRecoveryCode.Contains('-'))
                        {
                            var dashedCode = $"{cleanRecoveryCode.Substring(0, 4)}-{cleanRecoveryCode.Substring(4, 4)}";
                            matches = BCrypt.Net.BCrypt.Verify(dashedCode, unusedCode.CodeHash);
                        }
                    }
                    catch (BCrypt.Net.SaltParseException)
                    {
                        matches = false;
                    }

                    if (matches)
                    {
                        await _agentCustomRepository.MarkRecoveryCodeUsedAsync(unusedCode.AgentRecoveryCodeId);
                        isVerified = true;
                        break;
                    }
                }
            }

            // Başarısızlık durumunda başarısız denemeyi kaydet ve denetim günlüğü yaz
            if (!isVerified)
            {
                await _loginFlowService.RegisterFailureAsync(agent);
                await _loginFlowService.LogAsync(agent.Mail ?? string.Empty, agent.AgentId, succeeded: false, failureReason: AuthConstants.FailureInvalidCredentials);
                return null;
            }

            // Doğrulama başarılı: giriş akışını 2FA sağlanmış olarak tamamla
            return await _loginFlowService.CompleteAsync(agent, twoFactorSatisfied: true);
        }
    }
}
