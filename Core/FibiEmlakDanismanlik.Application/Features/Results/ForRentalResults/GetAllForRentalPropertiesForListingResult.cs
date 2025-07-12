using FibiEmlakDanismanlik.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Results.ForRentalResults
{
    public class GetAllForRentalPropertiesForListingResult:RentalPropertyBaseViewModel
    {
        // Konut & Ticari ortak
        public string? BuildingAge { get; set; }
        public double? GrossArea { get; set; }
        public double? NetArea { get; set; }
        public string? Heating { get; set; }
        public int? NumberOfFloors { get; set; } // Kaçıncı katta

        // Entity Idleri
        public int? RentalHousingListId { get; set; }
        public int? RentalCommercialListId { get; set; }
        public int? RentalLandListingId { get; set; }

        // Kategoriler
        public int? HousingCategoryId { get; set; }
        public string? HousingCategory { get; set; } // mapping ile getirişcem

        // Konut
        public bool? IsElevator { get; set; }
        public double? OpenArea { get; set; }
        public int? TotalNumberOfFloor { get; set; }
        public int? NumberOfBathRoom { get; set; }
        public string? NumberOfRoom { get; set; }
        public int? NumberOfBalconies { get; set; }
        public bool? BlackBox { get; set; }
        public bool? ParkingLot { get; set; }
        public bool? Furnished { get; set; }
        public bool? WithinTheComplex { get; set; }
        public decimal? Dues { get; set; }

        // İşyeri
        public string? Facade { get; set; }
        public int? NumberOfSection { get; set; }
        public int? NumberOfKitchens { get; set; }
        public int? NumberOfBathrooms { get; set; }

        //Arsa
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

        public int? LandCategoryId { get; set; }
        public string? LandCategory { get; set; }
    }
}
