using FibiEmlakDanismanlik.Application.Features.Commands.OurPartnersCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.OurPartnersHandlers
{
    public class UpdateOurPartnerCommandHandler : IRequestHandler<UpdateOurPartnerCommand>
    {
        private readonly IRepository<OurPartners> _repository;

        public UpdateOurPartnerCommandHandler(IRepository<OurPartners> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateOurPartnerCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Partner bulunamadı.");
            }

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

            entity.PartnerName = partnerName;
            entity.PartnerLogoUrl = partnerLogoUrl;

            await _repository.UpdateAsync(entity);
        }
    }
}
