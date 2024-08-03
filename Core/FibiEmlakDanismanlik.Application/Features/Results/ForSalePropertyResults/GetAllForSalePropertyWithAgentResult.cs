using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Results.ForSalePropertyResults
{
    public class GetAllForSalePropertyWithAgentResult
    {
        public string PropertyType { get; set; }
        public string PropertyName { get; set; }
        public decimal Price { get; set; }
        public string PropertyDescription { get; set; }
        public string PropertyStatus { get; set; }
        public DateTime CreatedDate { get; set; }

        public string AgentName { get; set; }
        public string AgentTitle { get; set; }
        public string AgentImgUrl { get; set; }
        public string PropImgUrl1 { get; set; }
    }
}
