using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public class ForSaleHousingListing //Satılık Konut
    {

        //MainInfo
        [Key]
        public int ForSaleHousingListId { get; set; }
        public int PropertyNo { get; set; }
        [Required]
        public string PropertyName { get; set; }
        [Required]
        public string PropertyDescription { get; set; }
        public string? PropertyStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        [Required]
        public decimal Price { get; set; }
        //Adress
        [Required]
        public string City { get; set; } // İl
        [Required]
        public string District { get; set; } // İlçe
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
        public int NumberOfFloors { get; set; }//kaçıncı kat
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
        //relational
        //Images
        [Required]
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
