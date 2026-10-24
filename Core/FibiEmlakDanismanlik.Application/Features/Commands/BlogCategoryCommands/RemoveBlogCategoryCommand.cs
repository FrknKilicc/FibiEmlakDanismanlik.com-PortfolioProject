using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.BlogCategoryCommands
{
    public class RemoveBlogCategoryCommand : IRequest
    {
        public int Id { get; set; }

        public RemoveBlogCategoryCommand(int id)
        {
            Id = id;
        }
    }
}
