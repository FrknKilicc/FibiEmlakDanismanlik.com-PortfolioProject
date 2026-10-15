using FibiEmlakDanismanlik.Application.Features.Results.TestimonialsResults;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Queries.TestimonialsQueries
{
    public class GetTestimonialByIdQuery : IRequest<GetTestimonialByIdResult?>
    {
        public int Id { get; set; }

        public GetTestimonialByIdQuery(int id)
        {
            Id = id;
        }
    }
}
