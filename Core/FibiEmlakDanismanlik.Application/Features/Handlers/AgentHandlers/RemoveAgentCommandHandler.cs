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
    public class RemoveAgentCommandHandler : IRequestHandler<RemoveAgentCommand>
    {
        private readonly IRepository<Agent> _repository;
        private readonly IAgentRepository _agentRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthStateCache _authStateCache;

        public RemoveAgentCommandHandler(
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

        public async Task Handle(RemoveAgentCommand request, CancellationToken cancellationToken)
        {
            var (actorId, actorRole) = AgentManagementRules.EnsureAuthenticated(_currentUserService);

            var target = await _repository.GetByIdAsync(request.Id);
            if (target == null)
                throw new AgentNotFoundException(request.Id);

            AgentManagementRules.EnsureCanManage(actorRole, actorId, target);

            if (actorId == target.AgentId)
                throw new AgentManagementForbiddenException("Kendinizi pasife alamazsınız.");

            if (target.Role == AgentRole.Owner && target.IsActive && await _agentRepository.CountActiveOwnersAsync(target.AgentId) == 0)
                throw new AgentManagementForbiddenException("Son aktif Sahip pasife alınamaz.");

            target.IsActive = false;
            target.ShowOnWebsite = false;
            target.SecurityStamp = Guid.NewGuid().ToString("N");

            await _repository.UpdateAsync(target);
            _authStateCache.Invalidate(target.AgentId);
        }
    }
}
