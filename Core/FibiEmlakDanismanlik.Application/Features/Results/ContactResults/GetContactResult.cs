using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Results.ContactResults
{
    public class GetContactResult
    {
        public int ContactId { get; set; }
        public string EMail1 { get; set; }
        public string EMail2 { get; set; }
        public string PhoneNumber1 { get; set; }
        public string PhoneNumber2 { get; set; }
        public string OfficeAddress { get; set; }
    }
}
