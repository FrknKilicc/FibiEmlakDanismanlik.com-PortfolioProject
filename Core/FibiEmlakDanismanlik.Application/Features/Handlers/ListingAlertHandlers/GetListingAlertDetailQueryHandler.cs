using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Queries.ListingAlertQueries;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.ListingAlertInterfaces;
using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ListingAlertHandlers
{
    /// <summary>
    /// Haber talebi ayrıntılarını yetki kapsamını denetleyerek getiren sorgu işleyicisi.
    /// </summary>
    public class GetListingAlertDetailQueryHandler : IRequestHandler<GetListingAlertDetailQuery, ListingAlertDetail?>
    {
        private readonly IListingAlertRepository _listingAlertRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetListingAlertDetailQueryHandler(IListingAlertRepository listingAlertRepository, ICurrentUserService currentUserService)
        {
            _listingAlertRepository = listingAlertRepository;
            _currentUserService = currentUserService;
        }

        public async Task<ListingAlertDetail?> Handle(GetListingAlertDetailQuery request, CancellationToken cancellationToken)
        {
            var exists = await _listingAlertRepository.ExistsAsync(request.Id);
            if (!exists)
            {
                return null;
            }

            var isPrivileged = _currentUserService.Role == AgentRole.Owner || _currentUserService.Role == AgentRole.Manager;
            if (!isPrivileged)
            {
                var assignedAgentId = await _listingAlertRepository.GetAssignedAgentIdAsync(request.Id);
                if (assignedAgentId != _currentUserService.AgentId)
                {
                    throw new ListingAlertAccessDeniedException("Bu haber talebinin ayrıntılarını görüntüleme yetkiniz yok.");
                }
            }

            return await _listingAlertRepository.GetDetailAsync(request.Id);
        }
    }
}
