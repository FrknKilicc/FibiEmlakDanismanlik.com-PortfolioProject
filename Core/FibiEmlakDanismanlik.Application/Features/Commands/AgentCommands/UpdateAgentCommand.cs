using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Commands.AgentCommands
{
    public class UpdateAgentCommand:IRequest
    {
        public int AgentId { get; set; }
        public string AgentName { get; set; }
        public string AgentTitle { get; set; }
        public string AgentDescription { get; set; }
        public string AgentImgUrl { get; set; }
        public string Mail { get; set; }
        public string AgentPhoneNumber { get; set; }
        public string Password { get; set; }
        public AgentRole? Role { get; set; }
        public bool? IsActive { get; set; }
        public bool? ShowOnWebsite { get; set; }
        public bool? CanViewAllListings { get; set; }
        public bool ResetTwoFactor { get; set; }
        public bool Unlock { get; set; }
    }
}
