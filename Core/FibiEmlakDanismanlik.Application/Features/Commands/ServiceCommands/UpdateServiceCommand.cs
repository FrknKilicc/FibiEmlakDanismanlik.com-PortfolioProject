using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Commands.ServiceCommands
{
    public class UpdateServiceCommand:IRequest
    {
        public int ServiceId { get; set; }
        public string ServiceImgUrl { get; set; }
        public string ServiceTitle { get; set; }
        public string ServiceDesc { get; set; }
    }
}
