using FibiEmlakDanismanlik.Application.Features.Commands.TestimonialsCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.TestimonialsHandlers
{
    public class UpdateTestimonialCommandHandler : IRequestHandler<UpdateTestimonialCommand>
    {
        private readonly IRepository<Testimonials> _repository;

        public UpdateTestimonialCommandHandler(IRepository<Testimonials> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateTestimonialCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Yorum bulunamadı.");
            }

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

            entity.Name = name;
            entity.Testimonail = testimonial;
            entity.Title = title;
            entity.LogoUrl = logoUrl;

            await _repository.UpdateAsync(entity);
        }
    }
}
