using FibiEmlakDanismanlik.Application.Features.Queries.TestimonialsQueries;
using FibiEmlakDanismanlik.Application.Features.Results.TestimonialsResults;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.TestimonialsHandlers
{
    public class GetTestimonialByIdQueryHandler : IRequestHandler<GetTestimonialByIdQuery, GetTestimonialByIdResult?>
    {
        private readonly IRepository<Testimonials> _repository;

        public GetTestimonialByIdQueryHandler(IRepository<Testimonials> repository)
        {
            _repository = repository;
        }

        public async Task<GetTestimonialByIdResult?> Handle(GetTestimonialByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                return null;
            }

            return new GetTestimonialByIdResult
            {
                Id = entity.Id,
                Title = entity.Title,
                Name = entity.Name,
                LogoUrl = entity.LogoUrl,
                Testimonail = entity.Testimonail
            };
        }
    }
}
