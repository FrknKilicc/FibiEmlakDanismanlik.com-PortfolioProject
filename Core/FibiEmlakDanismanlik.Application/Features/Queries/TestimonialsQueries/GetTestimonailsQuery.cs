using FibiEmlakDanismanlik.Application.Features.Results.TestimonialsResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Queries.TestimonialsQueries
{
    public class GetTestimonailsQuery:IRequest<List<GetTestimonialsResult> >
    {
    }
}
