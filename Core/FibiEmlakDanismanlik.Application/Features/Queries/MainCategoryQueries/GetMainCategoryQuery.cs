using FibiEmlakDanismanlik.Application.Features.Results.MainCategoryResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Queries.MainCategoryQueries
{
    public class GetMainCategoryQuery:IRequest<List<GetMainCategoryResult>>
    {
      
    }
}
