using System;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.BusinessSettingCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.BusinessSettingInterfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.BusinessSettingHandlers
{
    /// <summary>
    /// İş kuralı ayarlarını güncelleyen komut işleyicisi.
    /// </summary>
    public class UpdateBusinessSettingsCommandHandler : IRequestHandler<UpdateBusinessSettingsCommand>
    {
        private readonly IBusinessSettingRepository _businessSettingRepository;
        private readonly ICurrentUserService _currentUserService;

        public UpdateBusinessSettingsCommandHandler(
            IBusinessSettingRepository businessSettingRepository,
            ICurrentUserService currentUserService)
        {
            _businessSettingRepository = businessSettingRepository;
            _currentUserService = currentUserService;
        }

        public async Task Handle(UpdateBusinessSettingsCommand request, CancellationToken cancellationToken)
        {
            var setting = new BusinessSetting
            {
                BusinessSettingId = BusinessSettingConstants.SingletonId,
                SaleBuyerCommissionRate = request.SaleBuyerCommissionRate,
                SaleSellerCommissionRate = request.SaleSellerCommissionRate,
                RentCommissionMonths = request.RentCommissionMonths,
                BestDealMaxPerUsageType = request.BestDealMaxPerUsageType,
                ListingAlertMaxActivePerPhone = request.ListingAlertMaxActivePerPhone,
                UpdatedAtUtc = DateTime.UtcNow,
                UpdatedByAgentId = _currentUserService.AgentId
            };

            await _businessSettingRepository.SaveAsync(setting);
        }
    }
}
