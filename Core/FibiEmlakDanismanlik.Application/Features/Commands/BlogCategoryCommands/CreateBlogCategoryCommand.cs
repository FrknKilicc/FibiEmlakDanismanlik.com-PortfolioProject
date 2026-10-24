using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.BlogCategoryCommands
{
    public class CreateBlogCategoryCommand : IRequest
    {
        public string CategoryName { get; set; } = string.Empty;
    }
}
