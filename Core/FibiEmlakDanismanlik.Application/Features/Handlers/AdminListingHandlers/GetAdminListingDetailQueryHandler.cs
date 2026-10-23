using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Queries.AdminListingQueries;
using FibiEmlakDanismanlik.Application.Features.Results.AdminListingResults;
using FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.AdminListingHandlers
{
    /// <summary>
    /// Belirli bir ilanın detay sorgusunu yetki kontrolü ile birlikte işleyen işleyici.
    /// </summary>
    public class GetAdminListingDetailQueryHandler : IRequestHandler<GetAdminListingDetailQuery, GetAdminListingDetailResult?>
    {
        private readonly IListingAccessService _listingAccessService;
        private readonly IAdminListingRepository _adminListingRepository;

        public GetAdminListingDetailQueryHandler(
            IListingAccessService listingAccessService,
            IAdminListingRepository adminListingRepository)
        {
            _listingAccessService = listingAccessService;
            _adminListingRepository = adminListingRepository;
        }

        public async Task<GetAdminListingDetailResult?> Handle(GetAdminListingDetailQuery request, CancellationToken cancellationToken)
        {
            // İlanın var olup olmadığını kontrol et
            var listingAgentId = await _adminListingRepository.GetListingAgentIdAsync(request.ListingTypeId, request.ListingId);
            if (!listingAgentId.HasValue)
            {
                return null;
            }

            // Erişim yetkisini denetle
            var access = await _listingAccessService.GetAccessAsync(request.ListingTypeId, request.ListingId);
            if (access == ListingAccess.None)
            {
                throw new ListingAccessDeniedException();
            }

            var detail = await _adminListingRepository.GetDetailAsync(request.ListingTypeId, request.ListingId);
            if (detail == null)
            {
                return null;
            }

            return new GetAdminListingDetailResult
            {
                Listing = detail,
                CanEdit = access == ListingAccess.Edit
            };
        }
    }
}
