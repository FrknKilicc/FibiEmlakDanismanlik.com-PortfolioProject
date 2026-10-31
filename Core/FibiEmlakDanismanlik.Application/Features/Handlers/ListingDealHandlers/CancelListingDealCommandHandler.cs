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
using FibiEmlakDanismanlik.Domain.Common;
using FibiEmlakDanismanlik.Domain.Enums;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ListingDealHandlers
{
    public class CancelListingDealCommandHandler : IRequestHandler<CancelListingDealCommand, bool>
    {
        private readonly IListingDealRepository _listingDealRepository;
        private readonly IAdminListingRepository _adminListingRepository;
        private readonly ILeadRepository _leadRepository;
        private readonly ICurrentUserService _currentUserService;

        public CancelListingDealCommandHandler(
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

        public async Task<bool> Handle(CancelListingDealCommand request, CancellationToken cancellationToken)
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
                    new ValidationFailure("ListingDealId", "Yalnızca kapora aşamasındaki işlem iptal edilebilir.")
                });
            }

            if (request.DepositOutcome == DepositOutcome.Refunded)
            {
                if (!request.DepositRefundAmount.HasValue || request.DepositRefundAmount.Value <= 0)
                {
                    throw new ValidationException(new[]
                    {
                        new ValidationFailure("DepositRefundAmount", "Kapora iade tutarı sıfırdan büyük olmalıdır.")
                    });
                }

                var maxDeposit = deal.DepositAmount ?? 0m;
                if (request.DepositRefundAmount.Value > maxDeposit)
                {
                    throw new ValidationException(new[]
                    {
                        new ValidationFailure("DepositRefundAmount", "İade tutarı alınan kapora tutarını aşamaz.")
                    });
                }

                deal.DepositOutcome = DepositOutcome.Refunded;
                deal.DepositRefundAmount = request.DepositRefundAmount.Value;
                deal.DepositRefundDate = request.DepositRefundDate ?? TurkeyClock.Today;
            }
            else
            {
                deal.DepositOutcome = DepositOutcome.Kept;
                deal.DepositRefundAmount = null;
                deal.DepositRefundDate = null;
            }

            deal.Status = DealStatus.Cancelled;
            deal.CancelledDate = request.CancelledDate ?? TurkeyClock.Today;
            deal.UpdatedAtUtc = DateTime.UtcNow;

            await _listingDealRepository.SaveChangesAsync();

            var listingStatus = await _adminListingRepository.GetListingStatusAsync(deal.ListingTypeId, deal.ListingId);
            if (listingStatus == PropertyStatuses.Opsiyonlu)
            {
                await _adminListingRepository.SetStatusAsync(deal.ListingTypeId, deal.ListingId, PropertyStatuses.Aktif);
            }

            if (deal.LinkedLeadSource.HasValue && deal.LinkedLeadId.HasValue)
            {
                await DealLeadSync.AdvanceAsync(
                    _leadRepository,
                    deal.LinkedLeadSource.Value,
                    deal.LinkedLeadId.Value,
                    null,
                    $"#{deal.ListingDealId} numaralı işlem iptal edildi.",
                    _currentUserService.AgentId);
            }

            return true;
        }
    }
}
