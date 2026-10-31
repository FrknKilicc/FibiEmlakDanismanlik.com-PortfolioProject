using FibiEmlakDanismanlik.Application.Features.Commands.FibiServiceCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Common;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.FibiServiceHandlers
{
    public class CreateFibiServiceLeadCommandHandler : IRequestHandler<CreateFibiServiceLeadCommand>
    {
        private readonly IRepository<FibiServiceLead> _repository;

        public CreateFibiServiceLeadCommandHandler(IRepository<FibiServiceLead> repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateFibiServiceLeadCommand request, CancellationToken cancellationToken)
        {
            var email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
            var customerNote = string.IsNullOrWhiteSpace(request.CustomerNote) ? null : request.CustomerNote.Trim();
            var calculationSummary = string.IsNullOrWhiteSpace(request.CalculationSummaryJson) ? null : request.CalculationSummaryJson.Trim();

            var now = TurkeyClock.Now;
            var lead = new FibiServiceLead
            {
                SourceModule = request.SourceModule,
                FullName = request.FullName.Trim(),
                PhoneNumber = request.PhoneNumber.Trim(),
                Email = email,
                NeighborhoodId = request.NeighborhoodId,
                CalculationSummaryJson = calculationSummary,
                CustomerNote = customerNote,
                CreatedDate = now,
                KvkkApprovedAt = now,
                IsContacted = false
            };

            await _repository.CreateAsync(lead);
        }
    }
}
