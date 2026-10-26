using System;
using System.Collections.Generic;

namespace FibiEmlakDanismanlik.AdminUI.Models.Settings
{
    /// <summary>
    /// API'den dönen yetim görsel öğesi modeli.
    /// </summary>
    public sealed class OrphanImageItemDto
    {
        public string RelativePath { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public long SizeBytes { get; set; }
        public DateTime LastWriteUtc { get; set; }
    }

    /// <summary>
    /// API yetim görseller sorgusu yanıt modeli.
    /// </summary>
    public sealed class GetOrphanContentImagesResultDto
    {
        public List<OrphanImageItemDto> Items { get; set; } = new();
        public long TotalBytes { get; set; }
        public int ScannedCount { get; set; }
    }

    /// <summary>
    /// API yetim görselleri silme yanıt modeli.
    /// </summary>
    public sealed class DeleteOrphansResponseDto
    {
        public int Deleted { get; set; }
    }

    /// <summary>
    /// Kullanılmayan görseller yönetim sayfası görünüm modeli.
    /// </summary>
    public sealed class OrphanImagesViewModel
    {
        public List<OrphanImageItemDto> Items { get; set; } = new();
        public long TotalBytes { get; set; }
        public int ScannedCount { get; set; }
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Toplam boyutu megabayt cinsinden tek ondalık basamakla döndürür.
        /// </summary>
        public double TotalMegaBytes => Math.Round((double)TotalBytes / (1024 * 1024), 1);
    }
}
