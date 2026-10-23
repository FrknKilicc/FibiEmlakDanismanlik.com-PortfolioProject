using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Commands.AgentCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.AgentInterfaces;
using FibiEmlakDanismanlik.Application.Services.Auth;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.AgentHandlers
{
    public class UpdateAgentCommandHandler : IRequestHandler<UpdateAgentCommand>
    {
        private readonly IRepository<Agent> _repository;
        private readonly IAgentRepository _agentRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthStateCache _authStateCache;

        public UpdateAgentCommandHandler(
            IRepository<Agent> repository,
            IAgentRepository agentRepository,
            ICurrentUserService currentUserService,
            IAuthStateCache authStateCache)
        {
            _repository = repository;
            _agentRepository = agentRepository;
            _currentUserService = currentUserService;
            _authStateCache = authStateCache;
        }

        public async Task Handle(UpdateAgentCommand request, CancellationToken cancellationToken)
        {
            var (actorId, actorRole) = AgentManagementRules.EnsureAuthenticated(_currentUserService);

            var target = await _repository.GetByIdAsync(request.AgentId);
            if (target == null)
                throw new AgentNotFoundException(request.AgentId);

            AgentManagementRules.EnsureCanManage(actorRole, actorId, target);

            var mail = request.Mail?.Trim();
            if (await _agentRepository.MailExistsAsync(mail, request.AgentId))
                throw new DuplicateAgentMailException(mail);

            target.AgentName = request.AgentName;
            target.AgentDescription = request.AgentDescription;
            target.AgentTitle = request.AgentTitle;
            target.AgentImgUrl = request.AgentImgUrl;
            target.AgentPhoneNumber = request.AgentPhoneNumber;
            target.Mail = mail;

            bool stampChanged = false;

            // Rol değişimi
            if (request.Role.HasValue && request.Role.Value != target.Role)
            {
                if (actorId == target.AgentId)
                    throw new AgentManagementForbiddenException("Kendi rolünüzü değiştiremezsiniz.");

                AgentManagementRules.EnsureCanAssignRole(actorRole, request.Role.Value);

                if (target.Role == AgentRole.Owner && target.IsActive && await _agentRepository.CountActiveOwnersAsync(target.AgentId) == 0)
                    throw new AgentManagementForbiddenException("Son aktif Sahip'in rolü düşürülemez.");

                target.Role = request.Role.Value;
                target.SecurityStamp = Guid.NewGuid().ToString("N");
                stampChanged = true;
            }

            // Aktiflik değişimi
            if (request.IsActive.HasValue && request.IsActive.Value != target.IsActive)
            {
                if (!request.IsActive.Value)
                {
                    if (actorId == target.AgentId)
                        throw new AgentManagementForbiddenException("Kendinizi pasife alamazsınız.");

                    if (target.Role == AgentRole.Owner && target.IsActive && await _agentRepository.CountActiveOwnersAsync(target.AgentId) == 0)
                        throw new AgentManagementForbiddenException("Son aktif Sahip pasife alınamaz.");
                }

                target.IsActive = request.IsActive.Value;
                target.SecurityStamp = Guid.NewGuid().ToString("N");
                stampChanged = true;
            }

            // Gösterim ve yetki bayrakları
            if (request.ShowOnWebsite.HasValue)
                target.ShowOnWebsite = request.ShowOnWebsite.Value;

            if (request.CanViewAllListings.HasValue)
                target.CanViewAllListings = request.CanViewAllListings.Value;

            // Şifre güncelleme
            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                target.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
                target.MustChangePassword = (actorId != target.AgentId);
                target.PasswordChangedAtUtc = DateTime.UtcNow;
                target.SecurityStamp = Guid.NewGuid().ToString("N");
                stampChanged = true;
            }

            // İki adımlı doğrulama sıfırlama
            if (request.ResetTwoFactor)
            {
                target.TwoFactorEnabled = false;
                target.TwoFactorSecretProtected = null;
                await _agentRepository.ReplaceRecoveryCodesAsync(target.AgentId, Array.Empty<string>());
                target.SecurityStamp = Guid.NewGuid().ToString("N");
                stampChanged = true;
            }

            // Kilit açma
            if (request.Unlock)
            {
                target.FailedLoginCount = 0;
                target.LockoutEndUtc = null;
            }

            await _repository.UpdateAsync(target);

            if (stampChanged)
            {
                _authStateCache.Invalidate(target.AgentId);
            }
        }
    }
}
