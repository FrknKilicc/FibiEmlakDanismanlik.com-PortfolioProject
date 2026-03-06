using FibiEmlakDanismanlik.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Results.ForRentalPropertyResults
{
    public class GetForRentalPropertyDetailResult:ForRentalPropertForListingViewModel
    {
        public List<AmenityItemViewModel> Amenities { get; set; } = new();

        public class SelectedImageDto
        {
            public string Title { get; set; } = "";
            public string Url { get; set; } = "";
            public int SortOrder { get; set; }
        }
        public List<SelectedImageDto> FloorPlanImageItems { get; set; } = new();
    }
}
