using System;

namespace FibiEmlakDanismanlik.Application.Features.Results.BlogResults
{
    public class BlogCategoryItemResult
    {
        public int BlogCategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int BlogCount { get; set; }
    }
}
