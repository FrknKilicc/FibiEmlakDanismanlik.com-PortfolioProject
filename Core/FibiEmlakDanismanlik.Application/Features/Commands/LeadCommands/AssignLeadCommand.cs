using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.LeadCommands
{
    /// <summary>
    /// Müşteri talebini bir danışmana atayan veya atamasını kaldıran komut.
    /// </summary>
    public class AssignLeadCommand : IRequest<bool>
    {
        public LeadSource Source { get; set; }
        public int LeadId { get; set; }
        public int? AgentId { get; set; }

        public AssignLeadCommand()
        {
        }

        public AssignLeadCommand(LeadSource source, int leadId, int? agentId)
        {
            Source = source;
            LeadId = leadId;
            AgentId = agentId;
        }
    }
}
