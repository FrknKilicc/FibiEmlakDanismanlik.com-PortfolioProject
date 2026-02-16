using FibiEmlakDanismanlik.Application.Features.Results.MainBannerResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Queries.MainBannerQueries
{
    public class GetMainBannerByIdQuery:IRequest<GetMainBannerByIdResult>
    {
        public int Id { get; set; }

        public GetMainBannerByIdQuery(int id)
        {
            Id = id;
        }
    }
}
