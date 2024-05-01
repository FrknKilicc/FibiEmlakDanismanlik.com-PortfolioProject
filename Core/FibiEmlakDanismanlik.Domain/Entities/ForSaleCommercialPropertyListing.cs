using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public class ForSaleCommercialPropertyListing //Satılık İşyeri
    {
        //MainInfo 
        [Key]
        public int ForSaleCommercialListingId { get; set; } 

        [Display(Name = "İlan No")]
        public int PropertyNo { get; set; }

        [Display(Name = "İlan Adı")]
        [Required(ErrorMessage = "İlan adı zorunludur.")]
        public string PropertyName { get; set; }

        [Display(Name = "İlan Açıklaması")]
        [Required(ErrorMessage = "İlan Açıklaması zorunludur.")]
        public string PropertyDescription { get; set; }

        // Adres Bilgileri
        [Display(Name = "Şehir")]
        [Required(ErrorMessage = "Şehir alanı zorunludur.")]
        public string City { get; set; }

        [Display(Name = "İlçe")]
        public string District { get; set; }

        [Display(Name = "Adres")]
        public string AddressDesc { get; set; }
        // Adres Bilgileri

        [Display(Name = "Alan (m²)")]
        public double? Area { get; set; } 

        [Display(Name = "Fiyat")]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue, ErrorMessage = "Geçerli bir fiyat giriniz.")]
        public decimal Price { get; set; }

        [Display(Name = "Tapu Durumu")]
        public string TitleDeedStatus { get; set; } // Front drop list olarak verilecek // tapu durumu 

        [Display(Name = "Krediye Uygunluk")]
        public bool LandLoan { get; set; }

        [Display(Name = "Takas")]
        public bool Exchange { get; set; }

        [Display(Name = "Devren")]
        public bool Transferable { get; set; }
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
