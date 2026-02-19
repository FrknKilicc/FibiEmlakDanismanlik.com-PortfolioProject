namespace FibiEmlakDanismanlik.WebUI.Models
{
    public class SortVM
    {
        public string CurrentSortBy { get; set; } = "CreatedDesc";
        public string CurrentSortDir { get; set; } = "desc";
        public List<SortItemVM> Items { get; set; }

    }
    public class SortItemVM
    {
        public string SortBy { get; set; } = "CreatedDesc";
        public string SortDir { get; set; } = "desc";
        public string Text { get; set; } = ""; 
    }
}
