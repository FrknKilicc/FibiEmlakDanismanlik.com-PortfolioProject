using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Results.BlogResults
{
    public class BlogCategoryCountResult
    {
        public int? BlogCategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int? BlogCount { get; set; }
    }
}
