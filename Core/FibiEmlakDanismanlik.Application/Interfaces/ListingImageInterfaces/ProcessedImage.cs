using System;

namespace FibiEmlakDanismanlik.Application.Interfaces.ListingImageInterfaces
{
    /// <summary>
    /// İşlenmiş görselin boyut bilgilerini ve WebP formatındaki farklı çözünürlük çıktılarını temsil eder.
    /// </summary>
    public class ProcessedImage
    {
        /// <summary>
        /// En büyük boy görselin piksel cinsinden genişliği.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// En büyük boy görselin piksel cinsinden yüksekliği.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Büyük boy WebP görsel verisi (LargeWidth).
        /// </summary>
        public byte[] Large { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// Orta boy WebP görsel verisi (MediumWidth).
        /// </summary>
        public byte[] Medium { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// Küçük resim WebP görsel verisi (ThumbWidth).
        /// </summary>
        public byte[] Thumb { get; set; } = Array.Empty<byte>();
    }
}
