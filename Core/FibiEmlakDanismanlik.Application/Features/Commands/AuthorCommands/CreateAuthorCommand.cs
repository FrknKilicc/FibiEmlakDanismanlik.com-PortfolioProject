using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Commands.AuthorCommands
{
    public class CreateAuthorCommand:IRequest
    {
        public string AuthorName { get; set; }
        public string AuthorImgUrl { get; set; }
    }
}
