using FibiEmlakDanismanlik.Application.Features.Results.BlogResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Queries.BlogQueries
{
    public class GetBlogSuggestionsQuery:IRequest<List<BlogSuggestionResult>>
    {
        public string Q { get; }
        public int Take { get; }

        public GetBlogSuggestionsQuery(string q, int take)
        {
            Q = q;
            Take = take;
        }
    }
}
