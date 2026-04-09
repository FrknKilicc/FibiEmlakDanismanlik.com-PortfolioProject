using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Dto.FaqDtos
{
    public class ResultFaqDto
    {
        public int FrequentlyAskedQuestionId { get; set; }
        public string Question { get; set; }
        public string? Answer { get; set; }
    }
}
