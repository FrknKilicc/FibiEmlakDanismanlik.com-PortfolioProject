using System;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.ContentImageCommands;
using FibiEmlakDanismanlik.Application.Features.Results.ContentImageResults;
using FibiEmlakDanismanlik.Application.Interfaces.ListingImageInterfaces;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ContentImageHandlers
{
    /// <summary>
    /// İçerik görselini işleyen, optimize eden ve depolama alanına kaydeden komut işleyicisi.
    /// </summary>
    public class UploadContentImageCommandHandler : IRequestHandler<UploadContentImageCommand, UploadContentImageResult>
    {
        private readonly IImageProcessor _imageProcessor;
        private readonly IFileStorage _fileStorage;

        public UploadContentImageCommandHandler(
            IImageProcessor imageProcessor,
            IFileStorage fileStorage)
        {
            _imageProcessor = imageProcessor;
            _fileStorage = fileStorage;
        }

        public async Task<UploadContentImageResult> Handle(UploadContentImageCommand request, CancellationToken cancellationToken)
        {
            using var stream = request.File!.OpenReadStream();
            var processed = await _imageProcessor.ProcessAsync(stream, cancellationToken);

            var now = DateTime.UtcNow;
            var folder = request.Folder.Trim().ToLowerInvariant();
            var relativePath = $"{ContentImageConstants.ContentFolder}/{folder}/{now:yyyy}/{now:MM}/{Guid.NewGuid():N}-1600.webp";

            var url = await _fileStorage.SaveAsync(relativePath, processed.Large, cancellationToken);

            return new UploadContentImageResult
            {
                Url = url,
                Width = processed.Width,
                Height = processed.Height
            };
        }
    }
}
