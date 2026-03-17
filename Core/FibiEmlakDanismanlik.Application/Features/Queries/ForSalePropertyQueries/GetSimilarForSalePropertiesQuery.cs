using FibiEmlakDanismanlik.Application.Features.Results.CommonPropertyResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Queries.ForSalePropertyQueries
{
    public class GetSimilarForSalePropertiesQuery:IRequest<List<SimilarPropertyItemResult>>
    {
        public int Id { get; set; }

        public GetSimilarForSalePropertiesQuery(int id)
        {
            Id = id;
        }
    }
}
