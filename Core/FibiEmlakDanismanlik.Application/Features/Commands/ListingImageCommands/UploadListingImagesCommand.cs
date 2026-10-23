using System.Collections.Generic;
using FibiEmlakDanismanlik.Application.Features.Results.ListingImageResults;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.ListingImageCommands
{
    /// <summary>
    /// Belirtilen ilana bir veya birden fazla yeni görsel yükleme komutu.
    /// </summary>
    public class UploadListingImagesCommand : IRequest<List<GetListingImageResult>>
    {
        /// <summary>
        /// İlan türü kimliği.
        /// </summary>
        public int ListingTypeId { get; set; }

        /// <summary>
        /// İlan kimliği.
        /// </summary>
        public int ListingId { get; set; }

        /// <summary>
        /// Yüklenecek dosya koleksiyonu.
        /// </summary>
        public List<UploadedImageFile> Files { get; set; } = new();
    }
}
