namespace FibiEmlakDanismanlik.WebUI.Models.Nearby
{
    public class NearbyCategoryGroupVm
    {
        public string CategoryName { get; set; } = null!;
        public string? IconCss { get; set; }
        public int SortOrder { get; set; }
        public List<NearbyPlaceItemVm> Items { get; set; } = new();
    }
}
