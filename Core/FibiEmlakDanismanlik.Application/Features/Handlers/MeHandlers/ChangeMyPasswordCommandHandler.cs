using FibiEmlakDanismanlik.Application.Features.Commands.MeCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.MeHandlers
{
    /// <summary>
    /// Giriş yapmış çalışanın şifresini değiştiren, güvenlik damgasını yenileyen ve oturum önbelleğini temizleyen işleyici.
    /// </summary>
    public class ChangeMyPasswordCommandHandler : IRequestHandler<ChangeMyPasswordCommand>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IRepository<Agent> _agentRepository;
        private readonly IAuthStateCache _authStateCache;

        public ChangeMyPasswordCommandHandler(
            ICurrentUserService currentUserService,
            IRepository<Agent> agentRepository,
            IAuthStateCache authStateCache)
        {
            _currentUserService = currentUserService;
            _agentRepository = agentRepository;
            _authStateCache = authStateCache;
        }

        public async Task Handle(ChangeMyPasswordCommand request, CancellationToken cancellationToken)
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

            // Yeni şifrenin eskisiyle aynı olup olmadığını kontrol et
            bool isSamePassword = false;
            try
            {
                isSamePassword = BCrypt.Net.BCrypt.Verify(request.NewPassword, agent.Password);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                isSamePassword = false;
            }

            if (isSamePassword)
            {
                throw new ValidationException(
                    "Yeni şifre eskisiyle aynı olamaz.",
                    new[] { new ValidationFailure(nameof(request.NewPassword), "Yeni şifre eskisiyle aynı olamaz.") });
            }

            // Şifreyi hash'le, zorunlu şifre değişimini kapat, damgayı yenile ve kaydet
            agent.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            agent.MustChangePassword = false;
            agent.PasswordChangedAtUtc = DateTime.UtcNow;
            agent.SecurityStamp = Guid.NewGuid().ToString("N");

            await _agentRepository.UpdateAsync(agent);

            // Tüm aktif oturumları geçersiz kıl
            _authStateCache.Invalidate(agent.AgentId);
        }
    }
}
