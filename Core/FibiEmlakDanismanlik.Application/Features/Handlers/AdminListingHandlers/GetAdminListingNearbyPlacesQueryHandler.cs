using System.Collections.Generic;
using System.Linq;
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
    /// İlanın yakın yer kayıtlarını yetki denetimiyle getiren işleyici.
    /// </summary>
    public class GetAdminListingNearbyPlacesQueryHandler : IRequestHandler<GetAdminListingNearbyPlacesQuery, GetAdminListingNearbyPlacesResult>
    {
        private readonly IListingAccessService _listingAccessService;
        private readonly IAdminListingRepository _adminListingRepository;

        public GetAdminListingNearbyPlacesQueryHandler(
            IListingAccessService listingAccessService,
            IAdminListingRepository adminListingRepository)
        {
            _listingAccessService = listingAccessService;
            _adminListingRepository = adminListingRepository;
        }

        public async Task<GetAdminListingNearbyPlacesResult> Handle(GetAdminListingNearbyPlacesQuery request, CancellationToken cancellationToken)
        {
            var agentId = await _adminListingRepository.GetListingAgentIdAsync(request.ListingTypeId, request.ListingId);
            if (!agentId.HasValue)
            {
                throw new KeyNotFoundException("İlan bulunamadı.");
            }

            var access = await _listingAccessService.GetAccessAsync(request.ListingTypeId, request.ListingId);
            if (access == ListingAccess.None)
            {
                throw new ListingAccessDeniedException("Bu ilanı görüntüleme yetkiniz bulunmamaktadır.");
            }

            var (categories, places) = await _adminListingRepository.GetNearbyPlacesAsync(request.ListingTypeId, request.ListingId);

            return new GetAdminListingNearbyPlacesResult
            {
                CanEdit = access == ListingAccess.Edit,
                Categories = categories.Select(c => new NearbyCategoryItem
                {
                    Id = c.Id,
                    Name = c.Name,
                    IconCss = c.IconCss
                }).ToList(),
                Places = places.Select(p => new NearbyPlaceItem
                {
                    Id = p.Id,
                    NearbyCategoryId = p.NearbyCategoryId,
                    CategoryName = p.NearbyCategory?.Name,
                    PlaceName = p.PlaceName,
                    DistanceKm = p.DistanceKm,
                    IsActive = p.IsActive
                }).ToList()
            };
        }
    }
}
