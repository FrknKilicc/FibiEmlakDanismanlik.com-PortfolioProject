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
    public class UpdateAgentCommandHandler : IRequestHandler<UpdateAgentCommand>
    {
        private readonly IRepository<Agent> _repository;
        private readonly IAgentRepository _agentRepository;

        public UpdateAgentCommandHandler(IRepository<Agent> repository, IAgentRepository agentRepository)
        {
            _repository = repository;
            _agentRepository = agentRepository;
        }

        public async Task Handle(UpdateAgentCommand request, CancellationToken cancellationToken)
        {
            var mail = request.Mail?.Trim();
            if (await _agentRepository.MailExistsAsync(mail, request.AgentId))
                throw new DuplicateAgentMailException(mail);

            var value = await _repository.GetByIdAsync(request.AgentId);
            value.AgentName = request.AgentName;
            value.AgentDescription = request.AgentDescription;
            value.AgentTitle = request.AgentTitle;
            value.AgentImgUrl = request.AgentImgUrl;
            value.AgentPhoneNumber = request.AgentPhoneNumber;
            value.AgentId = request.AgentId;
            // Şifre boş gönderilirse mevcut hash korunur.
            if (!string.IsNullOrWhiteSpace(request.Password))
                value.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
            value.Mail = mail;
            await _repository.UpdateAsync(value);
        }
    }
}
