using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Commands.CustomerContactCommands
{
    public class UpdateCustomerContactCommand:IRequest
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
