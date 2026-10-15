using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.TestimonialsCommands
{
    public class CreateTestimonialCommand : IRequest
    {
        public string? Title { get; set; }
        public string? Name { get; set; }
        public string? LogoUrl { get; set; }
        public string? Testimonail { get; set; }
    }
}
