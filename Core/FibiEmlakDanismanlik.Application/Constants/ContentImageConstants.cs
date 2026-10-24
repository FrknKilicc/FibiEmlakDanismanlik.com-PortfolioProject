using System;
using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Constants
{
    /// <summary>
    /// İçerik görselleri (blog, banner, hakkımızda, yazar, danışman) ve depolama klasörleri için sabit tanımlar.
    /// </summary>
    public static class ContentImageConstants
    {
        /// <summary>
        /// İçerik görsellerinin saklandığı ana klasör adı.
        /// </summary>
        public const string ContentFolder = "content";

        /// <summary>
        /// İçerik görseli yüklenmesine izin verilen alt klasör adları.
        /// </summary>
        public static readonly IReadOnlyCollection<string> AllowedFolders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "blog",
            "banner",
            "about",
            "author",
            "agent"
        };

        /// <summary>
        /// Belirtilen klasör adının izin verilen içerik klasörleri arasında olup olmadığını doğrular.
        /// </summary>
        /// <param name="folder">Doğrulanacak klasör adı.</param>
        /// <returns>Klasör izin verilenler listesindeyse true, aksi halde false.</returns>
        public static bool IsAllowedFolder(string? folder)
        {
            return !string.IsNullOrWhiteSpace(folder) && AllowedFolders.Contains(folder);
        }
    }
}
