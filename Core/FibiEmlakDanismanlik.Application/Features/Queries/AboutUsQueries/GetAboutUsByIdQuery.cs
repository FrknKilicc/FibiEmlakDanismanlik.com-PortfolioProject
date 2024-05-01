using FibiEmlakDanismanlik.Application.Features.Results.AboutUsResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Queries.AboutUsQueries
{
    public class GetAboutUsByIdQuery:IRequest<GetAboutUsByIdResult>
    {
        public int Id { get; set; }

        public GetAboutUsByIdQuery(int id)
        {
            Id = id;
        }
    }
}
