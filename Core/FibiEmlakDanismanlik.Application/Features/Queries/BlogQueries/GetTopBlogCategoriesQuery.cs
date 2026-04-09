using FibiEmlakDanismanlik.Application.Features.Results.BlogResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Queries.BlogQueries
{
    public class GetTopBlogCategoriesQuery : IRequest<List<BlogCategoryCountResult>>
    {
        public int Take { get; set; }

        public GetTopBlogCategoriesQuery(int take = 5)
        {
            Take = take;
        }
    }
}
