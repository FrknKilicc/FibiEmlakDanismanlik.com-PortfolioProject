using System.Collections.Generic;
using FibiEmlakDanismanlik.Application.Interfaces.ListingAlertInterfaces;

namespace FibiEmlakDanismanlik.Application.Features.Results.ListingAlertResults
{
    /// <summary>
    /// Yönetim paneli haber bekleyenler listeleme ve özet sorgu sonucu.
    /// </summary>
    public class GetListingAlertsResult
    {
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<ListingAlertRow> Items { get; set; } = new List<ListingAlertRow>();
        public ListingAlertSummary Summary { get; set; } = new ListingAlertSummary();
    }
}
