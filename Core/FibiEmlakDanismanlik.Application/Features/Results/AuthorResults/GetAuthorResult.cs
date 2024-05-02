using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Results.AuthorResults
{
    public class GetAuthorResult
    {
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorImgUrl { get; set; }
    }
}
