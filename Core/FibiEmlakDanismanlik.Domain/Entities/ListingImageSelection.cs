using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public class ListingImageSelection
    {
        [Key]
        public int Id { get; set; }

        public int ListingId { get; set; }

        [Required]
        public string SectionKey { get; set; } = "floorplan";

        // 1-30 (PropImgUrl'den  kaçıncı kolon)
        public byte ImageNo { get; set; }

        // Accordion sırası (1-3)
        public int SortOrder { get; set; }
        public string? SlotKey { get; set; }
    }
}
