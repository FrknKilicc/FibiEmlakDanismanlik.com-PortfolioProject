using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.ListingImageCommands
{
    /// <summary>
    /// İlan görselinin türünü ve başlığını güncelleme komutu.
    /// </summary>
    public class UpdateListingImageKindCommand : IRequest<bool>
    {
        /// <summary>
        /// Güncellenecek görsel kimliği.
        /// </summary>
        public int ListingImageId { get; set; }

        /// <summary>
        /// Görsel türü (0: Fotoğraf, 1: Kat Planı, 2: Kroki, 3: İmar Planı).
        /// </summary>
        public int Kind { get; set; }

        /// <summary>
        /// Plan veya görsel başlığı.
        /// </summary>
        public string? Caption { get; set; }

        public UpdateListingImageKindCommand()
        {
        }

        public UpdateListingImageKindCommand(int listingImageId, int kind, string? caption)
        {
            ListingImageId = listingImageId;
            Kind = kind;
            Caption = caption;
        }
    }
}
