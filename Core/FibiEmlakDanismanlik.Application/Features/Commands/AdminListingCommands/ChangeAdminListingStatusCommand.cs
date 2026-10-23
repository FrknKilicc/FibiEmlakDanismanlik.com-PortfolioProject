using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.AdminListingCommands
{
    /// <summary>
    /// Yönetim panelinde ilan durumunu (Aktif, Opsiyonlu, Pasif, Satıldı, Kiralandı) değiştirme komutu.
    /// </summary>
    public class ChangeAdminListingStatusCommand : IRequest<bool>
    {
        public int ListingTypeId { get; set; }
        public int ListingId { get; set; }
        public string Status { get; set; } = string.Empty;

        public ChangeAdminListingStatusCommand()
        {
        }

        public ChangeAdminListingStatusCommand(int listingTypeId, int listingId, string status)
        {
            ListingTypeId = listingTypeId;
            ListingId = listingId;
            Status = status;
        }
    }
}
