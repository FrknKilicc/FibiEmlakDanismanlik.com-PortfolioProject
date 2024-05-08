using FibiEmlakDanismanlik.Application.Features.Results.MainCategoryResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Queries.MainCategoryQueries
{
    public class GetMainCategoryByIdQuery:IRequest<GetMainCategoryByIdResult>
    {
        public int Id { get; set; }

        public GetMainCategoryByIdQuery(int id)
        {
            Id = id;
        }
    }
}
