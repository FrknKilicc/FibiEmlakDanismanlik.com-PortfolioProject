using FibiEmlakDanismanlik.Application.Features.Commands.ListingImageCommands;
using FibiEmlakDanismanlik.Application.Features.Results.ContentImageResults;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.ContentImageCommands
{
    /// <summary>
    /// İçerik görseli yükleme komutu.
    /// </summary>
    public class UploadContentImageCommand : IRequest<UploadContentImageResult>
    {
        /// <summary>
        /// Görselin yükleneceği hedef içerik klasörü (blog, banner, about, author, agent).
        /// </summary>
        public string Folder { get; set; } = string.Empty;

        /// <summary>
        /// Yüklenecek dosya bilgisi ve okuma akışı.
        /// </summary>
        public UploadedImageFile? File { get; set; }
    }
}
