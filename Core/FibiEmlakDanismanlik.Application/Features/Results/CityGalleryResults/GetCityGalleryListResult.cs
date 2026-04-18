using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Results.CityGalleryResults
{
    public class GetCityGalleryListResult
    {
        public int CityGalleryImageId { get; set; }
        public int CityId { get; set; }

        public string CityName { get; set; } = string.Empty;
        public string CitySlug { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }
    }
}
