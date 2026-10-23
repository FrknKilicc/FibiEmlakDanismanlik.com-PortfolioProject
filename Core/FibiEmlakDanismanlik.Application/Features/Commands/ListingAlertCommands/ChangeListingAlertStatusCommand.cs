using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.ListingAlertCommands
{
    /// <summary>
    /// Haber talebinin takip durumunu güncelleyen komut.
    /// </summary>
    public class ChangeListingAlertStatusCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public ListingAlertStatus Status { get; set; }

        public ChangeListingAlertStatusCommand()
        {
        }

        public ChangeListingAlertStatusCommand(int id, ListingAlertStatus status)
        {
            Id = id;
            Status = status;
        }
    }
}
