using System.Collections.Generic;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.ListingImageCommands
{
    /// <summary>
    /// Bir ilana ait fotoğrafların görüntülenme sırasını ve kapak görselini güncelleme komutu.
    /// </summary>
    public class ReorderListingImagesCommand : IRequest
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
        /// Sıralanmış görsel kimlikleri listesi.
        /// </summary>
        public List<int> OrderedImageIds { get; set; } = new();

        /// <summary>
        /// Kapak / vitrin fotoğrafı olarak belirlenecek görsel kimliği.
        /// </summary>
        public int CoverImageId { get; set; }
    }
}
