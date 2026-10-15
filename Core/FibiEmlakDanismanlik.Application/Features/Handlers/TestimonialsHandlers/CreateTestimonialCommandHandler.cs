using FibiEmlakDanismanlik.Application.Features.Commands.TestimonialsCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.TestimonialsHandlers
{
    public class CreateTestimonialCommandHandler : IRequestHandler<CreateTestimonialCommand>
    {
        private readonly IRepository<Testimonials> _repository;

        public CreateTestimonialCommandHandler(IRepository<Testimonials> repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateTestimonialCommand request, CancellationToken cancellationToken)
        {
            var name = request.Name?.Trim();
            var testimonial = request.Testimonail?.Trim();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(testimonial))
            {
                throw new ArgumentException("Ad ve yorum metni zorunludur.");
            }

            var title = request.Title?.Trim();
            if (string.IsNullOrWhiteSpace(title))
            {
                title = null;
            }

            var logoUrl = request.LogoUrl?.Trim();
            if (string.IsNullOrWhiteSpace(logoUrl))
            {
                logoUrl = "icon-18";
            }

            var entity = new Testimonials
            {
                Name = name,
                Testimonail = testimonial,
                Title = title,
                LogoUrl = logoUrl
            };

            await _repository.CreateAsync(entity);
        }
    }
}
