using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Commands.AgentCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.AgentInterfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.AgentHandlers
{
    public class CreateAgentCommandHandler : IRequestHandler<CreateAgentCommand>
    {
        private readonly IRepository<Agent> _repository;
        private readonly IAgentRepository _agentRepository;

        public CreateAgentCommandHandler(IRepository<Agent> repository, IAgentRepository agentRepository)
        {
            _repository = repository;
            _agentRepository = agentRepository;
        }

        public async Task Handle(CreateAgentCommand request, CancellationToken cancellationToken)
        {
            var mail = request.Mail?.Trim();
            if (await _agentRepository.MailExistsAsync(mail))
                throw new DuplicateAgentMailException(mail);

            await _repository.CreateAsync(new Agent
            {
                AgentDescription = request.AgentDescription,
                AgentImgUrl = request.AgentImgUrl,
                AgentName = request.AgentName,
                AgentPhoneNumber = request.AgentPhoneNumber,
                AgentTitle = request.AgentTitle,
                Mail = mail,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            });
        }
    }
}
