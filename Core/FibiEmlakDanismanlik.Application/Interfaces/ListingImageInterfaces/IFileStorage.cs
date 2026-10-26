using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Interfaces.ListingImageInterfaces
{
    /// <summary>
    /// Depolanan dosyanın göreli yolu, boyutu ve son yazılma zamanı bilgisi.
    /// </summary>
    /// <param name="RelativePath">Kök depolamaya göre '/' ayraçlı dosya yolu (ör. content/blog/2026/10/x-1600.webp).</param>
    /// <param name="SizeBytes">Dosyanın bayt cinsinden boyutu.</param>
    /// <param name="LastWriteUtc">Dosyanın son yazılma UTC zamanı.</param>
    public sealed record StoredFileInfo(string RelativePath, long SizeBytes, DateTime LastWriteUtc);

    /// <summary>
    /// Dosyaların disk veya depolama alanına kaydedilmesi, silinmesi ve genel adreslerinin üretilmesi sözleşmesi.
    /// </summary>
    public interface IFileStorage
    {
        /// <summary>
        /// Dosya içeriğini belirtilen göreli yola kaydeder ve genel erişim adresini döndürür.
        /// </summary>
        /// <param name="relativePath">Kök depolama alanına göre göreli dosya yolu.</param>
        /// <param name="content">Dosya bayt verisi.</param>
        /// <param name="ct">İptal belirteci.</param>
        /// <returns>Dosyanın genel erişim URL adresi.</returns>
        Task<string> SaveAsync(string relativePath, byte[] content, CancellationToken ct);

        /// <summary>
        /// Belirtilen göreli yoldaki dosyayı siler. Dosya bulunamazsa sessizce tamamlanır.
        /// </summary>
        /// <param name="relativePath">Silinecek dosyanın göreli yolu.</param>
        Task DeleteAsync(string relativePath);

        /// <summary>
        /// Verilen göreli dosya yolu için tam genel erişim URL adresini üretir.
        /// </summary>
        /// <param name="relativePath">Göreli dosya yolu.</param>
        /// <returns>Genel erişim URL adresi.</returns>
        string GetPublicUrl(string relativePath);

        /// <summary>
        /// Belirtilen göreli klasör altındaki tüm dosyaları özyinelemeli olarak listeler.
        /// </summary>
        /// <param name="relativeFolder">Kök depolama alanına göre listelenecek göreli klasör yolu.</param>
        /// <returns>Bulunan dosyaların listesi.</returns>
        IReadOnlyList<StoredFileInfo> ListFiles(string relativeFolder);
    }
}
