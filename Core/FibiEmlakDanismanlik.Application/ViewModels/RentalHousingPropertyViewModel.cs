using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.ViewModels
{
    public class RentalHousingPropertyViewModel:RentalPropertyBaseViewModel
    {
        public bool IsElevator { get; set; }
        public double GrossArea { get; set; }
        public double NetArea { get; set; }
        public double? OpenArea { get; set; }
        public string BuildingAge { get; set; }
        public int TotalNumberOfFloor { get; set; }
        public int NumberOfFloors { get; set; }
        public int? NumberOfBathRoom { get; set; }
        public string NumberOfRoom { get; set; }
        public int? NumberOfBalconies { get; set; }
        public string Heating { get; set; }
        public bool? BlackBox { get; set; }
        public bool? ParkingLot { get; set; }
        public bool? Furnished { get; set; }
        public bool? WithinTheComplex { get; set; }
        public decimal? Dues { get; set; }
        public decimal Deposit { get; set; }
        public bool? BestDeals { get; set; }
    }
}
