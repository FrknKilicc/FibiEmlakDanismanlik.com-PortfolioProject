using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.ListingAlertCommands
{
    /// <summary>
    /// Haber talebi ile eşleşen bir ilan için bildirim veya eleme durumunu kaydeden komut.
    /// </summary>
    public class MarkListingAlertMatchCommand : IRequest<bool>
    {
        public int AlertId { get; set; }
        public int ListingTypeId { get; set; }
        public int ListingId { get; set; }
        public ListingAlertMatchStatus Status { get; set; }

        public MarkListingAlertMatchCommand()
        {
        }

        public MarkListingAlertMatchCommand(int alertId, int listingTypeId, int listingId, ListingAlertMatchStatus status)
        {
            AlertId = alertId;
            ListingTypeId = listingTypeId;
            ListingId = listingId;
            Status = status;
        }
    }
}
