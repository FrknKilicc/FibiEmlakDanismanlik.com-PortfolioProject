using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.AdminListingCommands;
using FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.AdminListingHandlers
{
    /// <summary>
    /// İlanın vitrin (BestDeals) durumunu belirleyen komut işleyicisi (Yalnızca Sahip/Yönetici).
    /// </summary>
    public class SetAdminListingBestDealCommandHandler : IRequestHandler<SetAdminListingBestDealCommand, bool>
    {
        private readonly IAdminListingRepository _adminListingRepository;

        public SetAdminListingBestDealCommandHandler(IAdminListingRepository adminListingRepository)
        {
            _adminListingRepository = adminListingRepository;
        }

        public async Task<bool> Handle(SetAdminListingBestDealCommand request, CancellationToken cancellationToken)
        {
            var currentStatus = await _adminListingRepository.GetListingStatusAsync(request.ListingTypeId, request.ListingId);
            if (currentStatus == null)
            {
                throw new KeyNotFoundException("İlan bulunamadı.");
            }

            if (request.IsBestDeal)
            {
                // Vitrine alınırken ilan Aktif veya Opsiyonlu olmalı
                if (!string.Equals(currentStatus, PropertyStatuses.Aktif, StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(currentStatus, PropertyStatuses.Opsiyonlu, StringComparison.OrdinalIgnoreCase))
                {
                    throw new ValidationException(new[]
                    {
                        new ValidationFailure(nameof(request.IsBestDeal), "Yalnızca Aktif veya Opsiyonlu durumdaki ilanlar vitrine eklenebilir.")
                    });
                }

                bool isRent = AdminListingConstants.IsRent(request.ListingTypeId);
                var currentCount = await _adminListingRepository.CountBestDealsAsync(isRent);

                if (currentCount >= BestDealConstants.MaxPerUsageType)
                {
                    var msg = isRent
                        ? $"Kiralık vitrinde zaten {BestDealConstants.MaxPerUsageType} ilan var; önce birini kaldırın."
                        : $"Satılık vitrinde zaten {BestDealConstants.MaxPerUsageType} ilan var; önce birini kaldırın.";

                    throw new ValidationException(new[]
                    {
                        new ValidationFailure(nameof(request.IsBestDeal), msg)
                    });
                }
            }

            return await _adminListingRepository.SetBestDealAsync(request.ListingTypeId, request.ListingId, request.IsBestDeal);
        }
    }
}
