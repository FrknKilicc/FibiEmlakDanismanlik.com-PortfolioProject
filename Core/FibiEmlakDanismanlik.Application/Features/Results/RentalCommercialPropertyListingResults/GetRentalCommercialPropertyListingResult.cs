﻿using FibiEmlakDanismanlik.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Results.RentalCommercialPropertyListingResults
{
    public class GetRentalCommercialPropertyListingResult
    {
        public int RentalCommercialListId { get; set; }
        public int? PropertyNo { get; set; }
        public string PropertyName { get; set; }
        public string PropertyDescription { get; set; }
        public string PropertyStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal Rent { get; set; } //Price

        //Adress
        public string City { get; set; } // İl
        public string District { get; set; } // İlçe
        public string Neighborhood { get; set; }//Mahalle
        public string? AddressDesc { get; set; } //AçıkAdress
                                                 //Adress
        public string? Facade { get; set; } //cephe
        public int? NumberOfSection { get; set; } // Bölüm Sayısı
        public int? NumberOfKitchens { get; set; } // Mutfak Sayısı
        public int? NumberOfBathrooms { get; set; }// Lavabo Sayısı
        public int NumberOfFloors { get; set; } // Kaçıncı Kat
        public double? GrossArea { get; set; } // brüt m2
        public double? NetArea { get; set; }
        public string BuildingAge { get; set; }
        public string Heating { get; set; }
        public string? TitleDeedStatus { get; set; } // front drop list olarak verilecek  // tapu durumu 
        public decimal Deposit { get; set; }
        //MainInfo

        //Optional
        public bool? BestDeals { get; set; }
        //Optional

        //relational
        public int AgentId { get; set; }
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
