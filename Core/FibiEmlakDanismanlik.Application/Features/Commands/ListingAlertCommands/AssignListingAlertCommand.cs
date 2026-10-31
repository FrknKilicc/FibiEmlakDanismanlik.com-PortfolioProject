using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.ListingAlertCommands
{
    /// <summary>
    /// Haber talebini bir danışmana atayan veya atamasını kaldıran komut.
    /// </summary>
    public class AssignListingAlertCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public int? AgentId { get; set; }

        public AssignListingAlertCommand()
        {
        }

        public AssignListingAlertCommand(int id, int? agentId)
        {
            Id = id;
            AgentId = agentId;
        }
    }
}
