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
    /// İlan durumu değiştirme komutunu yetki kurallarına göre işleyen işleyici.
    /// </summary>
    public class ChangeAdminListingStatusCommandHandler : IRequestHandler<ChangeAdminListingStatusCommand, bool>
    {
        private readonly IListingAccessService _listingAccessService;
        private readonly IAdminListingRepository _adminListingRepository;

        public ChangeAdminListingStatusCommandHandler(
            IListingAccessService listingAccessService,
            IAdminListingRepository adminListingRepository)
        {
            _listingAccessService = listingAccessService;
            _adminListingRepository = adminListingRepository;
        }

        public async Task<bool> Handle(ChangeAdminListingStatusCommand request, CancellationToken cancellationToken)
        {
            var agentId = await _adminListingRepository.GetListingAgentIdAsync(request.ListingTypeId, request.ListingId);
            if (!agentId.HasValue)
            {
                throw new KeyNotFoundException("İlan bulunamadı.");
            }

            var access = await _listingAccessService.GetAccessAsync(request.ListingTypeId, request.ListingId);
            if (access != ListingAccess.Edit)
            {
                throw new ListingAccessDeniedException("Bu ilanın durumunu değiştirme yetkiniz bulunmamaktadır.");
            }

            return await _adminListingRepository.SetStatusAsync(request.ListingTypeId, request.ListingId, request.Status);
        }
    }
}
