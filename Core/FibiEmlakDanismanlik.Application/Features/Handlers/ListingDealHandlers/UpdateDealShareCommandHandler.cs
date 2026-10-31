using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
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
    public class UpdateDealShareCommandHandler : IRequestHandler<UpdateDealShareCommand, bool>
    {
        private readonly IListingDealRepository _listingDealRepository;
        private readonly ICurrentUserService _currentUserService;

        public UpdateDealShareCommandHandler(
            IListingDealRepository listingDealRepository,
            ICurrentUserService currentUserService)
        {
            _listingDealRepository = listingDealRepository;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(UpdateDealShareCommand request, CancellationToken cancellationToken)
        {
            var isPrivileged = _currentUserService.Role == AgentRole.Owner || _currentUserService.Role == AgentRole.Manager;
            if (!isPrivileged)
            {
                throw new DealAccessDeniedException();
            }

            var share = await _listingDealRepository.GetTrackedShareAsync(request.DealShareId);
            if (share == null)
            {
                throw new KeyNotFoundException("Prim kaydı bulunamadı.");
            }

            var deal = share.ListingDeal;
            if (deal.Status == DealStatus.Cancelled)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure("DealShareId", "İptal edilmiş işlemin primi düzenlenemez.")
                });
            }

            if (share.PaymentStatus == DealSharePaymentStatus.Paid)
            {
                var percentChanged = request.SharePercent.HasValue && request.SharePercent != share.SharePercent;
                var amountChanged = request.Amount.HasValue && request.Amount != share.Amount;

                if (percentChanged || amountChanged)
                {
                    throw new ValidationException(new[]
                    {
                        new ValidationFailure("Amount", "Ödenmiş primin tutarı değiştirilemez; önce ödemeyi geri alın.")
                    });
                }

                share.Note = string.IsNullOrWhiteSpace(request.Note) ? null : request.Note.Trim();
                await _listingDealRepository.SaveChangesAsync();
                return true;
            }

            share.SharePercent = request.SharePercent;
            if (request.SharePercent.HasValue)
            {
                share.Amount = ListingDealConstants.Percent(deal.TotalCommission, request.SharePercent.Value);
            }
            else
            {
                share.Amount = request.Amount ?? 0m;
            }

            share.Note = string.IsNullOrWhiteSpace(request.Note) ? null : request.Note.Trim();

            ListingDealCalculator.EnsureSharesWithinCommission(deal, "Amount");

            await _listingDealRepository.SaveChangesAsync();
            return true;
        }
    }
}
