using FibiEmlakDanismanlik.Application.Features.Commands.CustomerContactCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.CustomerContactHandlers
{
    public class CreateCustomerContactCommandHandler : IRequestHandler<CreateCustomerContactCommand>
    {
        private readonly IRepository<CustomerContact> _repository;
        private readonly IAdminListingRepository _adminListingRepository;

        public CreateCustomerContactCommandHandler(
            IRepository<CustomerContact> repository,
            IAdminListingRepository adminListingRepository)
        {
            _repository = repository;
            _adminListingRepository = adminListingRepository;
        }

        public async Task Handle(CreateCustomerContactCommand request, CancellationToken cancellationToken)
        {
            int? assignedAgentId = null;
            if (request.PropertyListingId.HasValue && request.PropertyListingTypeEnum.HasValue)
            {
                // Randevu talebi bir ilana bağlıysa, ilanın danışmanı otomatik atanır
                assignedAgentId = await _adminListingRepository.GetListingAgentIdAsync(
                    (int)request.PropertyListingTypeEnum.Value,
                    request.PropertyListingId.Value);
            }

            await _repository.CreateAsync(new CustomerContact
            {
                CustomerContactName = request.CustomerContactName,
                CustomerContactMail = string.IsNullOrWhiteSpace(request.CustomerContactMail) ? null : request.CustomerContactMail.Trim(),
                CustomerContactPhone = request.CustomerContactPhone,
                CustomerContactMessage = request.CustomerContactMessage ?? string.Empty,
                PropertyListingId = request.PropertyListingId,
                AppointmentDateTime = request.AppointmentDateTime,
                PropertyListingTypeEnum = request.PropertyListingTypeEnum,
                LeadStatus = LeadStatus.New,
                AssignedAgentId = assignedAgentId,
                KvkkApprovedAt = DateTime.UtcNow
            });
        }
    }
}
