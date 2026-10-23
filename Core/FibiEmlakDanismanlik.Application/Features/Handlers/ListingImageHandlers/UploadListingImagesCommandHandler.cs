using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Commands.ListingImageCommands;
using FibiEmlakDanismanlik.Application.Features.Results.ListingImageResults;
using FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.ListingImageInterfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ListingImageHandlers
{
    /// <summary>
    /// İlan fotoğraflarını doğrulayan, boyutlandıran, depolayan ve veritabanı ile senkronize eden komut işleyicisi.
    /// </summary>
    public class UploadListingImagesCommandHandler : IRequestHandler<UploadListingImagesCommand, List<GetListingImageResult>>
    {
        private readonly IListingImageRepository _listingImageRepository;
        private readonly IImageProcessor _imageProcessor;
        private readonly IFileStorage _fileStorage;
        private readonly IListingAccessService _listingAccessService;

        public UploadListingImagesCommandHandler(
            IListingImageRepository listingImageRepository,
            IImageProcessor imageProcessor,
            IFileStorage fileStorage,
            IListingAccessService listingAccessService)
        {
            _listingImageRepository = listingImageRepository;
            _imageProcessor = imageProcessor;
            _fileStorage = fileStorage;
            _listingAccessService = listingAccessService;
        }

        public async Task<List<GetListingImageResult>> Handle(UploadListingImagesCommand request, CancellationToken cancellationToken)
        {
            if (!ListingImageConstants.IsKnownListingType(request.ListingTypeId) ||
                !await _listingImageRepository.ListingExistsAsync(request.ListingTypeId, request.ListingId))
            {
                throw new ImageValidationException("İlan bulunamadı.");
            }

            var access = await _listingAccessService.GetAccessAsync(request.ListingTypeId, request.ListingId);
            if (access != ListingAccess.Edit)
            {
                throw new ListingAccessDeniedException("Bu ilana fotoğraf yükleme yetkiniz bulunmamaktadır.");
            }

            var existingImages = await _listingImageRepository.GetByListingAsync(request.ListingTypeId, request.ListingId);

            if (existingImages.Count + request.Files.Count > ListingImageConstants.MaxImagesPerListing)
            {
                throw new ImageValidationException("Bir ilana en fazla 30 fotoğraf eklenebilir.");
            }

            var writtenFiles = new List<string>();
            var newEntities = new List<ListingImage>();

            int currentSortOrder = existingImages.Count > 0 ? existingImages.Max(x => x.SortOrder) : 0;
            bool isFirst = existingImages.Count == 0;

            try
            {
                foreach (var file in request.Files)
                {
                    if (file.OpenReadStream == null)
                    {
                        throw new ImageValidationException("Dosya okuma akışı alınamadı.");
                    }

                    using var stream = file.OpenReadStream();
                    var processed = await _imageProcessor.ProcessAsync(stream, cancellationToken);

                    var now = DateTime.UtcNow;
                    var relativePrefix = $"{ListingImageConstants.ListingsFolder}/{now:yyyy}/{now:MM}/{Guid.NewGuid():N}";
                    var largePath = $"{relativePrefix}-1600.webp";
                    var mediumPath = $"{relativePrefix}-800.webp";
                    var thumbPath = $"{relativePrefix}-400.webp";

                    var largeUrl = await _fileStorage.SaveAsync(largePath, processed.Large, cancellationToken);
                    writtenFiles.Add(largePath);

                    var mediumUrl = await _fileStorage.SaveAsync(mediumPath, processed.Medium, cancellationToken);
                    writtenFiles.Add(mediumPath);

                    var thumbUrl = await _fileStorage.SaveAsync(thumbPath, processed.Thumb, cancellationToken);
                    writtenFiles.Add(thumbPath);

                    currentSortOrder++;
                    bool isCover = false;
                    if (isFirst)
                    {
                        isCover = true;
                        isFirst = false;
                    }

                    var entity = new ListingImage
                    {
                        ListingTypeId = request.ListingTypeId,
                        ListingId = request.ListingId,
                        Url = largeUrl,
                        MediumUrl = mediumUrl,
                        ThumbUrl = thumbUrl,
                        Width = processed.Width,
                        Height = processed.Height,
                        SortOrder = currentSortOrder,
                        IsCover = isCover,
                        StoragePath = relativePrefix,
                        CreatedAtUtc = now
                    };

                    newEntities.Add(entity);
                }

                await _listingImageRepository.AddRangeAsync(newEntities);
                await _listingImageRepository.SyncLegacyColumnsAsync(request.ListingTypeId, request.ListingId);
            }
            catch (Exception)
            {
                foreach (var path in writtenFiles)
                {
                    try
                    {
                        await _fileStorage.DeleteAsync(path);
                    }
                    catch
                    {
                        // Hata durumunda yazılmış dosyalar temizlenirken oluşan hatalar yutulur
                    }
                }
                throw;
            }

            var updatedImages = await _listingImageRepository.GetByListingAsync(request.ListingTypeId, request.ListingId);

            return updatedImages
                .OrderByDescending(x => x.IsCover)
                .ThenBy(x => x.SortOrder)
                .Select(x => new GetListingImageResult
                {
                    ListingImageId = x.ListingImageId,
                    Url = x.Url,
                    MediumUrl = x.MediumUrl,
                    ThumbUrl = x.ThumbUrl,
                    Width = x.Width,
                    Height = x.Height,
                    AltText = x.AltText,
                    IsCover = x.IsCover,
                    SortOrder = x.SortOrder
                })
                .ToList();
        }
    }
}
