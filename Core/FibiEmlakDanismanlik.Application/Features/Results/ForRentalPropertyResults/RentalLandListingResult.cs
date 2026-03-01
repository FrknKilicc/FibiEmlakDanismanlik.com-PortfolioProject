using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Results.ForRentalPropertyResults
{
    public class RentalLandListingResult:RentalListingBaseResult
    {
        public int? LandCategoryId { get; set; }

        public string? ZoningStatus { get; set; }

        public double? Area { get; set; }
        public decimal? PricePerSquareMeter { get; set; }

        public string? ParcelNumber { get; set; }
        public string? PlotNumber { get; set; }
        public string? MapSheetNumber { get; set; }

        public double? FloorAreaRatio { get; set; }
        public double? BaseAreaRatio { get; set; }

        public string? ZoningPlan { get; set; }
        public string? DevelopmentRight { get; set; }
    }
}
