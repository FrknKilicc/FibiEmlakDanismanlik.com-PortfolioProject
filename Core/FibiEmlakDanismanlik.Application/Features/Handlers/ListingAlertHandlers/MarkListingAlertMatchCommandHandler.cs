using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Commands.ListingAlertCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.ListingAlertInterfaces;
using FibiEmlakDanismanlik.Domain.Enums;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ListingAlertHandlers
{
    /// <summary>
    /// Haber talebi ile bir ilanın eşleşme/bildirim durumunu güncelleyen veya ekleyen komut işleyicisi.
    /// </summary>
    public class MarkListingAlertMatchCommandHandler : IRequestHandler<MarkListingAlertMatchCommand, bool>
    {
        private readonly IListingAlertRepository _listingAlertRepository;
        private readonly ICurrentUserService _currentUserService;

        public MarkListingAlertMatchCommandHandler(IListingAlertRepository listingAlertRepository, ICurrentUserService currentUserService)
        {
            _listingAlertRepository = listingAlertRepository;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(MarkListingAlertMatchCommand request, CancellationToken cancellationToken)
        {
            var exists = await _listingAlertRepository.ExistsAsync(request.AlertId);
            if (!exists)
            {
                throw new KeyNotFoundException("Haber talebi bulunamadı.");
            }

            var isPrivileged = _currentUserService.Role == AgentRole.Owner || _currentUserService.Role == AgentRole.Manager;
            if (!isPrivileged)
            {
                var assignedAgentId = await _listingAlertRepository.GetAssignedAgentIdAsync(request.AlertId);
                if (assignedAgentId != _currentUserService.AgentId)
                {
                    throw new ListingAlertAccessDeniedException("Bu talep üzerinde eşleşme işaretleme yetkiniz yok.");
                }
            }

            // İlanın kaydın kriterlerine uyan aktif bir ilan olduğunu doğrula
            var seeAllScope = new ListingAlertScope { SeeAll = true };
            var matchingAlerts = await _listingAlertRepository.GetAlertsForListingAsync(request.ListingTypeId, request.ListingId, seeAllScope);
            if (!matchingAlerts.Any(x => x.ListingAlertId == request.AlertId))
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure("ListingId", "Bu ilan kaydın kriterlerine uymuyor.")
                });
            }

            await _listingAlertRepository.UpsertMatchAsync(
                request.AlertId,
                request.ListingTypeId,
                request.ListingId,
                request.Status,
                _currentUserService.AgentId);

            return true;
        }
    }
}
