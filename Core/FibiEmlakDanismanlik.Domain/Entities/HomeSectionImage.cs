using System.ComponentModel.DataAnnotations;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public class HomeSectionImage
    {
        [Key]
        public int HomeSectionImageId { get; set; }
        public string SectionKey { get; set; } = null!;   // örn. "PopularPlaces"
        public int Slot { get; set; }                     // bölüm içindeki sıra (1, 2, ...)
        public string ImageUrl { get; set; } = null!;
        public string? AltText { get; set; }
        public bool IsActive { get; set; }
    }
}
