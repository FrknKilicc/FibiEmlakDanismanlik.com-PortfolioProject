using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.TestimonialsCommands
{
    public class UpdateTestimonialCommand : IRequest
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Name { get; set; }
        public string? LogoUrl { get; set; }
        public string? Testimonail { get; set; }
        public bool IsActive { get; set; } = true;
        public int DisplayOrder { get; set; }
    }
}
