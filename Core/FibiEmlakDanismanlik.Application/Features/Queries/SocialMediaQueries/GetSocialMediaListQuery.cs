using FibiEmlakDanismanlik.Application.Features.Results.SocialMediaResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Queries.SocialMediaQueries
{
    public class GetSocialMediaListQuery:IRequest<List<GetSocialMediaResult>>
    {


    }
}
