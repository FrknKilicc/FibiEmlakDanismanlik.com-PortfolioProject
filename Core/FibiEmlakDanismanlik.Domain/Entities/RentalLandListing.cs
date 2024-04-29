using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public class RentalLandListing
    {   //MainInfo
        public int RentalLandListingId { get; set; }
        [Required]
        public int PropertyNo { get; set; }
        [Required]
        public string PropertyName { get; set; }
        [Required]
        public string PropertyDescription { get; set; }
        //Adress
        public string City { get; set; } // İl
        public string District { get; set; } // İlçe
        public string AddressDesc { get; set; } //Açık Adress
        //Adress
        public string ZoningStatus { get; set; } // İmar Durumu
        public double Area { get; set; } // m²
        public decimal Rent { get; set; } //PricePerMontxh
        public decimal PricePerSquareMeter { get; set; } // m² Fiyatı
        public string ParcelNumber { get; set; } // Ada No
        public string PlotNumber { get; set; } // Parsel No
        public string MapSheetNumber { get; set; } // Pafta No
        public double FloorAreaRatio { get; set; } // Kaks (Emsal)
        public double BaseAreaRatio { get; set; } // Taks (Emsal)
        public string ZoningPlan { get; set; } // Gabari
        public string TitleDeedStatus { get; set; } // front drop list olarak verilecek 
        public string DevelopmentRight { get; set; } // Kat Karşılığı
        //MainInfo

        //relational
        public int LandCategoryId { get; set; }
        public LandCategory LandCategory { get; set; }
        public int AgentId { get; set; }
        public Agent Agent { get; set; }
        //relational

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
