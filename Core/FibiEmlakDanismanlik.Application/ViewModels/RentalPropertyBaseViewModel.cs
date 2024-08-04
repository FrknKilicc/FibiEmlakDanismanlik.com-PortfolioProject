using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.ViewModels
{
    public class RentalPropertyBaseViewModel
    {
        public int? PropertyNo { get; set; }
        public string PropertyName { get; set; }
        public string PropertyType { get; set; }
        public string PropertyDescription { get; set; }
        public string PropertyStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal Rent { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Neighborhood { get; set; }
        public string AddressDesc { get; set; }
        public int AgentId { get; set; }
        public string AgentName { get; set; }
        public string AgentImgUrl { get; set; }
        public string PropImgUrl1 { get; set; }
    }
}
