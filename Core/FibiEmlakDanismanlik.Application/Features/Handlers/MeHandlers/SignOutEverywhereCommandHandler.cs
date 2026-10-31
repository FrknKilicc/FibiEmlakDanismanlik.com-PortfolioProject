using FibiEmlakDanismanlik.Application.Features.Commands.MeCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.MeHandlers
{
    /// <summary>
    /// Güvenlik damgasını yenileyip oturum önbelleğini temizleyerek tüm cihazlardaki oturumları kapatan işleyici.
    /// </summary>
    public class SignOutEverywhereCommandHandler : IRequestHandler<SignOutEverywhereCommand>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IRepository<Agent> _agentRepository;
        private readonly IAuthStateCache _authStateCache;

        public SignOutEverywhereCommandHandler(
            ICurrentUserService currentUserService,
            IRepository<Agent> agentRepository,
            IAuthStateCache authStateCache)
        {
            _currentUserService = currentUserService;
            _agentRepository = agentRepository;
            _authStateCache = authStateCache;
        }

        public async Task Handle(SignOutEverywhereCommand request, CancellationToken cancellationToken)
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

            agent.SecurityStamp = Guid.NewGuid().ToString("N");
            await _agentRepository.UpdateAsync(agent);

            _authStateCache.Invalidate(agent.AgentId);
        }
    }
}
