using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Commands.ListingDealCommands;
using FibiEmlakDanismanlik.Application.Helpers;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.LeadInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.ListingDealInterfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ListingDealHandlers
{
    public class CreateListingDealCommandHandler : IRequestHandler<CreateListingDealCommand, int>
    {
        private readonly IListingDealRepository _listingDealRepository;
        private readonly IAdminListingRepository _adminListingRepository;
        private readonly ILeadRepository _leadRepository;
        private readonly ICurrentUserService _currentUserService;

        public CreateListingDealCommandHandler(
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

        public async Task<int> Handle(CreateListingDealCommand request, CancellationToken cancellationToken)
        {
            var isPrivileged = _currentUserService.Role == AgentRole.Owner || _currentUserService.Role == AgentRole.Manager;
            if (!isPrivileged)
            {
                throw new DealAccessDeniedException();
            }

            var listingStatus = await _adminListingRepository.GetListingStatusAsync(request.ListingTypeId, request.ListingId);
            if (listingStatus == null)
            {
                throw new KeyNotFoundException("İlan bulunamadı.");
            }

            if (listingStatus == PropertyStatuses.Satildi || listingStatus == PropertyStatuses.Kiralandi)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure("ListingId", "Bu ilan zaten satıldı/kiralandı.")
                });
            }

            var hasOpenDeal = await _listingDealRepository.HasOpenDealAsync(request.ListingTypeId, request.ListingId);
            if (hasOpenDeal)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure("ListingId", "Bu ilan için açık (kapora alınmış) bir işlem zaten var.")
                });
            }

            if (request.LinkedLeadSource.HasValue && request.LinkedLeadId.HasValue)
            {
                var lead = await _leadRepository.GetDetailAsync(request.LinkedLeadSource.Value, request.LinkedLeadId.Value);
                if (lead == null)
                {
                    throw new ValidationException(new[]
                    {
                        new ValidationFailure("LinkedLeadId", "Seçilen talep bulunamadı.")
                    });
                }

                if (lead.ListingId.HasValue && (lead.ListingTypeId != request.ListingTypeId || lead.ListingId != request.ListingId))
                {
                    throw new ValidationException(new[]
                    {
                        new ValidationFailure("LinkedLeadId", "Seçilen talep bu ilana ait değil.")
                    });
                }
            }

            var deal = new ListingDeal
            {
                ListingTypeId = request.ListingTypeId,
                ListingId = request.ListingId,
                DealType = AdminListingConstants.IsRent(request.ListingTypeId) ? DealType.Rent : DealType.Sale,
                Status = DealStatus.DepositReceived,
                AgreedPrice = request.AgreedPrice,
                DepositAmount = request.DepositAmount,
                DepositDate = request.DepositDate,
                BuyerCommissionRate = request.BuyerCommissionRate,
                SellerCommissionRate = request.SellerCommissionRate,
                BuyerCommissionAmount = request.BuyerCommissionAmount,
                SellerCommissionAmount = request.SellerCommissionAmount,
                CustomerName = request.CustomerName.Trim(),
                CustomerPhone = string.IsNullOrWhiteSpace(request.CustomerPhone) ? null : request.CustomerPhone.Trim(),
                Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
                LinkedLeadSource = request.LinkedLeadSource,
                LinkedLeadId = request.LinkedLeadId,
                CreatedByAgentId = _currentUserService.AgentId,
                CreatedAtUtc = DateTime.UtcNow
            };

            ListingDealCalculator.ApplyCommission(deal);

            await _listingDealRepository.AddAsync(deal);

            if (listingStatus == PropertyStatuses.Aktif)
            {
                await _adminListingRepository.SetStatusAsync(request.ListingTypeId, request.ListingId, PropertyStatuses.Opsiyonlu);
            }

            if (deal.LinkedLeadSource.HasValue && deal.LinkedLeadId.HasValue)
            {
                await DealLeadSync.AdvanceAsync(
                    _leadRepository,
                    deal.LinkedLeadSource.Value,
                    deal.LinkedLeadId.Value,
                    LeadStatus.Deposit,
                    $"#{deal.ListingDealId} numaralı işlem açıldı (kapora).",
                    _currentUserService.AgentId);
            }

            return deal.ListingDealId;
        }
    }
}
