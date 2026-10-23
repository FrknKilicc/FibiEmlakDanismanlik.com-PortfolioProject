using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Commands.ListingDealCommands;
using FibiEmlakDanismanlik.Application.Helpers;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.ListingDealInterfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ListingDealHandlers
{
    public class AddDealShareCommandHandler : IRequestHandler<AddDealShareCommand, int>
    {
        private readonly IListingDealRepository _listingDealRepository;
        private readonly ICurrentUserService _currentUserService;

        public AddDealShareCommandHandler(
            IListingDealRepository listingDealRepository,
            ICurrentUserService currentUserService)
        {
            _listingDealRepository = listingDealRepository;
            _currentUserService = currentUserService;
        }

        public async Task<int> Handle(AddDealShareCommand request, CancellationToken cancellationToken)
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
                    new ValidationFailure("ListingDealId", "İptal edilmiş işleme prim eklenemez.")
                });
            }

            var agentRole = await _listingDealRepository.GetActiveAgentRoleAsync(request.AgentId);
            if (!agentRole.HasValue)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure("AgentId", "Çalışan bulunamadı veya aktif değil.")
                });
            }

            if (deal.Shares.Any(s => s.AgentId == request.AgentId))
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure("AgentId", "Bu çalışan işleme zaten eklenmiş.")
                });
            }

            if (deal.TotalCommission <= 0)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure("Amount", "Önce işlemin komisyon tutarını girin.")
                });
            }

            var amount = request.SharePercent.HasValue
                ? ListingDealConstants.Percent(deal.TotalCommission, request.SharePercent.Value)
                : (request.Amount ?? 0m);

            var share = new DealShare
            {
                ListingDealId = deal.ListingDealId,
                AgentId = request.AgentId,
                RoleAtDeal = agentRole.Value,
                SharePercent = request.SharePercent,
                Amount = amount,
                PaymentStatus = DealSharePaymentStatus.Pending,
                PaidDate = null,
                Note = string.IsNullOrWhiteSpace(request.Note) ? null : request.Note.Trim(),
                CreatedAtUtc = DateTime.UtcNow
            };

            deal.Shares.Add(share);
            ListingDealCalculator.EnsureSharesWithinCommission(deal, "Amount");

            await _listingDealRepository.SaveChangesAsync();
            return share.DealShareId;
        }
    }
}
