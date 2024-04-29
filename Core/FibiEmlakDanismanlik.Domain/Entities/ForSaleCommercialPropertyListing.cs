using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public class ForSaleCommercialPropertyListing //Satılık İşyeri
    {
        //MainInfo
        public int ForSaleCommercialListingId { get; set; }
        [Required]
        public int PropertyNo { get; set; }
        [Required]
        public string PropertyName { get; set; }
        [Required]
        public string PropertyDescription { get; set; }
        //Adress
        [Required]
        public string City { get; set; } // İl
        [Required]
        public string District { get; set; } // İlçe
        public string AddressDesc { get; set; } //Açık Adress
        //Adress
        public double Area { get; set; } // m²
        public decimal Price { get; set; } //Price
        public string TitleDeedStatus { get; set; } // front drop list olarak verilecek  // tapu durumu 
        public bool LandLoan { get; set; }// Krediye Uygunluk
        public bool Exchange { get; set; } // Takas
        public bool Transferable { get; set; } // devredilebilir ? 
        //MainInfo

        //Optional
        public bool? BestDeals { get; set; }
        //Optional

        //relational
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
