using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public class Agent
    {
        public int AgentId { get; set; }
        [Required]
        public string AgentName { get; set; }
        public string AgentDescription { get; set; }
        public string AgentImgUrl { get; set; }
        public string Mail { get; set; }
        public string AgentPhoneNumber { get; set; }

    }
}
