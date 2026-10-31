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
    public class RentalHousingListing : ILocatedListing, ISoftDeletable //Kiralık Konut
    {
        //MainInfo
        [Key]
        public int RentalHousingListId { get; set; }
        public int? PropertyNo { get; set; }
        [Required]
        public string PropertyName { get; set; }
        [Required]
        public string PropertyDescription { get; set; }
        public string? PropertyStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? DeletedAtUtc { get; set; }
        [Required]
        public decimal Rent { get; set; } //Price
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
        public string? AddressDesc { get; set; } //AçıkAdress
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
        public int NumberOfFloors { get; set; }//kaçıncı kat
        public int? NumberOfBathRoom { get; set; }
        [Required]
        public string NumberOfRoom { get; set; }
        public int? NumberOfBalconies { get; set; }
        [Required]
        public string Heating { get; set; }
        public bool? BlackBox { get; set; }
        public bool? ParkingLot { get; set; }
        public bool? Furnished { get; set; }
        public bool? WithinTheComplex { get; set; }
        public decimal? Dues { get; set; } // aidat
        [Required]
        public decimal Deposit { get; set; }
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
