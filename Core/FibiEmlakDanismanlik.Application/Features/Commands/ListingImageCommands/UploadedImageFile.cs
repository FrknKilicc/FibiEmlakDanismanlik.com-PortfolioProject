using System;
using System.IO;

namespace FibiEmlakDanismanlik.Application.Features.Commands.ListingImageCommands
{
    /// <summary>
    /// Application katmanının HTTP veya IFormFile bağımlılığı olmadan yüklenen dosyayı taşımasını sağlayan veri yapısı.
    /// </summary>
    public class UploadedImageFile
    {
        /// <summary>
        /// Orijinal dosya adı.
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Dosya boyutu (bayt).
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        /// Dosyanın okunabilir veri akışını açan temsilci.
        /// </summary>
        public Func<Stream> OpenReadStream { get; set; } = default!;
    }
}
