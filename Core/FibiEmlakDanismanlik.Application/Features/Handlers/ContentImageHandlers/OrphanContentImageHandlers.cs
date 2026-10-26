using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.ContentImageCommands;
using FibiEmlakDanismanlik.Application.Features.Queries.ContentImageQueries;
using FibiEmlakDanismanlik.Application.Features.Results.ContentImageResults;
using FibiEmlakDanismanlik.Application.Interfaces.ContentImageInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.ListingImageInterfaces;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ContentImageHandlers
{
    /// <summary>
    /// Kullanılmayan içerik görsellerinin listelenmesi ve silinmesi işlemlerini yürüten ortak işleyiciler.
    /// </summary>
    public static class OrphanContentImageHelper
    {
        /// <summary>
        /// Bir dosyanın yetim (sitede hiçbir kayıtta kullanılmayan) olup olmadığını doğrular.
        /// Dosya content/ altında olmalı, son yazılma zamanı 24 saatten eski olmalı ve dosya adı hiçbir referans metninde geçmemelidir.
        /// </summary>
        public static bool IsOrphan(StoredFileInfo file, IReadOnlyList<string> references, DateTime cutoffUtc)
        {
            if (string.IsNullOrWhiteSpace(file.RelativePath))
            {
                return false;
            }

            var normalized = file.RelativePath.Replace('\\', '/').TrimStart('/');

            // ASLA ilan veya kök dışı dosyalara dokunulamaz, yalnızca content/ altı
            if (!normalized.StartsWith("content/", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (normalized.Contains(".."))
            {
                return false;
            }

            // Yeni yüklenip henüz kaydedilmemiş form görsellerini silmemek için 24 saatten eski olmalı
            if (file.LastWriteUtc > cutoffUtc)
            {
                return false;
            }

            var fileName = Path.GetFileName(normalized);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }

            // Referans metinlerinin herhangi birinde dosya adı geçiyorsa yetim değildir
            for (var i = 0; i < references.Count; i++)
            {
                var refText = references[i];
                if (!string.IsNullOrEmpty(refText) && refText.IndexOf(fileName, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return false;
                }
            }

            return true;
        }
    }

    /// <summary>
    /// Sitede kullanılmayan içerik görsellerini listeleyen sorgu işleyicisi.
    /// </summary>
    public class GetOrphanContentImagesQueryHandler : IRequestHandler<GetOrphanContentImagesQuery, GetOrphanContentImagesResult>
    {
        private readonly IFileStorage _fileStorage;
        private readonly IContentImageUsageRepository _usageRepository;

        public GetOrphanContentImagesQueryHandler(
            IFileStorage fileStorage,
            IContentImageUsageRepository usageRepository)
        {
            _fileStorage = fileStorage;
            _usageRepository = usageRepository;
        }

        public async Task<GetOrphanContentImagesResult> Handle(GetOrphanContentImagesQuery request, CancellationToken cancellationToken)
        {
            var files = _fileStorage.ListFiles(ContentImageConstants.ContentFolder);
            var references = await _usageRepository.GetReferenceTextsAsync();
            var cutoffUtc = DateTime.UtcNow.AddHours(-24);

            var orphanItems = new List<OrphanContentImageItem>();
            long totalBytes = 0;

            foreach (var file in files)
            {
                if (OrphanContentImageHelper.IsOrphan(file, references, cutoffUtc))
                {
                    var relative = file.RelativePath.Replace('\\', '/').TrimStart('/');
                    orphanItems.Add(new OrphanContentImageItem
                    {
                        RelativePath = relative,
                        Url = _fileStorage.GetPublicUrl(relative),
                        SizeBytes = file.SizeBytes,
                        LastWriteUtc = file.LastWriteUtc
                    });
                    totalBytes += file.SizeBytes;
                }
            }

            // En son değiştirilenler başta olacak şekilde sıralanır
            orphanItems = orphanItems.OrderByDescending(x => x.LastWriteUtc).ToList();

            return new GetOrphanContentImagesResult
            {
                Items = orphanItems,
                TotalBytes = totalBytes,
                ScannedCount = files.Count
            };
        }
    }

    /// <summary>
    /// Seçilen kullanılmayan içerik görsellerini yeniden doğrulayarak silen komut işleyicisi.
    /// </summary>
    public class DeleteOrphanContentImagesCommandHandler : IRequestHandler<DeleteOrphanContentImagesCommand, int>
    {
        private const int MaxBatchSize = 500;
        private readonly IFileStorage _fileStorage;
        private readonly IContentImageUsageRepository _usageRepository;

        public DeleteOrphanContentImagesCommandHandler(
            IFileStorage fileStorage,
            IContentImageUsageRepository usageRepository)
        {
            _fileStorage = fileStorage;
            _usageRepository = usageRepository;
        }

        public async Task<int> Handle(DeleteOrphanContentImagesCommand request, CancellationToken cancellationToken)
        {
            if (request.RelativePaths == null || request.RelativePaths.Count == 0)
            {
                return 0;
            }

            // En fazla 500 yol kabul edilir
            var candidatePaths = request.RelativePaths.Take(MaxBatchSize).ToList();

            // Yeniden doğrulama için güncel dosya ve referans listesi taranır
            var files = _fileStorage.ListFiles(ContentImageConstants.ContentFolder);
            var references = await _usageRepository.GetReferenceTextsAsync();
            var cutoffUtc = DateTime.UtcNow.AddHours(-24);

            var fileMap = files.ToDictionary(
                f => f.RelativePath.Replace('\\', '/').TrimStart('/'),
                f => f,
                StringComparer.OrdinalIgnoreCase);

            var processed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var deletedCount = 0;

            foreach (var path in candidatePaths)
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    continue;
                }

                var normalized = path.Replace('\\', '/').TrimStart('/');

                // '..' içeren veya content/ ile başlamayan yollar reddedilir/atlanır
                if (normalized.Contains("..") || !normalized.StartsWith("content/", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!processed.Add(normalized))
                {
                    continue;
                }

                if (!fileMap.TryGetValue(normalized, out var storedFile))
                {
                    continue;
                }

                // Yalnızca hâlâ yetim olan dosyalar silinir
                if (OrphanContentImageHelper.IsOrphan(storedFile, references, cutoffUtc))
                {
                    await _fileStorage.DeleteAsync(storedFile.RelativePath);
                    deletedCount++;
                }
            }

            return deletedCount;
        }
    }
}
