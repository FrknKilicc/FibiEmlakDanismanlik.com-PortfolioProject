using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.AdminListingCommands
{
    /// <summary>
    /// Bir ilana danışman için özel görüntüleme veya düzenleme izni verme veya güncelleme komutu (Yalnızca Sahip/Yönetici).
    /// </summary>
    public class AddAdminListingGrantCommand : IRequest<bool>
    {
        public int ListingTypeId { get; set; }
        public int ListingId { get; set; }
        public int AgentId { get; set; }
        public string Access { get; set; } = string.Empty; // "View" veya "Edit"

        public AddAdminListingGrantCommand()
        {
        }

        public AddAdminListingGrantCommand(int listingTypeId, int listingId, int agentId, string access)
        {
            ListingTypeId = listingTypeId;
            ListingId = listingId;
            AgentId = agentId;
            Access = access;
        }
    }
}
