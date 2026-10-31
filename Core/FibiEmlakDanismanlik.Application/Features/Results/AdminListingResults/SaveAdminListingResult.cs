namespace FibiEmlakDanismanlik.Application.Features.Results.AdminListingResults
{
    /// <summary>
    /// Yönetim paneli ilan kaydetme (ekleme/güncelleme) işlemi sonucu.
    /// </summary>
    public class SaveAdminListingResult
    {
        public int ListingTypeId { get; set; }
        public int ListingId { get; set; }

        public SaveAdminListingResult()
        {
        }

        public SaveAdminListingResult(int listingTypeId, int listingId)
        {
            ListingTypeId = listingTypeId;
            ListingId = listingId;
        }
    }
}
