using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Commands.MainCategoryCommands
{
    public class RemoveMainCategoryCommand:IRequest
    {
        public int Id { get; set; }

        public RemoveMainCategoryCommand(int id)
        {
            Id = id;
        }
    }
}
