using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Commands.AgentCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.AgentInterfaces;
using FibiEmlakDanismanlik.Application.Services.Auth;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.AgentHandlers
{
    public class CreateAgentCommandHandler : IRequestHandler<CreateAgentCommand>
    {
        private readonly IRepository<Agent> _repository;
        private readonly IAgentRepository _agentRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthStateCache _authStateCache;

        public CreateAgentCommandHandler(
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

        public async Task Handle(CreateAgentCommand request, CancellationToken cancellationToken)
        {
            var (actorId, actorRole) = AgentManagementRules.EnsureAuthenticated(_currentUserService);
            AgentManagementRules.EnsureCanAssignRole(actorRole, request.Role);

            var mail = request.Mail?.Trim();
            if (await _agentRepository.MailExistsAsync(mail))
                throw new DuplicateAgentMailException(mail);

            await _repository.CreateAsync(new Agent
            {
                AgentDescription = request.AgentDescription ?? string.Empty,
                AgentImgUrl = request.AgentImgUrl ?? string.Empty,
                AgentName = request.AgentName,
                AgentPhoneNumber = request.AgentPhoneNumber,
                AgentTitle = request.AgentTitle ?? string.Empty,
                Mail = mail,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = request.Role,
                ShowOnWebsite = request.ShowOnWebsite,
                CanViewAllListings = request.CanViewAllListings,
                IsActive = true,
                MustChangePassword = true,
                SecurityStamp = Guid.NewGuid().ToString("N"),
                PasswordChangedAtUtc = null
            });
        }
    }
}
