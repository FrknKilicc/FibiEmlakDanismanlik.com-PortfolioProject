using FibiEmlakDanismanlik.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Results.BlogResults
{
    public class GetBlogResult
    {
        public int BlogId { get; set; }
        public string BlogTitle { get; set; } = string.Empty;
        public string BlogDescription { get; set; } = string.Empty;
        public string BlogImgUrl { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public bool TopNews { get; set; }
        public int AuthorId { get; set; }
        public int? BlogCategoryId { get; set; }
    }
}
