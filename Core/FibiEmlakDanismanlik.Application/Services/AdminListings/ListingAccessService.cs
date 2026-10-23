using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces;
using FibiEmlakDanismanlik.Domain.Enums;

namespace FibiEmlakDanismanlik.Application.Services.AdminListings
{
    /// <summary>
    /// Yönetim panelinde ilan erişim kurallarını ve kullanıcı kapsamını denetleyen servis.
    /// </summary>
    public class ListingAccessService : IListingAccessService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IAdminListingRepository _adminListingRepository;

        public ListingAccessService(
            ICurrentUserService currentUserService,
            IAdminListingRepository adminListingRepository)
        {
            _currentUserService = currentUserService;
            _adminListingRepository = adminListingRepository;
        }

        public async Task<ListingAccess> GetAccessAsync(int listingTypeId, int listingId)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.AgentId.HasValue || !_currentUserService.Role.HasValue)
            {
                return ListingAccess.None;
            }

            // İlanın veritabanında var olup olmadığını ve kime ait olduğunu kontrol et
            var listingAgentId = await _adminListingRepository.GetListingAgentIdAsync(listingTypeId, listingId);
            if (!listingAgentId.HasValue)
            {
                return ListingAccess.None;
            }

            var role = _currentUserService.Role.Value;
            var currentAgentId = _currentUserService.AgentId.Value;

            // Owner ve Manager tüm mevcut ilanları görebilir ve düzenleyebilir
            if (role == AgentRole.Owner || role == AgentRole.Manager)
            {
                return ListingAccess.Edit;
            }

            if (role == AgentRole.Consultant)
            {
                // Danışmanın kendi ilanı ise hem görür hem düzenler
                if (listingAgentId.Value == currentAgentId)
                {
                    return ListingAccess.Edit;
                }

                // Özel erişim izni (grant) kontrolü
                var grants = await _adminListingRepository.GetGrantsForAgentAsync(currentAgentId);
                var grant = grants.FirstOrDefault(g => g.ListingTypeId == listingTypeId && g.ListingId == listingId);
                if (grant != null)
                {
                    if (grant.Access == ListingAccessLevel.Edit || (int)grant.Access == 2)
                    {
                        return ListingAccess.Edit;
                    }

                    if (grant.Access == ListingAccessLevel.View || (int)grant.Access == 1)
                    {
                        return ListingAccess.View;
                    }
                }

                // Tüm ilanları görme yetkisi varsa yalnızca görebilir
                var canViewAll = await _adminListingRepository.GetCanViewAllListingsAsync(currentAgentId);
                if (canViewAll)
                {
                    return ListingAccess.View;
                }
            }

            return ListingAccess.None;
        }

        public async Task<ListingScope> GetScopeAsync()
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.AgentId.HasValue || !_currentUserService.Role.HasValue)
            {
                return new ListingScope
                {
                    SeeAll = false,
                    OwnAgentId = null,
                    GrantedListings = Array.Empty<(int, int)>()
                };
            }

            var role = _currentUserService.Role.Value;
            var currentAgentId = _currentUserService.AgentId.Value;

            // Owner ve Manager tüm ilanları görebilir
            if (role == AgentRole.Owner || role == AgentRole.Manager)
            {
                return new ListingScope
                {
                    SeeAll = true,
                    OwnAgentId = currentAgentId
                };
            }

            if (role == AgentRole.Consultant)
            {
                // CanViewAllListings yetkisi varsa tüm ilanları görebilir
                var canViewAll = await _adminListingRepository.GetCanViewAllListingsAsync(currentAgentId);
                if (canViewAll)
                {
                    return new ListingScope
                    {
                        SeeAll = true,
                        OwnAgentId = currentAgentId
                    };
                }

                // Aksi halde kendi ilanları ve grant verilmiş ilanlar
                var grants = await _adminListingRepository.GetGrantsForAgentAsync(currentAgentId);
                var grantedListings = grants.Select(g => (g.ListingTypeId, g.ListingId)).ToList();

                return new ListingScope
                {
                    SeeAll = false,
                    OwnAgentId = currentAgentId,
                    GrantedListings = grantedListings
                };
            }

            return new ListingScope
            {
                SeeAll = false,
                OwnAgentId = null,
                GrantedListings = Array.Empty<(int, int)>()
            };
        }
    }
}
