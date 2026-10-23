using System.Collections.Generic;
using FibiEmlakDanismanlik.Application.Interfaces.ListingDealInterfaces;

namespace FibiEmlakDanismanlik.Application.Features.Results.ListingDealResults
{
    /// <summary>
    /// Sayfalanmış işlem listesi ve genel özet sonucu.
    /// </summary>
    public class GetListingDealsResult
    {
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<ListingDealRow> Rows { get; set; } = new List<ListingDealRow>();
        public ListingDealSummary Summary { get; set; } = new ListingDealSummary();
    }
}
