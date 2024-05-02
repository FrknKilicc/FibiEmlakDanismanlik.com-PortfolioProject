using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Results.AgentResult
{
    public class GetAgentResult
    {

       
        public int AgentId { get; set; }
        public string AgentName { get; set; }
        public string AgentTitle { get; set; }
        public string AgentDescription { get; set; }
        public string AgentImgUrl { get; set; }
        public string Mail { get; set; }
        public string AgentPhoneNumber { get; set; }
        public string Password { get; set; }
    }
}
