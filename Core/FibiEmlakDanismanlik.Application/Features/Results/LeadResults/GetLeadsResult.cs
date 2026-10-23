using System.Collections.Generic;
using FibiEmlakDanismanlik.Application.Interfaces.LeadInterfaces;

namespace FibiEmlakDanismanlik.Application.Features.Results.LeadResults
{
    /// <summary>
    /// Yönetim paneli müşteri talepleri listeleme ve özet sorgu sonucu.
    /// </summary>
    public class GetLeadsResult
    {
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<LeadRow> Items { get; set; } = new List<LeadRow>();
        public LeadSummary Summary { get; set; } = new LeadSummary();
    }
}
