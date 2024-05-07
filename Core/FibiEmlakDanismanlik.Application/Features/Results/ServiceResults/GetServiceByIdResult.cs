using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Results.ServiceResults
{
    public class GetServiceByIdResult
    {
        public int ServiceId { get; set; }
        public string ServiceImgUrl { get; set; }
        public string ServiceTitle { get; set; }
        public string ServiceDesc { get; set; }
    }
}
