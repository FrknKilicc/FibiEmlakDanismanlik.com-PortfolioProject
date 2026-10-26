using System;
using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Features.Results.ContentImageResults
{
    /// <summary>
    /// Kullanılmayan yetim içerik görseli bilgisi.
    /// </summary>
    public sealed class OrphanContentImageItem
    {
        public string RelativePath { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public long SizeBytes { get; set; }
        public DateTime LastWriteUtc { get; set; }
    }

    /// <summary>
    /// Kullanılmayan içerik görselleri sorgu sonucu.
    /// </summary>
    public sealed class GetOrphanContentImagesResult
    {
        public List<OrphanContentImageItem> Items { get; set; } = new();
        public long TotalBytes { get; set; }
        public int ScannedCount { get; set; }
    }
}
