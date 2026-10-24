using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Commands.ListingImageCommands;
using FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.ListingImageInterfaces;
using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ListingImageHandlers
{
    /// <summary>
    /// İlan görselinin türünü ve başlığını güncelleyen komut işleyicisi.
    /// </summary>
    public class UpdateListingImageKindCommandHandler : IRequestHandler<UpdateListingImageKindCommand, bool>
    {
        private readonly IListingImageRepository _listingImageRepository;
        private readonly IListingAccessService _listingAccessService;

        public UpdateListingImageKindCommandHandler(
            IListingImageRepository listingImageRepository,
            IListingAccessService listingAccessService)
        {
            _listingImageRepository = listingImageRepository;
            _listingAccessService = listingAccessService;
        }

        public async Task<bool> Handle(UpdateListingImageKindCommand request, CancellationToken cancellationToken)
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
                throw new ListingAccessDeniedException("Bu ilanın fotoğraflarını düzenleme yetkiniz bulunmamaktadır.");
            }

            await _listingImageRepository.UpdateKindAsync(request.ListingImageId, (ListingImageKind)request.Kind, request.Caption);

            await _listingImageRepository.NormalizeAsync(listingTypeId, listingId);
            await _listingImageRepository.SyncLegacyColumnsAsync(listingTypeId, listingId);

            return true;
        }
    }
}
