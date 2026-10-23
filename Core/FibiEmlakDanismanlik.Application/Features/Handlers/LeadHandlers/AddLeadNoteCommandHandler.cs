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
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.LeadHandlers
{
    /// <summary>
    /// Müşteri talebine danışman notu veya görüşme/arama kaydı ekleyen komut işleyicisi.
    /// </summary>
    public class AddLeadNoteCommandHandler : IRequestHandler<AddLeadNoteCommand, bool>
    {
        private readonly ILeadRepository _leadRepository;
        private readonly ICurrentUserService _currentUserService;

        public AddLeadNoteCommandHandler(ILeadRepository leadRepository, ICurrentUserService currentUserService)
        {
            _leadRepository = leadRepository;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(AddLeadNoteCommand request, CancellationToken cancellationToken)
        {
            var ownership = await _leadRepository.GetOwnershipAsync(request.Source, request.LeadId);
            if (ownership == null)
            {
                throw new KeyNotFoundException("Talep bulunamadı.");
            }

            var isPrivileged = _currentUserService.Role == AgentRole.Owner || _currentUserService.Role == AgentRole.Manager;
            if (!isPrivileged)
            {
                var currentAgentId = _currentUserService.AgentId;
                var isAssigned = ownership.Value.AssignedAgentId.HasValue && ownership.Value.AssignedAgentId == currentAgentId;
                var isListingOwner = ownership.Value.ListingAgentId.HasValue && ownership.Value.ListingAgentId == currentAgentId;

                if (!isAssigned && !isListingOwner)
                {
                    throw new LeadAccessDeniedException("Bu talebe not ekleme yetkiniz yok.");
                }
            }

            var activity = new LeadActivity
            {
                Source = request.Source,
                LeadId = request.LeadId,
                AgentId = _currentUserService.AgentId,
                Type = request.IsCall ? LeadActivityType.Call : LeadActivityType.Note,
                Text = request.Text,
                OldStatus = null,
                NewStatus = null,
                AssignedAgentId = null,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _leadRepository.AddActivityAsync(activity);
            return true;
        }
    }
}
