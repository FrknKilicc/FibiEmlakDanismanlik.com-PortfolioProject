using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Commands.ListingAlertCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.ListingAlertInterfaces;
using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ListingAlertHandlers
{
    /// <summary>
    /// Haber talebinin takip durumunu değiştiren komut işleyicisi.
    /// </summary>
    public class ChangeListingAlertStatusCommandHandler : IRequestHandler<ChangeListingAlertStatusCommand, bool>
    {
        private readonly IListingAlertRepository _listingAlertRepository;
        private readonly ICurrentUserService _currentUserService;

        public ChangeListingAlertStatusCommandHandler(IListingAlertRepository listingAlertRepository, ICurrentUserService currentUserService)
        {
            _listingAlertRepository = listingAlertRepository;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(ChangeListingAlertStatusCommand request, CancellationToken cancellationToken)
        {
            var exists = await _listingAlertRepository.ExistsAsync(request.Id);
            if (!exists)
            {
                throw new KeyNotFoundException("Haber talebi bulunamadı.");
            }

            var isPrivileged = _currentUserService.Role == AgentRole.Owner || _currentUserService.Role == AgentRole.Manager;
            if (!isPrivileged)
            {
                var assignedAgentId = await _listingAlertRepository.GetAssignedAgentIdAsync(request.Id);
                if (assignedAgentId != _currentUserService.AgentId)
                {
                    throw new ListingAlertAccessDeniedException("Bu talebin durumunu güncelleme yetkiniz yok.");
                }
            }

            return await _listingAlertRepository.SetStatusAsync(request.Id, request.Status);
        }
    }
}
