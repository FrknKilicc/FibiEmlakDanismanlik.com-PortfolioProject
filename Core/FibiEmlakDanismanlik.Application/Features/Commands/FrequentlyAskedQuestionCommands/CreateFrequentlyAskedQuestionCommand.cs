using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Commands.FrequentlyAskedQuestionCommands
{
    public class CreateFrequentlyAskedQuestionCommand:IRequest
    {
        public string Question { get; set; }
        public string? Answer { get; set; }

    }
}
