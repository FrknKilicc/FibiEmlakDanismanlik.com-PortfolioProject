using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Interfaces.ListingImageInterfaces
{
    /// <summary>
    /// Görsellerin doğrulanması, EXIF yönünün uygulanması ve farklı WebP boyutlarında yeniden kodlanması sözleşmesi.
    /// </summary>
    public interface IImageProcessor
    {
        /// <summary>
        /// Verilen görsel akışını işleyerek üç farklı boyutta WebP çıktısı üretir.
        /// Geçersiz veya desteklenmeyen dosyalarda ImageValidationException fırlatır.
        /// </summary>
        /// <param name="input">Görsel dosya akışı.</param>
        /// <param name="ct">İptal belirteci.</param>
        /// <returns>İşlenmiş görsel boyutları ve WebP bayt dizileri.</returns>
        Task<ProcessedImage> ProcessAsync(Stream input, CancellationToken ct);
    }
}
