using FibiEmlakDanismanlik.Application.Features.Commands.OurPartnersCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.OurPartnersHandlers
{
    public class CreateOurPartnerCommandHandler : IRequestHandler<CreateOurPartnerCommand>
    {
        private readonly IRepository<OurPartners> _repository;

        public CreateOurPartnerCommandHandler(IRepository<OurPartners> repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateOurPartnerCommand request, CancellationToken cancellationToken)
        {
            var partnerName = request.PartnerName?.Trim();
            var partnerLogoUrl = request.PartnerLogoUrl?.Trim();

            if (string.IsNullOrWhiteSpace(partnerName) || string.IsNullOrWhiteSpace(partnerLogoUrl))
            {
                throw new ArgumentException("Partner adı ve logo adresi zorunludur.");
            }

            var entity = new OurPartners
            {
                PartnerName = partnerName,
                PartnerLogoUrl = partnerLogoUrl,
                IsActive = request.IsActive,
                DisplayOrder = request.DisplayOrder
            };

            await _repository.CreateAsync(entity);
        }
    }
}
