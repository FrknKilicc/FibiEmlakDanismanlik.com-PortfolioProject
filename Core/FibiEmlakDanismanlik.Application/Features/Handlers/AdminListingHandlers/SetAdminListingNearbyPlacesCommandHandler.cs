using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Commands.AdminListingCommands;
using FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.AdminListingHandlers
{
    /// <summary>
    /// İlanın yakın yer listesini yetki denetimiyle değiştiren işleyici.
    /// </summary>
    public class SetAdminListingNearbyPlacesCommandHandler : IRequestHandler<SetAdminListingNearbyPlacesCommand, bool>
    {
        private readonly IListingAccessService _listingAccessService;
        private readonly IAdminListingRepository _adminListingRepository;

        public SetAdminListingNearbyPlacesCommandHandler(
            IListingAccessService listingAccessService,
            IAdminListingRepository adminListingRepository)
        {
            _listingAccessService = listingAccessService;
            _adminListingRepository = adminListingRepository;
        }

        public async Task<bool> Handle(SetAdminListingNearbyPlacesCommand request, CancellationToken cancellationToken)
        {
            var agentId = await _adminListingRepository.GetListingAgentIdAsync(request.ListingTypeId, request.ListingId);
            if (!agentId.HasValue)
            {
                throw new KeyNotFoundException("İlan bulunamadı.");
            }

            var access = await _listingAccessService.GetAccessAsync(request.ListingTypeId, request.ListingId);
            if (access != ListingAccess.Edit)
            {
                throw new ListingAccessDeniedException("Bu ilanın yakın yerlerini düzenleme yetkiniz bulunmamaktadır.");
            }

            var (categories, _) = await _adminListingRepository.GetNearbyPlacesAsync(request.ListingTypeId, request.ListingId);
            var activeCategoryIds = categories.Select(c => c.Id).ToHashSet();
            if (request.Places.Any(p => !activeCategoryIds.Contains(p.NearbyCategoryId)))
            {
                throw new ValidationException(new[] { new ValidationFailure("Places", "Geçersiz yakın yer kategorisi.") });
            }

            // Aynı kategori + ad ikinci kez gelirse benzersiz index'e takılmasın, ilki kalır
            var places = request.Places
                .Select(p => (p.NearbyCategoryId, PlaceName: p.PlaceName.Trim(), p.DistanceKm))
                .GroupBy(p => (p.NearbyCategoryId, Key: p.PlaceName.ToUpperInvariant()))
                .Select(g => g.First())
                .ToList();

            return await _adminListingRepository.ReplaceNearbyPlacesAsync(request.ListingTypeId, request.ListingId, places);
        }
    }
}
