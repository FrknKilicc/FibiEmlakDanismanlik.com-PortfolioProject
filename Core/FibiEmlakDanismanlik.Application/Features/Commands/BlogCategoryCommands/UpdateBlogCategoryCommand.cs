using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.BlogCategoryCommands
{
    public class UpdateBlogCategoryCommand : IRequest
    {
        public int BlogCategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
