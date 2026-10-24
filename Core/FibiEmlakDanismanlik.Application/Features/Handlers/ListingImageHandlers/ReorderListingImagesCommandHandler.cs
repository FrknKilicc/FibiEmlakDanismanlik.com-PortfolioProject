using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Commands.ListingImageCommands;
using FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.ListingImageInterfaces;
using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ListingImageHandlers
{
    /// <summary>
    /// Bir ilana ait görsellerin sırasını ve kapak fotoğrafını güncelleyen komut işleyicisi.
    /// </summary>
    public class ReorderListingImagesCommandHandler : IRequestHandler<ReorderListingImagesCommand>
    {
        private readonly IListingImageRepository _listingImageRepository;
        private readonly IListingAccessService _listingAccessService;

        public ReorderListingImagesCommandHandler(
            IListingImageRepository listingImageRepository,
            IListingAccessService listingAccessService)
        {
            _listingImageRepository = listingImageRepository;
            _listingAccessService = listingAccessService;
        }

        public async Task Handle(ReorderListingImagesCommand request, CancellationToken cancellationToken)
        {
            if (!ListingImageConstants.IsKnownListingType(request.ListingTypeId) ||
                !await _listingImageRepository.ListingExistsAsync(request.ListingTypeId, request.ListingId))
            {
                throw new ImageValidationException("İlan bulunamadı.");
            }

            var access = await _listingAccessService.GetAccessAsync(request.ListingTypeId, request.ListingId);
            if (access != ListingAccess.Edit)
            {
                throw new ListingAccessDeniedException("Bu ilanın fotoğraflarını sıralama yetkiniz bulunmamaktadır.");
            }

            if (request.OrderedImageIds == null || request.OrderedImageIds.Count == 0)
            {
                throw new ImageValidationException("Sıralanacak fotoğraf listesi boş olamaz.");
            }

            var existingImages = await _listingImageRepository.GetByListingAsync(request.ListingTypeId, request.ListingId);
            var existingIds = existingImages.Select(x => x.ListingImageId).ToHashSet();

            if (request.OrderedImageIds.Any(id => !existingIds.Contains(id)))
            {
                throw new ImageValidationException("Sıralamadaki fotoğrafların tümü bu ilana ait olmalıdır.");
            }

            if (!existingIds.Contains(request.CoverImageId))
            {
                throw new ImageValidationException("Kapak fotoğrafı bu ilana ait olmalıdır.");
            }

            var coverImage = existingImages.FirstOrDefault(x => x.ListingImageId == request.CoverImageId);
            if (coverImage != null && coverImage.Kind != ListingImageKind.Photo)
            {
                throw new ImageValidationException("Kapak olarak yalnızca fotoğraf seçilebilir; kat planı, kroki veya imar planı kapak olamaz.");
            }

            await _listingImageRepository.SaveOrderAsync(
                request.ListingTypeId,
                request.ListingId,
                request.OrderedImageIds,
                request.CoverImageId);

            await _listingImageRepository.NormalizeAsync(request.ListingTypeId, request.ListingId);

            await _listingImageRepository.SyncLegacyColumnsAsync(request.ListingTypeId, request.ListingId);
        }
    }
}
