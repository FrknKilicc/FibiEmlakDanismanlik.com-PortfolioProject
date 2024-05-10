using FibiEmlakDanismanlik.Application.Features.Results.MainBannerResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Queries.MainBannerQueries
{
    public class GetMainBannerQuery:IRequest<List<GetMainBannerResult>>
    {
    }
}
