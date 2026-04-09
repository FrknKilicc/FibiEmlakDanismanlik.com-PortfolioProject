using FibiEmlakDanismanlik.Application.Features.Queries.TestimonialsQueries;
using FibiEmlakDanismanlik.Application.Features.Results.TestimonialsResults;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.TestimonialsHandlers
{
    public class GetAllTestimonialsQueryHandler : IRequestHandler<GetTestimonailsQuery, List<GetTestimonialsResult>>
    {
        private readonly IRepository<Testimonials> repository;

        public GetAllTestimonialsQueryHandler(IRepository<Testimonials> repository)
        {
            this.repository = repository;
        }

        public async Task<List<GetTestimonialsResult>> Handle(GetTestimonailsQuery request, CancellationToken cancellationToken)
        {
            var value = await repository.GetAllAsync();

            return value.Select(x => new GetTestimonialsResult
            {
                Id = x.Id,
                Name = x.Name,
                LogoUrl = x.LogoUrl,
                Testimonail = x.Testimonail,
                Title = x.Title,
            }).ToList();
        }
    }
}
