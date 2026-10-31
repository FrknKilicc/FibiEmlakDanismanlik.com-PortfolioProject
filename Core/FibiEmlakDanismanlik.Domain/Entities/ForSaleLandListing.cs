using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Domain.Common;
using FibiEmlakDanismanlik.Domain.Interfaces;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public class ForSaleLandListing : ILocatedListing, ISoftDeletable //Satılık Arsa
    {
        //MainInfo
        [Key]
        public int ForSaleLandListingId { get; set; }
        public int? PropertyNo { get; set; }
        [Required]
        public string PropertyName { get; set; }
        [Required]
        public string PropertyDescription { get; set; }
        public string PropertyStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? DeletedAtUtc { get; set; }
        //Adress
        [Required]
        public string City { get; set; } // İl
        [Required]
        public string District { get; set; } // İlçe
        public int? CityId { get; set; }      // Cities.CityId — City metninden otomatik çözülür
        public int? DistrictId { get; set; }  // Districts.DistrictId — District metninden otomatik çözülür
        public int? NeighborhoodId { get; set; }
        [Required]
        public string Neighborhood { get; set; }//Mahalle
        public string AddressDesc { get; set; } //Açık Adress
        //mahalle
        //Adress
        public string ZoningStatus { get; set; } // İmar Durumu

        //between
        public double? SharePercentage { get; set; } // çevir
        public double Area { get; set; } // m²
        public decimal Price { get; set; } //Price
        public decimal PricePerSquareMeter { get; set; } // m² Fiyatı
        public string? ParcelNumber { get; set; } // Ada No
        public string? PlotNumber { get; set; } // Parsel No
        public string? MapSheetNumber { get; set; } // Pafta No
        public double? FloorAreaRatio { get; set; } // Kaks (Emsal)
        public double? BaseAreaRatio { get; set; } // Taks (Emsal)
        public string? ZoningPlan { get; set; } // Gabari
        public string TitleDeedStatus { get; set; } // front drop list olarak verilecek  // tapu durumu 
        public bool? DevelopmentRight { get; set; } // Kat Karşılığı
        public bool? LandLoan { get; set; }// Krediye Uygunluk
        public bool? Exchange { get; set; } // Takas
        //MainInfo

        //Optional
        public bool? BestDeals { get; set; }
        //Optional

        //relational
        public int LandCategoryId { get; set; }
        public LandCategory LandCategory { get; set; }
        public int AgentId { get; set; }
        public Agent Agent { get; set; }
        public int ListingTypeId { get; set; }

        [ForeignKey("ListingTypeId")]
        public ListingType ListingType { get; set; }

        //relational

        //Images
    }
}
