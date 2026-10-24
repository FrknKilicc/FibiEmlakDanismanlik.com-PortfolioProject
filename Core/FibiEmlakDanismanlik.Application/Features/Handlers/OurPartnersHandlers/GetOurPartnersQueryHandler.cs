using FibiEmlakDanismanlik.Application.Features.Queries.OurPartnersQueries;
using FibiEmlakDanismanlik.Application.Features.Results.OurPartnersResults;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.OurPartnersHandlers
{
    public class GetOurPartnersQueryHandler : IRequestHandler<GetOurPartnersQuery, List<GetOurPartnersResult>>
    {
        private readonly IRepository<OurPartners> _repository;

        public GetOurPartnersQueryHandler(IRepository<OurPartners> repository)
        {
            _repository = repository;
        }

        public async Task<List<GetOurPartnersResult>> Handle(GetOurPartnersQuery request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetAllAsync();
            var query = value.AsEnumerable();

            if (!request.IncludeInactive)
            {
                query = query.Where(x => x.IsActive);
            }

            return query
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.Id)
                .Select(x => new GetOurPartnersResult
                {
                    Id = x.Id,
                    PartnerLogoUrl = x.PartnerLogoUrl,
                    PartnerName = x.PartnerName,
                    IsActive = x.IsActive,
                    DisplayOrder = x.DisplayOrder
                }).ToList();
        }
    }
}
