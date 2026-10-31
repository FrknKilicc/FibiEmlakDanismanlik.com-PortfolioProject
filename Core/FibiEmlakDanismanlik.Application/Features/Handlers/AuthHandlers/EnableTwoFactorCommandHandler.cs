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
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.AuthHandlers
{
    /// <summary>
    /// İki adımlı doğrulamayı kod ile onaylayıp kurtarma kodları üreten ve etkinleştiren komut işleyicisi.
    /// </summary>
    public class EnableTwoFactorCommandHandler : IRequestHandler<EnableTwoFactorCommand, GetEnableTwoFactorResult?>
    {
        private readonly IRepository<Agent> _agentRepository;
        private readonly IAgentRepository _agentCustomRepository;
        private readonly ILoginFlowService _loginFlowService;
        private readonly ITwoFactorService _twoFactorService;
        private readonly IConfiguration _configuration;

        public EnableTwoFactorCommandHandler(
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

        public async Task<GetEnableTwoFactorResult?> Handle(EnableTwoFactorCommand request, CancellationToken cancellationToken)
        {
            // Challenge token'ı '2fa-setup' amacıyla doğrula
            var agentId = ChallengeTokenHelper.Validate(
                request.ChallengeToken,
                AuthConstants.PurposeTwoFactorSetup,
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

            // Şifrelenmiş gizli anahtarı çöz
            var secret = _twoFactorService.Unprotect(agent.TwoFactorSecretProtected);
            if (string.IsNullOrEmpty(secret))
            {
                await _loginFlowService.LogAsync(agent.Mail ?? string.Empty, agent.AgentId, succeeded: false, failureReason: AuthConstants.FailureInvalidChallenge);
                return null;
            }

            // Girilen 6 haneli TOTP kodunu doğrula
            if (!_twoFactorService.VerifyCodeOnce(agent.AgentId, secret, request.Code))
            {
                await _loginFlowService.RegisterFailureAsync(agent);
                await _loginFlowService.LogAsync(agent.Mail ?? string.Empty, agent.AgentId, succeeded: false, failureReason: AuthConstants.FailureInvalidCredentials);
                return null;
            }

            // Kod doğruysa 2FA'yı etkinleştir ve güvenlik damgasını yenile
            agent.TwoFactorEnabled = true;
            agent.SecurityStamp = Guid.NewGuid().ToString("N");

            // Kurtarma kodlarını üret
            var recoveryCodes = RecoveryCodeGenerator.Generate(AuthConstants.RecoveryCodeCount);

            // Kurtarma kodlarının BCrypt hash'lerini veritabanına kaydet
            var codeHashes = recoveryCodes.Select(c => BCrypt.Net.BCrypt.HashPassword(c)).ToList();
            await _agentCustomRepository.ReplaceRecoveryCodesAsync(agent.AgentId, codeHashes);

            // Danışman durumunu kaydet
            await _agentRepository.UpdateAsync(agent);

            // Giriş akışını tamamla (2FA şartı sağlandı)
            var loginResult = await _loginFlowService.CompleteAsync(agent, twoFactorSatisfied: true);

            // Düz metin kurtarma kodları yalnızca bu yanıtta tek seferlik döndürülür
            return new GetEnableTwoFactorResult
            {
                RecoveryCodes = recoveryCodes,
                Login = loginResult
            };
        }
    }
}
