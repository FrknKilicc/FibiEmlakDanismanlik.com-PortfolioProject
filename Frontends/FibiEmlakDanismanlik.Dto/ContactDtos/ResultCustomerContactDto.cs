using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Dto.ContactDtos
{
    public class ResultCustomerContactDto
    {
        public int CustomerContactId { get; set; }
        public string CustomerContactName { get; set; }
        public string CustomerContactMail { get; set; }
        public string CustomerContactPhone { get; set; }
        public string CustomerContactMessage { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        //Relational

        public int? PropertyListingId { get; set; }
      
        //Relational
    }
}
