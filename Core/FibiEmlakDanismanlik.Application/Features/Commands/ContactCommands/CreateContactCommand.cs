using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Commands.ContactCommands
{
    public class CreateContactCommand:IRequest
    {
        public string EMail1 { get; set; }
        public string EMail2 { get; set; }
        public string PhoneNumber1 { get; set; }
        public string PhoneNumber2 { get; set; }
        public string OfficeAddress { get; set; }
    }
}
