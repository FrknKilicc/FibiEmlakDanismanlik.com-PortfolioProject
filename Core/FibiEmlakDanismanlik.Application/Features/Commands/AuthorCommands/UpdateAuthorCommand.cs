using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Commands.AuthorCommands
{
    public class UpdateAuthorCommand:IRequest
    {
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorImgUrl { get; set; }
    }
}
