using System.Collections.Generic;
using FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces;

namespace FibiEmlakDanismanlik.Application.Features.Results.AdminListingResults
{
    /// <summary>
    /// Yönetim paneli ilan listeleme sorgusu sonucu.
    /// </summary>
    public class GetAdminListingsResult
    {
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<AdminListingRow> Items { get; set; } = new();
        public AdminListingSummary Summary { get; set; } = new();
    }
}
