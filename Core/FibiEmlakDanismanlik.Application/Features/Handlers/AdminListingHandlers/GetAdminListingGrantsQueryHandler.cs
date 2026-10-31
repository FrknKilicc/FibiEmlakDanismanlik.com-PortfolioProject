using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Features.Queries.AdminListingQueries;
using FibiEmlakDanismanlik.Application.Features.Results.AdminListingResults;
using FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces;
using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.AdminListingHandlers
{
    /// <summary>
    /// Bir ilana verilmiş danışman erişim izinlerini listeleyen sorgu işleyicisi (Yalnızca Sahip/Yönetici).
    /// </summary>
    public class GetAdminListingGrantsQueryHandler : IRequestHandler<GetAdminListingGrantsQuery, List<GetAdminListingGrantResult>>
    {
        private readonly IAdminListingRepository _adminListingRepository;

        public GetAdminListingGrantsQueryHandler(IAdminListingRepository adminListingRepository)
        {
            _adminListingRepository = adminListingRepository;
        }

        public async Task<List<GetAdminListingGrantResult>> Handle(GetAdminListingGrantsQuery request, CancellationToken cancellationToken)
        {
            var listingAgentId = await _adminListingRepository.GetListingAgentIdAsync(request.ListingTypeId, request.ListingId);
            if (!listingAgentId.HasValue)
            {
                throw new KeyNotFoundException("İlan bulunamadı.");
            }

            var grants = await _adminListingRepository.GetGrantsForListingAsync(request.ListingTypeId, request.ListingId);
            var results = new List<GetAdminListingGrantResult>();

            foreach (var grant in grants)
            {
                var agent = await _adminListingRepository.GetAgentByIdAsync(grant.AgentId);
                results.Add(new GetAdminListingGrantResult
                {
                    ListingAccessGrantId = grant.ListingAccessGrantId,
                    AgentId = grant.AgentId,
                    AgentName = agent?.AgentName ?? string.Empty,
                    Access = grant.Access == ListingAccessLevel.Edit ? "Edit" : "View",
                    GrantedAt = grant.GrantedAtUtc
                });
            }

            return results;
        }
    }
}
