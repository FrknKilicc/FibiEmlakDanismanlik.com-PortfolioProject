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

            if (partnerName.Length > 100)
            {
                throw new ArgumentException("Partner adı en fazla 100 karakter olabilir.");
            }

            if (partnerLogoUrl.Length > 500)
            {
                throw new ArgumentException("Logo adresi en fazla 500 karakter olabilir.");
            }

            if (!partnerLogoUrl.StartsWith("/") && !partnerLogoUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Logo adresi '/' ile başlayan bir yol ya da https adresi olmalıdır.");
            }

            var entity = new OurPartners
            {
                PartnerName = partnerName,
                PartnerLogoUrl = partnerLogoUrl
            };

            await _repository.CreateAsync(entity);
        }
    }
}
