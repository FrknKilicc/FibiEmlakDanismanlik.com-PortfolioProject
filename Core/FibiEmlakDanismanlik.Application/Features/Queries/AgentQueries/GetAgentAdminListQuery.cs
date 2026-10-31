using FibiEmlakDanismanlik.Application.Features.Results.AgentResult;
using MediatR;
using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Features.Queries.AgentQueries
{
    public class GetAgentAdminListQuery : IRequest<List<GetAgentAdminListResult>>
    {
    }
}
