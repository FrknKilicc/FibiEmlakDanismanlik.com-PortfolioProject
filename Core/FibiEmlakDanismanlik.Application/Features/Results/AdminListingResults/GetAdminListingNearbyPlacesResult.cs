using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Features.Results.AdminListingResults
{
    /// <summary>
    /// Yakın yer kategorisi seçim öğesi.
    /// </summary>
    public class NearbyCategoryItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? IconCss { get; set; }
    }

    /// <summary>
    /// İlana bağlı yakın yer kaydı.
    /// </summary>
    public class NearbyPlaceItem
    {
        public int Id { get; set; }
        public int NearbyCategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string PlaceName { get; set; } = string.Empty;
        public decimal DistanceKm { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Yönetim paneli ilan düzenlemedeki "Yakın Yerler" kartının verisi.
    /// </summary>
    public class GetAdminListingNearbyPlacesResult
    {
        public bool CanEdit { get; set; }
        public List<NearbyCategoryItem> Categories { get; set; } = new();
        public List<NearbyPlaceItem> Places { get; set; } = new();
    }
}
