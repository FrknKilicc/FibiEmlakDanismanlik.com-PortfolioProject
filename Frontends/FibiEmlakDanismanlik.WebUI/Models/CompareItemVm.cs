namespace FibiEmlakDanismanlik.WebUI.Models
{
    public class CompareItemVm
    {
        public int ListingId { get; set; }
        public int UsageTypeId { get; set; }
        public string ListingType { get; set; } = string.Empty;

        public string PropertyName { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public string? City { get; set; }
        public string? District { get; set; }
        public string? Neighborhood { get; set; }

        public double? GrossArea { get; set; }
        public string? ImageUrl { get; set; }

        // Konut / genel
        public string? RoomCount { get; set; }
        public string? LivingRoomCount { get; set; }
        public string? BathroomCount { get; set; }
        public string? BalconyCount { get; set; }
        public string? FloorLocation { get; set; }
        public string? BuildingAge { get; set; }
        public string? HeatingType { get; set; }

        public string? SectionCount { get; set; }
        public string? KitchenCount { get; set; }

        public string? NetArea { get; set; }
        public string? Deposit { get; set; }
        public string? Dues { get; set; }

        public string? ZoningStatus { get; set; }
        public string? DeedStatus { get; set; }
        public string? DevelopmentRight { get; set; }
        public string? PricePerSquareMeter { get; set; }
        // Arsa
        public string? BlockNo { get; set; }
        public string? Gabari { get; set; }
        public string? Kaks { get; set; }
        // İşyeri
        public string? UsageStatus { get; set; }
    }
}
