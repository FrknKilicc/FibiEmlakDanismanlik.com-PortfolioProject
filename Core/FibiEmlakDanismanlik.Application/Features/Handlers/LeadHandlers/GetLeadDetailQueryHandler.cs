using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Queries.LeadQueries;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.LeadInterfaces;
using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.LeadHandlers
{
    /// <summary>
    /// Müşteri talebi detayını yetki kapsamını denetleyerek getiren sorgu işleyicisi.
    /// </summary>
    public class GetLeadDetailQueryHandler : IRequestHandler<GetLeadDetailQuery, LeadDetail?>
    {
        private readonly ILeadRepository _leadRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetLeadDetailQueryHandler(ILeadRepository leadRepository, ICurrentUserService currentUserService)
        {
            _leadRepository = leadRepository;
            _currentUserService = currentUserService;
        }

        public async Task<LeadDetail?> Handle(GetLeadDetailQuery request, CancellationToken cancellationToken)
        {
            var ownership = await _leadRepository.GetOwnershipAsync(request.Source, request.LeadId);
            if (ownership == null)
            {
                return null;
            }

            var isPrivileged = _currentUserService.Role == AgentRole.Owner || _currentUserService.Role == AgentRole.Manager;
            if (!isPrivileged)
            {
                var currentAgentId = _currentUserService.AgentId;
                var isAssigned = ownership.Value.AssignedAgentId.HasValue && ownership.Value.AssignedAgentId == currentAgentId;
                var isListingOwner = ownership.Value.ListingAgentId.HasValue && ownership.Value.ListingAgentId == currentAgentId;

                if (!isAssigned && !isListingOwner)
                {
                    throw new LeadAccessDeniedException("Bu talebin ayrıntılarını görüntüleme yetkiniz yok.");
                }
            }

            var detail = await _leadRepository.GetDetailAsync(request.Source, request.LeadId);
            if (detail != null && !isPrivileged)
            {
                detail.LinkedDealId = null;
            }

            return detail;
        }
    }
}
