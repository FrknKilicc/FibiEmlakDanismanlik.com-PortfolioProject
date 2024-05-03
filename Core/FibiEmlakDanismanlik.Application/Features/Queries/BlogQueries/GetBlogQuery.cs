using FibiEmlakDanismanlik.Application.Features.Results.BlogResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Queries.BlogQueries
{
    public class GetBlogQuery:IRequest<List<GetBlogResult>>
    {
    }
}
