using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Features.Queries.BusinessSettingQueries;
using FibiEmlakDanismanlik.Application.Features.Results.BusinessSettingResults;
using FibiEmlakDanismanlik.Application.Interfaces.BusinessSettingInterfaces;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.BusinessSettingHandlers
{
    /// <summary>
    /// İş kuralı ayarlarını getiren sorgu işleyicisi.
    /// </summary>
    public class GetBusinessSettingsQueryHandler : IRequestHandler<GetBusinessSettingsQuery, GetBusinessSettingsResult>
    {
        private readonly IBusinessSettingRepository _repository;

        public GetBusinessSettingsQueryHandler(IBusinessSettingRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetBusinessSettingsResult> Handle(GetBusinessSettingsQuery request, CancellationToken cancellationToken)
        {
            var setting = await _repository.GetAsync();

            return new GetBusinessSettingsResult
            {
                SaleBuyerCommissionRate = setting.SaleBuyerCommissionRate,
                SaleSellerCommissionRate = setting.SaleSellerCommissionRate,
                RentCommissionMonths = setting.RentCommissionMonths,
                BestDealMaxPerUsageType = setting.BestDealMaxPerUsageType,
                ListingAlertMaxActivePerPhone = setting.ListingAlertMaxActivePerPhone,
                UpdatedAtUtc = setting.UpdatedAtUtc,
                UpdatedByAgentId = setting.UpdatedByAgentId
            };
        }
    }
}
