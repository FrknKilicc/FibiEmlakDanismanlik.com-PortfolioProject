using System.Collections.Generic;
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
    /// Haber talebini bir danışmana atayan komut işleyicisi (Yalnızca Sahip ve Yönetici).
    /// </summary>
    public class AssignListingAlertCommandHandler : IRequestHandler<AssignListingAlertCommand, bool>
    {
        private readonly IListingAlertRepository _listingAlertRepository;
        private readonly ICurrentUserService _currentUserService;

        public AssignListingAlertCommandHandler(IListingAlertRepository listingAlertRepository, ICurrentUserService currentUserService)
        {
            _listingAlertRepository = listingAlertRepository;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(AssignListingAlertCommand request, CancellationToken cancellationToken)
        {
            var isPrivileged = _currentUserService.Role == AgentRole.Owner || _currentUserService.Role == AgentRole.Manager;
            if (!isPrivileged)
            {
                throw new ListingAlertAccessDeniedException("Yalnızca Sahip ve Yönetici rolündeki kullanıcılar danışman ataması yapabilir.");
            }

            var exists = await _listingAlertRepository.ExistsAsync(request.Id);
            if (!exists)
            {
                throw new KeyNotFoundException("Haber talebi bulunamadı.");
            }

            if (request.AgentId.HasValue)
            {
                var isActive = await _listingAlertRepository.IsActiveAgentAsync(request.AgentId.Value);
                if (!isActive)
                {
                    throw new ValidationException(new[]
                    {
                        new ValidationFailure("AgentId", "Atanan danışman bulunamadı veya aktif durumda değil.")
                    });
                }
            }

            return await _listingAlertRepository.AssignAsync(request.Id, request.AgentId);
        }
    }
}
