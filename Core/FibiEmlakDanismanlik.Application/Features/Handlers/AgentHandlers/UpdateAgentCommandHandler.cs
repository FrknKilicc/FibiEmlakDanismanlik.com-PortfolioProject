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
    public class UpdateAgentCommandHandler : IRequestHandler<UpdateAgentCommand>
    {
        private readonly IRepository<Agent> _repository;

        public UpdateAgentCommandHandler(IRepository<Agent> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateAgentCommand request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.AgentId);
            value.AgentName = request.AgentName;
            value.AgentDescription = request.AgentDescription;
            value.AgentTitle = request.AgentTitle;
            value.AgentImgUrl = request.AgentImgUrl;
            value.AgentPhoneNumber = request.AgentPhoneNumber;
            value.AgentId = request.AgentId;
            value.Password = request.Password;
            value.Mail=request.Mail;
            await _repository.UpdateAsync(value);
        }
    }
}
