using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Queries.LeadQueries;
using FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.LeadInterfaces;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.LeadHandlers
{
    /// <summary>
    /// Belirli bir ilana ait müşteri taleplerini ilan erişim düzeyini denetleyerek getiren sorgu işleyicisi.
    /// </summary>
    public class GetListingLeadsQueryHandler : IRequestHandler<GetListingLeadsQuery, List<LeadRow>>
    {
        private readonly ILeadRepository _leadRepository;
        private readonly IListingAccessService _listingAccessService;

        public GetListingLeadsQueryHandler(ILeadRepository leadRepository, IListingAccessService listingAccessService)
        {
            _leadRepository = leadRepository;
            _listingAccessService = listingAccessService;
        }

        public async Task<List<LeadRow>> Handle(GetListingLeadsQuery request, CancellationToken cancellationToken)
        {
            var access = await _listingAccessService.GetAccessAsync(request.ListingTypeId, request.ListingId);
            if (access == ListingAccess.None)
            {
                throw new ListingAccessDeniedException("Bu ilanın taleplerini görüntüleme yetkiniz yok.");
            }

            return await _leadRepository.GetByListingAsync(request.ListingTypeId, request.ListingId);
        }
    }
}
