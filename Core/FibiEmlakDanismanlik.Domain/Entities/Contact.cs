using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public class Contact
    {
        [Key]
        public int ContactId { get; set; }
        public string EMail1 { get; set; }
        public string EMail2 { get; set; }
        public int PhoneNumber1 { get; set; }
        public int PhoneNumber2 { get; set; }
        public string OfficeAddress { get; set; }
    }
}
