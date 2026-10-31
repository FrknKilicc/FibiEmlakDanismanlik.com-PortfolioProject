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
    public class ForSaleHousingListing : ILocatedListing, ISoftDeletable //Satılık Konut
    {

        //MainInfo
        [Key]
        public int ForSaleHousingListId { get; set; }
        public int? PropertyNo { get; set; }
        [Required]
        public string PropertyName { get; set; }
        [Required]
        public string PropertyDescription { get; set; }
        public string? PropertyStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? DeletedAtUtc { get; set; }
        [Required]
        public decimal Price { get; set; }
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
        public string? AddressDesc { get; set; } //Açık Adress
        //Adress
        public string? Facade { get; set; } //cephe
        [Required]
        public bool IsElevator { get; set; }
        [Required]
        public double GrossArea { get; set; }
        [Required]
        public double NetArea { get; set; }
        public double? OpenArea { get; set; }
        [Required]
        public string BuildingAge { get; set; }
        [Required]
        public int TotalNumberOfFloor { get; set; }
        [Required]
        public int NumberOfFloors { get; set; } //kaçıncı kat
        public int? NumberOfBathRoom { get; set; }
        [Required]
        public string NumberOfRoom { get; set; }
        [Required]
        public string Heating { get; set; }
        public bool? BlackBox { get; set; }
        public int? NumberOfBalconies { get; set; }
        public bool? ParkingLot { get; set; }
        public bool? Furnished { get; set; }
        [Required]
        public string UsageStatus { get; set; } //front drop list olarak verilecek
        public bool? WithinTheComplex { get; set; }
        public decimal? Dues { get; set; }

        [Display(Name = "Takas")]
        public bool Exchange { get; set; }
        [Required]
        public string TitleDeedStatus { get; set; } // front drop list olarak verilecek
        [Required]
        public bool HomeLoan { get; set; }
        //MainInfo

        //Optional
        public bool? BestDeals { get; set; }
        //Optional

        //relational
        public int HousingCategoryId { get; set; }
        public HousingCategory HousingCategory { get; set; }
        public int AgentId { get; set; }
        public Agent Agent { get; set; }
        public int ListingTypeId { get; set; }

        [ForeignKey("ListingTypeId")]
        public ListingType ListingType { get; set; }
        //relational
        //Images
    }
}
