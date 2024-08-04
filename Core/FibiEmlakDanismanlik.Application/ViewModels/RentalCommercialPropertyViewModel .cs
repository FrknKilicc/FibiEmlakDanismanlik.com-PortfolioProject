using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.ViewModels
{
    public class RentalCommercialPropertyViewModel:RentalPropertyBaseViewModel
    {
        public string Facade { get; set; }
        public int? NumberOfSection { get; set; }
        public int? NumberOfKitchens { get; set; }
        public int? NumberOfBathrooms { get; set; }
        public int NumberOfFloors { get; set; }
        public double? GrossArea { get; set; }
        public double? NetArea { get; set; }
        public string BuildingAge { get; set; }
        public string Heating { get; set; }
        public string TitleDeedStatus { get; set; }
        public decimal Deposit { get; set; }
        public bool? BestDeals { get; set; }
    }
}
