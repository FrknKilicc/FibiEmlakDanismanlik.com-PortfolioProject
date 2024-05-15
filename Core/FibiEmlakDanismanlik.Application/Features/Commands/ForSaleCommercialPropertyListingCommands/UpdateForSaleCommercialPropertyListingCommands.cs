using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Commands.ForSaleCommercialPropertyListingCommands
{
    public class UpdateForSaleCommercialPropertyListingCommands: IRequest
    {
        //MainInfo 
        public int ForSaleCommercialListingId { get; set; }
        public string PropertyName { get; set; }
        public string PropertyDescription { get; set; }

        // Adres Bilgileri
        public string City { get; set; }
        public string District { get; set; }
        public string AddressDesc { get; set; }
        // Adres Bilgileri
        public double? Area { get; set; }
        public decimal Price { get; set; }
        public string TitleDeedStatus { get; set; } // Front drop list olarak verilecek // tapu durumu 
        public bool LandLoan { get; set; }
        public bool Exchange { get; set; }
        public bool Transferable { get; set; }
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
        //Images
    }
}
