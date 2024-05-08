using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Commands.FrequentlyAskedQuestionCommands
{
    public class RemoveFrequentlyAskedQuestionCommand:IRequest
    {
        public int Id { get; set; }

        public RemoveFrequentlyAskedQuestionCommand(int id)
        {
            Id = id;
        }
    }
}
