using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Features.Queries.LeadQueries;
using FibiEmlakDanismanlik.Application.Features.Results.LeadResults;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.LeadInterfaces;
using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.LeadHandlers
{
    /// <summary>
    /// Müşteri taleplerini kullanıcı rol ve yetki kapsamına göre arayan ve özet sayaçlarını üreten sorgu işleyicisi.
    /// </summary>
    public class GetLeadsQueryHandler : IRequestHandler<GetLeadsQuery, GetLeadsResult>
    {
        private readonly ILeadRepository _leadRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetLeadsQueryHandler(ILeadRepository leadRepository, ICurrentUserService currentUserService)
        {
            _leadRepository = leadRepository;
            _currentUserService = currentUserService;
        }

        public async Task<GetLeadsResult> Handle(GetLeadsQuery request, CancellationToken cancellationToken)
        {
            var seeAll = _currentUserService.Role == AgentRole.Owner || _currentUserService.Role == AgentRole.Manager;
            var scope = new LeadScope
            {
                SeeAll = seeAll,
                AgentId = _currentUserService.AgentId
            };

            var (total, rows) = await _leadRepository.SearchAsync(request, scope);
            var summary = await _leadRepository.GetSummaryAsync(scope);

            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

            return new GetLeadsResult
            {
                Total = total,
                Page = page,
                PageSize = pageSize,
                Items = rows,
                Summary = summary
            };
        }
    }
}
