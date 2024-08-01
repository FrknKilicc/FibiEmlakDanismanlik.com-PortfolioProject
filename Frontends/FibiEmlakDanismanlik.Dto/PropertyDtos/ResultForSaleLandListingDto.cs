using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Dto.PropertyDtos
{
    public class ResultForSaleLandListingDto
    {
        //MainInfo
        public int ForSaleLandListingId { get; set; }
        public int? PropertyNo { get; set; }
        public string PropertyName { get; set; }
        public string PropertyDescription { get; set; }
        public string PropertyStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        //Adress
        public string City { get; set; } // İl
        public string District { get; set; } // İlçe
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
        public int AgentId { get; set; }

        //relational

        //Images
        public string? PropImgUrl1 { get; set; }
    }
}
