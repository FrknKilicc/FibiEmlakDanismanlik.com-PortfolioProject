using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Commands.ListingImageCommands;
using FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.ListingImageInterfaces;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ListingImageHandlers
{
    /// <summary>
    /// İlan görselini veritabanından ve diskten silen, gerekirse kapak görselini yeniden atayan komut işleyicisi.
    /// </summary>
    public class RemoveListingImageCommandHandler : IRequestHandler<RemoveListingImageCommand, bool>
    {
        private readonly IListingImageRepository _listingImageRepository;
        private readonly IFileStorage _fileStorage;
        private readonly IListingAccessService _listingAccessService;

        public RemoveListingImageCommandHandler(
            IListingImageRepository listingImageRepository,
            IFileStorage fileStorage,
            IListingAccessService listingAccessService)
        {
            _listingImageRepository = listingImageRepository;
            _fileStorage = fileStorage;
            _listingAccessService = listingAccessService;
        }

        public async Task<bool> Handle(RemoveListingImageCommand request, CancellationToken cancellationToken)
        {
            var image = await _listingImageRepository.GetByIdAsync(request.ListingImageId);
            if (image == null)
            {
                return false;
            }

            int listingTypeId = image.ListingTypeId;
            int listingId = image.ListingId;

            var access = await _listingAccessService.GetAccessAsync(listingTypeId, listingId);
            if (access != ListingAccess.Edit)
            {
                throw new ListingAccessDeniedException("Bu ilanın fotoğrafını silme yetkiniz bulunmamaktadır.");
            }
            bool wasCover = image.IsCover;
            string? storagePath = image.StoragePath;

            await _listingImageRepository.RemoveAsync(image);

            if (!string.IsNullOrWhiteSpace(storagePath))
            {
                await _fileStorage.DeleteAsync($"{storagePath}-1600.webp");
                await _fileStorage.DeleteAsync($"{storagePath}-800.webp");
                await _fileStorage.DeleteAsync($"{storagePath}-400.webp");
            }

            var remainingImages = await _listingImageRepository.GetByListingAsync(listingTypeId, listingId);
            if (wasCover && remainingImages.Count > 0)
            {
                var firstRemaining = remainingImages[0];
                var orderedIds = remainingImages.Select(x => x.ListingImageId).ToList();
                await _listingImageRepository.SaveOrderAsync(listingTypeId, listingId, orderedIds, firstRemaining.ListingImageId);
            }

            await _listingImageRepository.SyncLegacyColumnsAsync(listingTypeId, listingId);

            return true;
        }
    }
}
