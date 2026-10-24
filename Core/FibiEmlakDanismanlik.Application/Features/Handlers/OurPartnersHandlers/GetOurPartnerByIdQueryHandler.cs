using FibiEmlakDanismanlik.Application.Features.Queries.OurPartnersQueries;
using FibiEmlakDanismanlik.Application.Features.Results.OurPartnersResults;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.OurPartnersHandlers
{
    public class GetOurPartnerByIdQueryHandler : IRequestHandler<GetOurPartnerByIdQuery, GetOurPartnerByIdResult?>
    {
        private readonly IRepository<OurPartners> _repository;

        public GetOurPartnerByIdQueryHandler(IRepository<OurPartners> repository)
        {
            _repository = repository;
        }

        public async Task<GetOurPartnerByIdResult?> Handle(GetOurPartnerByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                return null;
            }

            return new GetOurPartnerByIdResult
            {
                Id = entity.Id,
                PartnerName = entity.PartnerName,
                PartnerLogoUrl = entity.PartnerLogoUrl,
                IsActive = entity.IsActive,
                DisplayOrder = entity.DisplayOrder
            };
        }
    }
}
