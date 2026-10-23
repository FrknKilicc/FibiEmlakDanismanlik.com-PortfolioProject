using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Interfaces.ListingImageInterfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace FibiEmlakDanismanlik.Infrastructure.Storage
{
    /// <summary>
    /// Görsellerin yerel dosya sisteminde saklanmasını ve yol aşma (path traversal) korumasını sağlayan depolama servisi.
    /// </summary>
    public class LocalFileStorage : IFileStorage
    {
        private readonly string _rootPath;
        private readonly string _publicBaseUrl;

        public LocalFileStorage(IConfiguration configuration, IHostEnvironment environment)
        {
            var configuredRoot = configuration["FileStorage:RootPath"];
            _rootPath = string.IsNullOrWhiteSpace(configuredRoot)
                ? Path.Combine(environment.ContentRootPath, "uploads")
                : configuredRoot;

            _rootPath = Path.GetFullPath(_rootPath);

            var configuredUrl = configuration["FileStorage:PublicBaseUrl"];
            _publicBaseUrl = string.IsNullOrWhiteSpace(configuredUrl)
                ? string.Empty
                : configuredUrl.TrimEnd('/');
        }

        public async Task<string> SaveAsync(string relativePath, byte[] content, CancellationToken ct)
        {
            var fullPath = ValidateAndGetFullPath(relativePath);

            var directoryPath = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            await File.WriteAllBytesAsync(fullPath, content, ct);

            return GetPublicUrl(relativePath);
        }

        public Task DeleteAsync(string relativePath)
        {
            try
            {
                var fullPath = ValidateAndGetFullPath(relativePath);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            catch (ArgumentException)
            {
                // Geçersiz yol durumunda sessizce tamamlanır
            }
            catch (IOException)
            {
                // Dosya erişim veya kilit durumlarında sessizce tamamlanır
            }

            return Task.CompletedTask;
        }

        public string GetPublicUrl(string relativePath)
        {
            var cleanRelative = relativePath.TrimStart('/');
            if (string.IsNullOrWhiteSpace(_publicBaseUrl))
            {
                return $"{ListingImageConstants.UploadsRequestPath}/{cleanRelative}";
            }

            return $"{_publicBaseUrl}{ListingImageConstants.UploadsRequestPath}/{cleanRelative}";
        }

        private string ValidateAndGetFullPath(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                throw new ArgumentException("Göreli dosya yolu boş olamaz.", nameof(relativePath));
            }

            if (relativePath.Contains(".."))
            {
                throw new ArgumentException("Göreli yolda '..' bulunamaz.", nameof(relativePath));
            }

            if (relativePath.Contains('\\'))
            {
                throw new ArgumentException("Göreli yolda ters eğik çizgi ('\\') bulunamaz.", nameof(relativePath));
            }

            if (Path.IsPathRooted(relativePath) || relativePath.StartsWith("/"))
            {
                throw new ArgumentException("Göreli yol mutlak yol veya kök eğik çizgisi ile başlayamaz.", nameof(relativePath));
            }

            if (relativePath.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                throw new ArgumentException("Göreli yolda geçersiz karakterler bulunmaktadır.", nameof(relativePath));
            }

            var normalizedRelative = relativePath.Replace('/', Path.DirectorySeparatorChar);
            var combinedPath = Path.GetFullPath(Path.Combine(_rootPath, normalizedRelative));

            var separator = Path.DirectorySeparatorChar.ToString();
            var rootWithTrailing = _rootPath.EndsWith(separator) ? _rootPath : _rootPath + separator;

            if (!combinedPath.StartsWith(rootWithTrailing, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Dosya yolu kök depolama alanının dışına çıkamaz.", nameof(relativePath));
            }

            return combinedPath;
        }
    }
}
