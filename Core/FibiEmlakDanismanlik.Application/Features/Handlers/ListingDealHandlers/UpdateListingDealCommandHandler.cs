using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Commands.ListingDealCommands;
using FibiEmlakDanismanlik.Application.Helpers;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.ListingDealInterfaces;
using FibiEmlakDanismanlik.Domain.Enums;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ListingDealHandlers
{
    public class UpdateListingDealCommandHandler : IRequestHandler<UpdateListingDealCommand, bool>
    {
        private readonly IListingDealRepository _listingDealRepository;
        private readonly ICurrentUserService _currentUserService;

        public UpdateListingDealCommandHandler(
            IListingDealRepository listingDealRepository,
            ICurrentUserService currentUserService)
        {
            _listingDealRepository = listingDealRepository;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(UpdateListingDealCommand request, CancellationToken cancellationToken)
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

            if (deal.Status == DealStatus.Cancelled)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure("ListingDealId", "İptal edilmiş işlem düzenlenemez.")
                });
            }

            deal.AgreedPrice = request.AgreedPrice;
            deal.DepositAmount = request.DepositAmount;
            deal.DepositDate = request.DepositDate;
            deal.BuyerCommissionRate = request.BuyerCommissionRate;
            deal.SellerCommissionRate = request.SellerCommissionRate;
            deal.BuyerCommissionAmount = request.BuyerCommissionAmount;
            deal.SellerCommissionAmount = request.SellerCommissionAmount;
            deal.CustomerName = request.CustomerName.Trim();
            deal.CustomerPhone = string.IsNullOrWhiteSpace(request.CustomerPhone) ? null : request.CustomerPhone.Trim();
            deal.Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim();

            ListingDealCalculator.ApplyCommission(deal);
            ListingDealCalculator.RecalculateShares(deal);
            ListingDealCalculator.EnsureSharesWithinCommission(deal, "BuyerCommissionAmount");

            deal.UpdatedAtUtc = DateTime.UtcNow;

            await _listingDealRepository.SaveChangesAsync();
            return true;
        }
    }
}
