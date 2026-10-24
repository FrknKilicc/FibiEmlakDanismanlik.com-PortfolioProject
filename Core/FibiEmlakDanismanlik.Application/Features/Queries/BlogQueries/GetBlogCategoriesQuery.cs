using FibiEmlakDanismanlik.Application.Features.Results.BlogResults;
using MediatR;
using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Features.Queries.BlogQueries
{
    public class GetBlogCategoriesQuery : IRequest<List<BlogCategoryItemResult>>
    {
    }
}
