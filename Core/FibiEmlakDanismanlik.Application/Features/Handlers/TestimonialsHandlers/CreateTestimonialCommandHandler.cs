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

            var title = string.IsNullOrWhiteSpace(request.Title) ? null : request.Title.Trim();
            var logoUrl = string.IsNullOrWhiteSpace(request.LogoUrl) ? "icon-18" : request.LogoUrl.Trim();

            var entity = new Testimonials
            {
                Name = name,
                Testimonail = testimonial,
                Title = title,
                LogoUrl = logoUrl,
                IsActive = request.IsActive,
                DisplayOrder = request.DisplayOrder
            };

            await _repository.CreateAsync(entity);
        }
    }
}
