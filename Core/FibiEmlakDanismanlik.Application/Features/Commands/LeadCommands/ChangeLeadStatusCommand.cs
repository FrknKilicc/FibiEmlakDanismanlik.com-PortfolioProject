using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.LeadCommands
{
    /// <summary>
    /// Müşteri talebinin durumunu güncelleyen ve isteğe bağlı açıklama/not ekleyen komut.
    /// </summary>
    public class ChangeLeadStatusCommand : IRequest<bool>
    {
        public LeadSource Source { get; set; }
        public int LeadId { get; set; }
        public LeadStatus Status { get; set; }
        public string? Note { get; set; }

        public ChangeLeadStatusCommand()
        {
        }

        public ChangeLeadStatusCommand(LeadSource source, int leadId, LeadStatus status, string? note)
        {
            Source = source;
            LeadId = leadId;
            Status = status;
            Note = note;
        }
    }
}
