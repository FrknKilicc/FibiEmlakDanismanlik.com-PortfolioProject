using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Results.BlogResults
{
    public class GetBlogDetailWithAuthorResult
    {
        public int BlogId { get; set; }
        public string BlogTitle { get; set; }
        public string BlogDescription { get; set; }
        public string BlogImgUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool TopNews { get; set; }

        // Author props
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorImgUrl { get; set; }

    }
}
