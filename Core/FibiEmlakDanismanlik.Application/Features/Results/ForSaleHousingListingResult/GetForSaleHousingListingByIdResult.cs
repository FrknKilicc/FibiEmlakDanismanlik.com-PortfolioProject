using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Results.ForSaleHousingListingResult
{
    public class GetForSaleHousingListingByIdResult
    {
        public int ForSaleHousingListId { get; set; }
        public int? PropertyNo { get; set; }
        public string PropertyName { get; set; }
        public string PropertyDescription { get; set; }
        public string? PropertyStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal Price { get; set; }
        //Adress
        public string City { get; set; } // İl
        public string District { get; set; } // İlçe
        public string Neighborhood { get; set; }//Mahalle
        public string? AddressDesc { get; set; } //Açık Adress
        public string? Facade { get; set; } //cephe
        public bool IsElevator { get; set; }
        public double GrossArea { get; set; }
        public double NetArea { get; set; }
        public double? OpenArea { get; set; }
        public string BuildingAge { get; set; }
        public int TotalNumberOfFloor { get; set; }
        public int NumberOfFloors { get; set; }//kaçıncı kat
        public int? NumberOfBathRoom { get; set; }
        public string NumberOfRoom { get; set; }
        public string Heating { get; set; }
        public bool? BlackBox { get; set; }
        public int? NumberOfBalconies { get; set; }
        public bool? ParkingLot { get; set; }
        public bool? Furnished { get; set; }
        public string UsageStatus { get; set; } //front drop list olarak verilecek
        public bool? WithinTheComplex { get; set; }
        public decimal? Dues { get; set; }

        public bool Exchange { get; set; }
        public string TitleDeedStatus { get; set; } // front drop list olarak verilecek
        public bool HomeLoan { get; set; }
        //MainInfo

        //Optional
        public bool? BestDeals { get; set; }
        //Optional

        //relational
        public int HousingCategoryId { get; set; }
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
    }
}
