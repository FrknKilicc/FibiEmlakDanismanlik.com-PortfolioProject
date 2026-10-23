using FibiEmlakDanismanlik.Application.Interfaces.LeadInterfaces;
using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Queries.LeadQueries
{
    /// <summary>
    /// Müşteri talebi ayrıntılı görünümünü ve hareket geçmişini getiren sorgu.
    /// </summary>
    public class GetLeadDetailQuery : IRequest<LeadDetail?>
    {
        public LeadSource Source { get; set; }
        public int LeadId { get; set; }

        public GetLeadDetailQuery()
        {
        }

        public GetLeadDetailQuery(LeadSource source, int leadId)
        {
            Source = source;
            LeadId = leadId;
        }
    }
}
