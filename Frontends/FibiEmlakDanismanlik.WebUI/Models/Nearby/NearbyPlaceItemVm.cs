namespace FibiEmlakDanismanlik.WebUI.Models.Nearby
{
    public class NearbyPlaceItemVm
    {
        public string PlaceName { get; set; } = null!;
        public decimal DistanceKm { get; set; }
        public byte? Stars { get; set; }
    }
}
