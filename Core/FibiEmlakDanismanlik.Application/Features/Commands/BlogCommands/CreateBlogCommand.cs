using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Commands.BlogCommands
{
    public class CreateBlogCommand : IRequest
    {
        public string BlogTitle { get; set; } = string.Empty;
        public string BlogDescription { get; set; } = string.Empty;
        public string BlogImgUrl { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public bool TopNews { get; set; }
        public int AuthorId { get; set; }
        public int? BlogCategoryId { get; set; }
        public List<string>? Tags { get; set; }
    }
}
