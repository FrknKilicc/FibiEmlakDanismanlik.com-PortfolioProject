using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Queries.BestDealQueries;
using FibiEmlakDanismanlik.Application.Features.Results.BestDealResults;
using FibiEmlakDanismanlik.Application.Interfaces.BestDealInterfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.BestDealHandlers
{
    public class GetBestDealsQueryHandler : IRequestHandler<GetBestDealsQuery, List<GetBestDealResult>>
    {
        private readonly IBestDealRepository _repository;

        public GetBestDealsQueryHandler(IBestDealRepository repository)
        {
            _repository = repository;
        }

        public Task<List<GetBestDealResult>> Handle(GetBestDealsQuery request, CancellationToken cancellationToken)
        {
            return _repository.GetBestDealsAsync(request.UsageType, BestDealConstants.MaxPerUsageType);
        }
    }
}
