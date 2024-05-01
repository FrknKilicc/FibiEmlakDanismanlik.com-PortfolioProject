using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.Entities
{
    public class FrequentlyAskedQuestion
    {
        [Key]
        public int FrequentlyAskedQuestionId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}
