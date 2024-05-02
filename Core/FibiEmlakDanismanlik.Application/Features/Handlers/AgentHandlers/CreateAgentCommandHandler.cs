using FibiEmlakDanismanlik.Application.Features.Commands.AgentCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
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

        public CreateAgentCommandHandler(IRepository<Agent> repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateAgentCommand request, CancellationToken cancellationToken)
        {
            await _repository.CreateAsync(new Agent
            {
                AgentDescription = request.AgentDescription,
                AgentImgUrl = request.AgentImgUrl,
                AgentName = request.AgentName,
                AgentPhoneNumber = request.AgentPhoneNumber,
                AgentTitle = request.AgentTitle,
                Mail = request.Mail,
                Password = request.Password,
            });
        }
    }
}
