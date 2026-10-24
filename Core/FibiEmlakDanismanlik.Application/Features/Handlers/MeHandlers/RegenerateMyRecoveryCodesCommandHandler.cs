using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.MeCommands;
using FibiEmlakDanismanlik.Application.Helpers;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.AgentInterfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.MeHandlers
{
    /// <summary>
    /// Şifre doğrulaması yaparak iki adımlı kurtarma kodlarını geçersiz kılıp yenilerini üreten işleyici.
    /// </summary>
    public class RegenerateMyRecoveryCodesCommandHandler : IRequestHandler<RegenerateMyRecoveryCodesCommand, List<string>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IRepository<Agent> _agentRepository;
        private readonly IAgentRepository _agentCustomRepository;

        public RegenerateMyRecoveryCodesCommandHandler(
            ICurrentUserService currentUserService,
            IRepository<Agent> agentRepository,
            IAgentRepository agentCustomRepository)
        {
            _currentUserService = currentUserService;
            _agentRepository = agentRepository;
            _agentCustomRepository = agentCustomRepository;
        }

        public async Task<List<string>> Handle(RegenerateMyRecoveryCodesCommand request, CancellationToken cancellationToken)
        {
            var agentId = _currentUserService.AgentId;
            if (agentId == null)
            {
                throw new UnauthorizedAccessException("Oturum açmış kullanıcı bulunamadı.");
            }

            var agent = await _agentRepository.GetByIdAsync(agentId.Value);
            if (agent == null)
            {
                throw new KeyNotFoundException("Kullanıcı hesabı bulunamadı.");
            }

            // Mevcut şifreyi doğrula
            bool isCurrentPasswordValid = false;
            try
            {
                isCurrentPasswordValid = !string.IsNullOrEmpty(agent.Password) &&
                                         BCrypt.Net.BCrypt.Verify(request.CurrentPassword, agent.Password);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                isCurrentPasswordValid = false;
            }

            if (!isCurrentPasswordValid)
            {
                throw new ValidationException(
                    "Mevcut şifre hatalı.",
                    new[] { new ValidationFailure(nameof(request.CurrentPassword), "Mevcut şifre hatalı.") });
            }

            // İki adımlı doğrulama etkin mi kontrol et
            if (!agent.TwoFactorEnabled)
            {
                throw new ValidationException(
                    "İki adımlı doğrulama açık değil.",
                    new[] { new ValidationFailure(nameof(agent.TwoFactorEnabled), "İki adımlı doğrulama açık değil.") });
            }

            // Yeni kurtarma kodları üret
            var codes = RecoveryCodeGenerator.Generate(AuthConstants.RecoveryCodeCount);

            // BCrypt hash'lerini veritabanına kaydet
            var codeHashes = codes.Select(c => BCrypt.Net.BCrypt.HashPassword(c)).ToList();
            await _agentCustomRepository.ReplaceRecoveryCodesAsync(agent.AgentId, codeHashes);

            // Düz kodları tek seferlik yanıtta döndür (asla loglanmaz)
            return codes;
        }
    }
}
