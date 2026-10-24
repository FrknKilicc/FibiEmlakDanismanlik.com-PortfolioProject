namespace FibiEmlakDanismanlik.Application.Features.Results.ContentImageResults
{
    /// <summary>
    /// İçerik görseli yükleme işlemi sonucunda üretilen erişim ve boyut bilgileri.
    /// </summary>
    public class UploadContentImageResult
    {
        /// <summary>
        /// Yüklenen görselin genel erişim URL adresi.
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Görselin piksel cinsinden genişliği.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Görselin piksel cinsinden yüksekliği.
        /// </summary>
        public int Height { get; set; }
    }
}
