using FibiEmlakDanismanlik.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Results.ForSaleLandListingResults
{
    public class GetForSalesLandListingResult
    {
        public int ForSaleLandListingId { get; set; }
        public int? PropertyNo { get; set; }
        public string PropertyName { get; set; }
        public string PropertyDescription { get; set; }
        public string PropertyStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public string City { get; set; } // İl
        public string District { get; set; } // İlçe
        public string Neighborhood { get; set; }//Mahalle
        public string AddressDesc { get; set; } //Açık Adress
        public string ZoningStatus { get; set; } // İmar Durumu
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

        public bool? BestDeals { get; set; }

        public int LandCategoryId { get; set; }
        public int AgentId { get; set; }


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


    }
}
