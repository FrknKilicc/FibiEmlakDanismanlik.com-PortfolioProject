using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Commands.MainCategoryCommands
{
    public class UpdateMainCategoryCommand:IRequest
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
