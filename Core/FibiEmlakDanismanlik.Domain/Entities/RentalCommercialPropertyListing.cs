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
    public class RentalCommercialPropertyListing : ILocatedListing, ISoftDeletable //Kiralık İşyeri 
    {
        //MainInfo
        [Key]
        public int RentalCommercialListId { get; set; }
        public int? PropertyNo { get; set; }
        [Required]
        public string PropertyName { get; set; }
        [Required]
        public string PropertyDescription { get; set; }
        public string PropertyStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? DeletedAtUtc { get; set; }
        [Required]
        public decimal Rent { get; set; } //Price

        //Adress
        public string City { get; set; } // İl
        public string District { get; set; } // İlçe
        public int? CityId { get; set; }      // Cities.CityId — City metninden otomatik çözülür
        public int? DistrictId { get; set; }  // Districts.DistrictId — District metninden otomatik çözülür
        public int? NeighborhoodId { get; set; }
        [Required]
        public string Neighborhood { get; set; }//Mahalle
        public string? AddressDesc { get; set; } //AçıkAdress
                                                 //Adress
        public string? Facade { get; set; } //cephe
        public int? NumberOfSection { get; set; } // Bölüm Sayısı
        public int? NumberOfKitchens { get; set; } // Mutfak Sayısı
        public int? NumberOfBathrooms { get; set; }// Lavabo Sayısı
        [Required]
        public int NumberOfFloors { get; set; } // Kaçıncı Kat
        public double? GrossArea { get; set; } // brüt m2
        public double? NetArea { get; set; }
        [Required]
        public string BuildingAge { get; set; }
        [Required]
        public string Heating { get; set; }
        public string? TitleDeedStatus { get; set; } // front drop list olarak verilecek  // tapu durumu 
        [Required]
        public decimal Deposit { get; set; } 
        //MainInfo

         //Optional
        public bool? BestDeals { get; set; }
        //Optional

        //relational
        public int AgentId { get; set; }
        public Agent Agent { get; set; }
        public int ListingTypeId { get; set; }

        [ForeignKey("ListingTypeId")]
        public ListingType ListingType { get; set; }

        //relational

        //Images
    }
}
