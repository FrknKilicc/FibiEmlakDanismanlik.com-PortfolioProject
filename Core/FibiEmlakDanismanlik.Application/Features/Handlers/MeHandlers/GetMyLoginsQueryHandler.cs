using FibiEmlakDanismanlik.Application.Features.Queries.MeQueries;
using FibiEmlakDanismanlik.Application.Features.Results.MeResults;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.AgentInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.MeHandlers
{
    /// <summary>
    /// Giriş yapmış çalışanın son 20 giriş denemesini listeleyen işleyici.
    /// </summary>
    public class GetMyLoginsQueryHandler : IRequestHandler<GetMyLoginsQuery, List<GetMyLoginResult>>
    {
        private const int RecentLoginsCount = 20;

        private readonly ICurrentUserService _currentUserService;
        private readonly IAgentRepository _agentCustomRepository;

        public GetMyLoginsQueryHandler(
            ICurrentUserService currentUserService,
            IAgentRepository agentCustomRepository)
        {
            _currentUserService = currentUserService;
            _agentCustomRepository = agentCustomRepository;
        }

        public async Task<List<GetMyLoginResult>> Handle(GetMyLoginsQuery request, CancellationToken cancellationToken)
        {
            var agentId = _currentUserService.AgentId;
            if (agentId == null)
            {
                throw new UnauthorizedAccessException("Oturum açmış kullanıcı bulunamadı.");
            }

            var logs = await _agentCustomRepository.GetRecentLoginsAsync(agentId.Value, RecentLoginsCount);

            return logs.Select(x => new GetMyLoginResult
            {
                CreatedAtUtc = x.CreatedAtUtc,
                Succeeded = x.Succeeded,
                FailureReason = x.FailureReason,
                IpAddress = x.IpAddress,
                UserAgent = x.UserAgent
            }).ToList();
        }
    }
}
