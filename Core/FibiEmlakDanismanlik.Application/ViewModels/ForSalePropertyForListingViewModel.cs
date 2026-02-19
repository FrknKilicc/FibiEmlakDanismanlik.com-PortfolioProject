using FibiEmlakDanismanlik.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.ViewModels
{
    public class ForSalePropertyForListingViewModel
    {
        // Ortak Alanlar (3 model de var)
        public int ListingId { get; set; } //
        public int? PropertyNo { get; set; }
        public string PropertyName { get; set; }
        public string? PropertyDescription { get; set; }
        public string? PropertyStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public decimal? Price { get; set; }
        public string? TitleDeedStatus { get; set; } // DropDown için ortak
        public int SourceType { get; set; } // 1 konut, 2 arsa, 3 işyeri // listingtypeId sourcetype ı ikame ediyor.


        // Adres Bilgileri 
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Neighborhood { get; set; }
        public string? AddressDesc { get; set; }

        // Arsa  Alanları
        public double? Area { get; set; } // m²
        public double? SharePercentage { get; set; }
        public decimal? PricePerSquareMeter { get; set; }
        public string? ParcelNumber { get; set; } // Ada No
        public string? PlotNumber { get; set; } // Parsel No
        public string? MapSheetNumber { get; set; } // Pafta No
        public double? FloorAreaRatio { get; set; } // Kaks
        public double? BaseAreaRatio { get; set; } // Taks
        public string? ZoningPlan { get; set; } // Gabari
        public string? ZoningStatus { get; set; } // İmar Durumu
        public bool? DevelopmentRight { get; set; } // Kat Karşılığı
        public bool? LandLoan { get; set; }
        public bool? Exchange { get; set; }
        public bool? BestDeals { get; set; }
        public int? LandCategoryId { get; set; }

        // Konut Alanları
        public string? Facade { get; set; } // Cephe
        public bool? IsElevator { get; set; }
        public double? GrossArea { get; set; }
        public double? NetArea { get; set; }
        public double? OpenArea { get; set; }
        public string? BuildingAge { get; set; }
        public int? TotalNumberOfFloor { get; set; }
        public int? NumberOfFloors { get; set; } // Kaçıncı Kat
        public int? NumberOfBathRoom { get; set; }
        public string? NumberOfRoom { get; set; }
        public string? Heating { get; set; }
        public bool? BlackBox { get; set; }
        public int? NumberOfBalconies { get; set; }
        public bool? ParkingLot { get; set; }
        public bool? Furnished { get; set; }
        public string? UsageStatus { get; set; } // Drop list
        public bool? WithinTheComplex { get; set; }
        public decimal? Dues { get; set; }
        public bool? HomeLoan { get; set; }

        // Ticari Özel Alanlar
        public int? NumberOfSection { get; set; }
        public int? NumberOfKitchens { get; set; }
        public int? NumberOfBathrooms { get; set; }
        public bool? Transferable { get; set; }

        // İlan Kaynağı ve Temsilci
        public int AgentId { get; set; }
        public string? AgentName { get; set; }
        public string? AgentTitle { get; set; }
        public string? AgentImgUrl { get; set; }


        // İlan Türü: Arsa, Konut, Ticari

        public string ListingType { get; set; } // gerekbilir, arsa , konut,ticari
        public int ListingTypeId { get; set; }
        public int UsageTypeId { get; set; } // 1=ForSale, 2=ForRent, 3=Both

        //Images  Top10
        public string? PropImgUrl1 { get; set; }
        public string? PropImgUrl2 { get; set; }
        public string? PropImgUrl3 { get; set; }
        public string? PropImgUrl4 { get; set; }
        public string? PropImgUrl5 { get; set; }
        public string? PropImgUrl6 { get; set; }
        public string? PropImgUrl7 { get; set; }
        public string? PropImgUrl8 { get; set; }
        public string? PropImgUrl9 { get; set; }
        public string? PropImgUrl10 { get; set; }
        public string? PropImgUrl11 { get; set; }
        public string? PropImgUrl12 { get; set; }
        public string? PropImgUrl13 { get; set; }
        public string? PropImgUrl14 { get; set; }
        public string? PropImgUrl15 { get; set; }
        public string? PropImgUrl16 { get; set; }
        public string? PropImgUrl17 { get; set; }
        public string? PropImgUrl18 { get; set; }
        public string? PropImgUrl19 { get; set; }
        public string? PropImgUrl20 { get; set; }
        public string? PropImgUrl21 { get; set; }
        public string? PropImgUrl22 { get; set; }
        public string? PropImgUrl23 { get; set; }
        public string? PropImgUrl24 { get; set; }
        public string? PropImgUrl25 { get; set; }
        public string? PropImgUrl26 { get; set; }
        public string? PropImgUrl27 { get; set; }
        public string? PropImgUrl28 { get; set; }
        public string? PropImgUrl29 { get; set; }
        public string? PropImgUrl30 { get; set; }
        //Images
    
}
}
