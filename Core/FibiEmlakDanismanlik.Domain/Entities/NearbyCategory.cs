using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public class NearbyCategory
    {
        public int Id { get; set; }

        // education, health, transport gibi (unique)
        public string Key { get; set; } = null!;

        // UI'da görünen isim
        public string Name { get; set; } = null!;

        // tema ikon class'ı (fas fa-book-reader vb.)
        public string? IconCss { get; set; }

        public int SortOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;

        public ICollection<ListingNearbyPlace>? Places { get; set; }
    }
}
