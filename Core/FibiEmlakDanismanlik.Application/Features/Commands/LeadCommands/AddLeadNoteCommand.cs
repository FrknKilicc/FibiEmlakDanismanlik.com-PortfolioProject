using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.LeadCommands
{
    /// <summary>
    /// Müşteri talebine danışman notu veya görüşme/arama kaydı ekleyen komut.
    /// </summary>
    public class AddLeadNoteCommand : IRequest<bool>
    {
        public LeadSource Source { get; set; }
        public int LeadId { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsCall { get; set; }

        public AddLeadNoteCommand()
        {
        }

        public AddLeadNoteCommand(LeadSource source, int leadId, string text, bool isCall)
        {
            Source = source;
            LeadId = leadId;
            Text = text;
            IsCall = isCall;
        }
    }
}
