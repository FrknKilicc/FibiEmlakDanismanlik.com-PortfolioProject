using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Queries.AdminListingQueries;
using FibiEmlakDanismanlik.Application.Features.Results.AdminListingResults;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces;
using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.AdminListingHandlers
{
    /// <summary>
    /// Yönetim paneli ilan arayüzü referans ve filtre verilerini getiren işleyici.
    /// </summary>
    public class GetAdminListingLookupsQueryHandler : IRequestHandler<GetAdminListingLookupsQuery, GetAdminListingLookupsResult>
    {
        private readonly IAdminListingRepository _adminListingRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetAdminListingLookupsQueryHandler(
            IAdminListingRepository adminListingRepository,
            ICurrentUserService currentUserService)
        {
            _adminListingRepository = adminListingRepository;
            _currentUserService = currentUserService;
        }

        public async Task<GetAdminListingLookupsResult> Handle(GetAdminListingLookupsQuery request, CancellationToken cancellationToken)
        {
            var amenities = await _adminListingRepository.GetAmenitiesAsync();
            var housingCategories = await _adminListingRepository.GetHousingCategoriesAsync();
            var landCategories = await _adminListingRepository.GetLandCategoriesAsync();

            var amenityItems = amenities.Select(a => new AmenityLookupItem
            {
                Id = a.AmenityId,
                Name = a.Name,
                GroupName = a.GroupName,
                AppliesToHousing = a.AppliesToHousing,
                AppliesToLand = a.AppliesToLand,
                AppliesToCommercial = a.AppliesToCommercial
            }).ToList();

            // Aktif danışmanlar yalnızca Owner ve Manager rolleri için doldurulur; Consultant için boş liste döner
            var isManagerOrAbove = _currentUserService.Role == AgentRole.Owner || _currentUserService.Role == AgentRole.Manager;
            var agentItems = new List<AgentLookupItem>();

            if (isManagerOrAbove)
            {
                var activeAgents = await _adminListingRepository.GetActiveAgentsAsync();
                agentItems = activeAgents.Select(a => new AgentLookupItem
                {
                    AgentId = a.AgentId,
                    AgentName = a.AgentName
                }).ToList();
            }

            var statuses = AdminListingConstants.AllStatuses.ToList();

            return new GetAdminListingLookupsResult
            {
                Amenities = amenityItems,
                HousingCategories = housingCategories,
                LandCategories = landCategories,
                Agents = agentItems,
                Statuses = statuses
            };
        }
    }
}
