using FibiEmlakDanismanlik.Dto.BlogDtos;

namespace FibiEmlakDanismanlik.WebUI.Models
{
    public class BlogListingPageVm
    {
        public List<ResultBlogWithAuthorDto> Items { get; set; } = new();

        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        public int TotalPages =>
            PageSize == 0 ? 0 : (int)Math.Ceiling((double)Total / PageSize);
    }
}
