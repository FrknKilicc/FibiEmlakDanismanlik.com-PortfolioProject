using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Commands.LeadCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.LeadInterfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.LeadHandlers
{
    /// <summary>
    /// Müşteri talebini bir danışmana atayan ve atama hareketini kaydeden komut işleyicisi (Yalnızca Sahip/Yönetici).
    /// </summary>
    public class AssignLeadCommandHandler : IRequestHandler<AssignLeadCommand, bool>
    {
        private readonly ILeadRepository _leadRepository;
        private readonly ICurrentUserService _currentUserService;

        public AssignLeadCommandHandler(ILeadRepository leadRepository, ICurrentUserService currentUserService)
        {
            _leadRepository = leadRepository;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(AssignLeadCommand request, CancellationToken cancellationToken)
        {
            var isPrivileged = _currentUserService.Role == AgentRole.Owner || _currentUserService.Role == AgentRole.Manager;
            if (!isPrivileged)
            {
                throw new LeadAccessDeniedException("Yalnızca Sahip ve Yönetici rolündeki kullanıcılar danışman ataması yapabilir.");
            }

            var ownership = await _leadRepository.GetOwnershipAsync(request.Source, request.LeadId);
            if (ownership == null)
            {
                throw new KeyNotFoundException("Talep bulunamadı.");
            }

            if (request.AgentId.HasValue)
            {
                var isActive = await _leadRepository.IsActiveAgentAsync(request.AgentId.Value);
                if (!isActive)
                {
                    throw new ValidationException(new[] { new ValidationFailure("AgentId", "Atanan danışman bulunamadı veya aktif durumda değil.") });
                }
            }

            await _leadRepository.AssignAsync(request.Source, request.LeadId, request.AgentId);

            var activity = new LeadActivity
            {
                Source = request.Source,
                LeadId = request.LeadId,
                AgentId = _currentUserService.AgentId,
                Type = LeadActivityType.Assignment,
                Text = null,
                OldStatus = null,
                NewStatus = null,
                AssignedAgentId = request.AgentId,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _leadRepository.AddActivityAsync(activity);
            return true;
        }
    }
}
