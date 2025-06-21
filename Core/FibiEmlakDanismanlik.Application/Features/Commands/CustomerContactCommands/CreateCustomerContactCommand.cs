using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Commands.CustomerContactCommands
{
    public class CreateCustomerContactCommand:IRequest
    {
       
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
