using FibiEmlakDanismanlik.Application.Features.Commands.OurPartnersCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.OurPartnersHandlers
{
    public class RemoveOurPartnerCommandHandler : IRequestHandler<RemoveOurPartnerCommand>
    {
        private readonly IRepository<OurPartners> _repository;

        public RemoveOurPartnerCommandHandler(IRepository<OurPartners> repository)
        {
            _repository = repository;
        }

        public async Task Handle(RemoveOurPartnerCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Partner bulunamadı.");
            }

            await _repository.RemoveAsync(entity);
        }
    }
}
