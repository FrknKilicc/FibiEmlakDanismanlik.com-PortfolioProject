using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public class RentalCommercialPropertyListing
    {
        //MainInfo
        public int RentalCommercialListId { get; set; }
        [Required]
        public int PropertyNo { get; set; }
        [Required]
        public string PropertyName { get; set; }
        [Required]
        public string PropertyDescription { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal Rent { get; set; } //Price
        //Adress
        public string City { get; set; } // İl
        public string District { get; set; } // İlçe
        public string AddressDesc { get; set; } //AçıkAdress
                                                //Adress
        public double GrossArea { get; set; } // brüt m2
        public double NetArea { get; set; }
        public string BuildingAge { get; set; }
        public int NumberOfFloors { get; set; }
        public string Heating { get; set; }
        public bool Furnished { get; set; }
        public string TitleDeedStatus { get; set; } // front drop list olarak verilecek  // tapu durumu 
        public decimal Dues { get; set; } // aidat
        public decimal Deposit { get; set; } //
        //MainInfo

        //relational
        public int AgentId { get; set; }
        public Agent Agent { get; set; }

        //Images
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
        //Images






    }
}
