using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Commands.ListingDealCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.ListingDealInterfaces;
using FibiEmlakDanismanlik.Domain.Enums;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ListingDealHandlers
{
    public class RemoveDealShareCommandHandler : IRequestHandler<RemoveDealShareCommand, bool>
    {
        private readonly IListingDealRepository _listingDealRepository;
        private readonly ICurrentUserService _currentUserService;

        public RemoveDealShareCommandHandler(
            IListingDealRepository listingDealRepository,
            ICurrentUserService currentUserService)
        {
            _listingDealRepository = listingDealRepository;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(RemoveDealShareCommand request, CancellationToken cancellationToken)
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

            if (share.PaymentStatus == DealSharePaymentStatus.Paid)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure("DealShareId", "Ödenmiş prim silinemez; önce ödemeyi geri alın.")
                });
            }

            _listingDealRepository.RemoveShare(share);
            await _listingDealRepository.SaveChangesAsync();
            return true;
        }
    }
}
