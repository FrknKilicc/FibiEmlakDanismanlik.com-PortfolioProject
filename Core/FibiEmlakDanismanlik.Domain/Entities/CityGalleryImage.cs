using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public class CityGalleryImage
    {
        public int CityGalleryImageId { get; set; }
        public int CityId { get; set; }

        public string ImageUrl { get; set; } = null!;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }

        public City City { get; set; } = null!;
    }
}
