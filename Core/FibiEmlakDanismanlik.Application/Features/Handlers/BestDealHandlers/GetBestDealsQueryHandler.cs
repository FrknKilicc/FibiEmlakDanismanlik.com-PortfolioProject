using FibiEmlakDanismanlik.Application.Features.Queries.BestDealQueries;
using FibiEmlakDanismanlik.Application.Features.Results.BestDealResults;
using FibiEmlakDanismanlik.Application.Interfaces.BestDealInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.BusinessSettingInterfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.BestDealHandlers
{
    public class GetBestDealsQueryHandler : IRequestHandler<GetBestDealsQuery, List<GetBestDealResult>>
    {
        private readonly IBestDealRepository _repository;
        private readonly IBusinessSettingRepository _businessSettingRepository;

        public GetBestDealsQueryHandler(
            IBestDealRepository repository,
            IBusinessSettingRepository businessSettingRepository)
        {
            _repository = repository;
            _businessSettingRepository = businessSettingRepository;
        }

        public async Task<List<GetBestDealResult>> Handle(GetBestDealsQuery request, CancellationToken cancellationToken)
        {
            var settings = await _businessSettingRepository.GetAsync();
            return await _repository.GetBestDealsAsync(request.UsageType, settings.BestDealMaxPerUsageType);
        }
    }
}
