using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Commands.AdminListingCommands;
using FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.AdminListingHandlers
{
    /// <summary>
    /// Harita pini koordinatlarını yetki denetimiyle güncelleyen işleyici.
    /// </summary>
    public class SetAdminListingGeoPointCommandHandler : IRequestHandler<SetAdminListingGeoPointCommand, bool>
    {
        private readonly IListingAccessService _listingAccessService;
        private readonly IAdminListingRepository _adminListingRepository;

        public SetAdminListingGeoPointCommandHandler(
            IListingAccessService listingAccessService,
            IAdminListingRepository adminListingRepository)
        {
            _listingAccessService = listingAccessService;
            _adminListingRepository = adminListingRepository;
        }

        public async Task<bool> Handle(SetAdminListingGeoPointCommand request, CancellationToken cancellationToken)
        {
            var agentId = await _adminListingRepository.GetListingAgentIdAsync(request.ListingTypeId, request.ListingId);
            if (!agentId.HasValue)
            {
                throw new KeyNotFoundException("İlan bulunamadı.");
            }

            var access = await _listingAccessService.GetAccessAsync(request.ListingTypeId, request.ListingId);
            if (access != ListingAccess.Edit)
            {
                throw new ListingAccessDeniedException("Bu ilanın harita konumunu düzenleme yetkiniz bulunmamaktadır.");
            }

            return await _adminListingRepository.SetGeoPointAsync(
                request.ListingTypeId,
                request.ListingId,
                request.Latitude,
                request.Longitude);
        }
    }
}
