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

            var title = string.IsNullOrWhiteSpace(request.Title) ? null : request.Title.Trim();

            // Komuttaki değer boşsa entity'deki değeri değiştirme; entity'deki de boşsa "icon-18" yap
            if (!string.IsNullOrWhiteSpace(request.LogoUrl))
            {
                entity.LogoUrl = request.LogoUrl.Trim();
            }
            else if (string.IsNullOrWhiteSpace(entity.LogoUrl))
            {
                entity.LogoUrl = "icon-18";
            }

            entity.Name = name;
            entity.Testimonail = testimonial;
            entity.Title = title;
            entity.IsActive = request.IsActive;
            entity.DisplayOrder = request.DisplayOrder;

            await _repository.UpdateAsync(entity);
        }
    }
}
