using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Commands.ListingDealCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.ListingDealInterfaces;
using FibiEmlakDanismanlik.Domain.Common;
using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ListingDealHandlers
{
    public class SetDealSharePaymentCommandHandler : IRequestHandler<SetDealSharePaymentCommand, bool>
    {
        private readonly IListingDealRepository _listingDealRepository;
        private readonly ICurrentUserService _currentUserService;

        public SetDealSharePaymentCommandHandler(
            IListingDealRepository listingDealRepository,
            ICurrentUserService currentUserService)
        {
            _listingDealRepository = listingDealRepository;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(SetDealSharePaymentCommand request, CancellationToken cancellationToken)
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

            if (request.Paid)
            {
                share.PaymentStatus = DealSharePaymentStatus.Paid;
                share.PaidDate = request.PaidDate ?? TurkeyClock.Today;
            }
            else
            {
                share.PaymentStatus = DealSharePaymentStatus.Pending;
                share.PaidDate = null;
            }

            await _listingDealRepository.SaveChangesAsync();
            return true;
        }
    }
}
