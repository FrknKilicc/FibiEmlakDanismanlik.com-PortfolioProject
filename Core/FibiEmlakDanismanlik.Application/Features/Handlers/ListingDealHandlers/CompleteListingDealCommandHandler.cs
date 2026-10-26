using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Commands.ListingDealCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.LeadInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.ListingDealInterfaces;
using FibiEmlakDanismanlik.Domain.Enums;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ListingDealHandlers
{
    public class CompleteListingDealCommandHandler : IRequestHandler<CompleteListingDealCommand, bool>
    {
        private readonly IListingDealRepository _listingDealRepository;
        private readonly IAdminListingRepository _adminListingRepository;
        private readonly ILeadRepository _leadRepository;
        private readonly ICurrentUserService _currentUserService;

        public CompleteListingDealCommandHandler(
            IListingDealRepository listingDealRepository,
            IAdminListingRepository adminListingRepository,
            ILeadRepository leadRepository,
            ICurrentUserService currentUserService)
        {
            _listingDealRepository = listingDealRepository;
            _adminListingRepository = adminListingRepository;
            _leadRepository = leadRepository;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(CompleteListingDealCommand request, CancellationToken cancellationToken)
        {
            var isPrivileged = _currentUserService.Role == AgentRole.Owner || _currentUserService.Role == AgentRole.Manager;
            if (!isPrivileged)
            {
                throw new DealAccessDeniedException();
            }

            var deal = await _listingDealRepository.GetTrackedAsync(request.ListingDealId);
            if (deal == null)
            {
                throw new KeyNotFoundException("İşlem kaydı bulunamadı.");
            }

            if (deal.Status != DealStatus.DepositReceived)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure("ListingDealId", "Yalnızca kapora aşamasındaki işlem tamamlanabilir.")
                });
            }

            deal.Status = DealStatus.Completed;
            deal.CompletedDate = request.CompletedDate ?? DateTime.Today;
            deal.UpdatedAtUtc = DateTime.UtcNow;

            await _listingDealRepository.SaveChangesAsync();

            var newStatus = deal.DealType == DealType.Sale ? PropertyStatuses.Satildi : PropertyStatuses.Kiralandi;
            await _adminListingRepository.SetStatusAsync(deal.ListingTypeId, deal.ListingId, newStatus);

            if (deal.LinkedLeadSource.HasValue && deal.LinkedLeadId.HasValue)
            {
                await DealLeadSync.AdvanceAsync(
                    _leadRepository,
                    deal.LinkedLeadSource.Value,
                    deal.LinkedLeadId.Value,
                    LeadStatus.Won,
                    $"#{deal.ListingDealId} numaralı işlem tamamlandı.",
                    _currentUserService.AgentId);
            }

            return true;
        }
    }
}
