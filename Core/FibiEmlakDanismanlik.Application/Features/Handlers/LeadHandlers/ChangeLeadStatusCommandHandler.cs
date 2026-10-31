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
    /// Müşteri talebi durumunu değiştiren ve durum değişikliği hareketini kaydeden komut işleyicisi.
    /// </summary>
    public class ChangeLeadStatusCommandHandler : IRequestHandler<ChangeLeadStatusCommand, bool>
    {
        private readonly ILeadRepository _leadRepository;
        private readonly ICurrentUserService _currentUserService;

        public ChangeLeadStatusCommandHandler(ILeadRepository leadRepository, ICurrentUserService currentUserService)
        {
            _leadRepository = leadRepository;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(ChangeLeadStatusCommand request, CancellationToken cancellationToken)
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
                    throw new LeadAccessDeniedException("Bu talebin durumunu güncelleme yetkiniz yok.");
                }
            }

            var detail = await _leadRepository.GetDetailAsync(request.Source, request.LeadId);
            if (detail == null)
            {
                throw new KeyNotFoundException("Talep bulunamadı.");
            }

            // Durum aynıysa işlem yapma (no-op)
            if (detail.Status == request.Status)
            {
                return true;
            }

            var oldStatus = detail.Status;
            await _leadRepository.UpdateStatusAsync(request.Source, request.LeadId, request.Status);

            var activity = new LeadActivity
            {
                Source = request.Source,
                LeadId = request.LeadId,
                AgentId = _currentUserService.AgentId,
                Type = LeadActivityType.StatusChange,
                Text = request.Note,
                OldStatus = oldStatus,
                NewStatus = request.Status,
                AssignedAgentId = null,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _leadRepository.AddActivityAsync(activity);
            return true;
        }
    }
}
