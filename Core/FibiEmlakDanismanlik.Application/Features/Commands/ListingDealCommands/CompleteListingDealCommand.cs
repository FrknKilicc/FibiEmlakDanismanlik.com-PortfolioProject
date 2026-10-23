using System;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.ListingDealCommands
{
    /// <summary>
    /// Kapora aşamasındaki işlemi tamamlama komutu.
    /// </summary>
    public class CompleteListingDealCommand : IRequest<bool>
    {
        public int ListingDealId { get; set; }
        public DateTime? CompletedDate { get; set; }

        public CompleteListingDealCommand()
        {
        }

        public CompleteListingDealCommand(int listingDealId, DateTime? completedDate = null)
        {
            ListingDealId = listingDealId;
            CompletedDate = completedDate;
        }
    }
}
