using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.TestimonialsCommands
{
    public class RemoveTestimonialCommand : IRequest
    {
        public int Id { get; set; }

        public RemoveTestimonialCommand(int id)
        {
            Id = id;
        }
    }
}
