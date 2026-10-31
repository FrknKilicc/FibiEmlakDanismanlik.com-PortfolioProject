using MediatR;
using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Features.Queries.BlogQueries
{
    public class GetBlogTagNamesQuery : IRequest<List<string>>
    {
    }
}
