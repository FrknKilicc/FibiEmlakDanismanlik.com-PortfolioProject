namespace FibiEmlakDanismanlik.WebUI.Models
{
    public class CompareTableVm
    {
        public List<int> ListingIds { get; set; } = new ();
        public List<CompareItemVm> Items { get; set; } = new();

        public string CompareTypeKey { get; set; } = string.Empty;

        public bool HasError { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        public List<CompareFieldVm> Fields { get; set; } = new();
    }
}
