namespace FibiEmlakDanismanlik.WebUI.Models
{
    public class CompareActionButtonVm
    {
        public int ListingId { get; set; }
        public int UsageTypeId { get; set; }
        public string ListingType { get; set; } = string.Empty;

        public string CompareTypeKey
        {
            get
            {
                var usage = UsageTypeId switch
                {
                    1 => "forsale",
                    2 => "forrent",
                    3 => "both",
                    _ => "unknown"
                };

                var listing = (ListingType ?? string.Empty).Trim().ToLowerInvariant();

                listing = listing switch
                {
                    "konut" => "housing",
                    "arsa" => "land",
                    "işyeri" => "commercial",
                    "isyeri" => "commercial",
                    _ => "unknown"
                };

                return $"{usage}-{listing}";
        }
        }
    }
}
 

