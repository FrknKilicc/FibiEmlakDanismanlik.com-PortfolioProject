using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.AdminListingCommands;
using FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.BusinessSettingInterfaces;
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
        private readonly IBusinessSettingRepository _businessSettingRepository;

        public SetAdminListingBestDealCommandHandler(
            IAdminListingRepository adminListingRepository,
            IBusinessSettingRepository businessSettingRepository)
        {
            _adminListingRepository = adminListingRepository;
            _businessSettingRepository = businessSettingRepository;
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
                var maxBestDeals = (await _businessSettingRepository.GetAsync()).BestDealMaxPerUsageType;

                if (currentCount >= maxBestDeals)
                {
                    var msg = isRent
                        ? $"Kiralık vitrinde zaten {maxBestDeals} ilan var; önce birini kaldırın."
                        : $"Satılık vitrinde zaten {maxBestDeals} ilan var; önce birini kaldırın.";

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
