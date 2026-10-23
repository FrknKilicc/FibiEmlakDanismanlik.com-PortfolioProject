using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Features.Commands.AdminListingCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces;
using FibiEmlakDanismanlik.Domain.Enums;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.AdminListingHandlers
{
    /// <summary>
    /// Bir ilana danışman erişim yetkisi tanımlayan veya güncelleyen komut işleyicisi (Yalnızca Sahip/Yönetici).
    /// </summary>
    public class AddAdminListingGrantCommandHandler : IRequestHandler<AddAdminListingGrantCommand, bool>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IAdminListingRepository _adminListingRepository;

        public AddAdminListingGrantCommandHandler(
            ICurrentUserService currentUserService,
            IAdminListingRepository adminListingRepository)
        {
            _currentUserService = currentUserService;
            _adminListingRepository = adminListingRepository;
        }

        public async Task<bool> Handle(AddAdminListingGrantCommand request, CancellationToken cancellationToken)
        {
            var listingAgentId = await _adminListingRepository.GetListingAgentIdAsync(request.ListingTypeId, request.ListingId);
            if (!listingAgentId.HasValue)
            {
                throw new KeyNotFoundException("İlan bulunamadı.");
            }

            // İlanın kendi sahibi olamaz
            if (listingAgentId.Value == request.AgentId)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure(nameof(request.AgentId), "İlanın kendi sahibine ayrıca erişim yetkisi tanımlanamaz.")
                });
            }

            // Hedef aktif bir danışman olmalı
            var targetAgent = await _adminListingRepository.GetAgentByIdAsync(request.AgentId);
            if (targetAgent == null || !targetAgent.IsActive || targetAgent.Role != AgentRole.Consultant)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure(nameof(request.AgentId), "Erişim yetkisi yalnızca aktif danışman hesaplarına verilebilir.")
                });
            }

            var grantedByAgentId = _currentUserService.AgentId ?? targetAgent.AgentId;
            var accessLevel = string.Equals(request.Access, "Edit", StringComparison.OrdinalIgnoreCase)
                ? ListingAccessLevel.Edit
                : ListingAccessLevel.View;

            await _adminListingRepository.AddOrUpdateGrantAsync(
                request.ListingTypeId,
                request.ListingId,
                request.AgentId,
                accessLevel,
                grantedByAgentId);

            return true;
        }
    }
}
