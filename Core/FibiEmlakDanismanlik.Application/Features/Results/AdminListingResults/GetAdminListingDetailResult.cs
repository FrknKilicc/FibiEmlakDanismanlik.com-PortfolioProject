using FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces;

namespace FibiEmlakDanismanlik.Application.Features.Results.AdminListingResults
{
    /// <summary>
    /// Yönetim paneli ilan detay sorgusu sonucu.
    /// </summary>
    public class GetAdminListingDetailResult
    {
        public AdminListingDetail Listing { get; set; } = null!;
        public bool CanEdit { get; set; }
    }
}
