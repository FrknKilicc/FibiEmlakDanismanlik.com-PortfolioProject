using FibiEmlakDanismanlik.Application.Features.Results.AgentResult;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Queries.AgentQueries
{
    public class GetAgentQuery:IRequest<List<GetAgentResult>>
    {
    }
}
