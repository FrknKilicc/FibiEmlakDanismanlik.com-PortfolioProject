using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Queries.FibiServiceQueries;
using FibiEmlakDanismanlik.Application.Features.Results.FibiServiceResults;
using FibiEmlakDanismanlik.Application.Helpers;
using FibiEmlakDanismanlik.Application.Interfaces.FibiServiceInterfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.FibiServiceHandlers
{
    public class GetNeighborhoodAnalysisQueryHandler : IRequestHandler<GetNeighborhoodAnalysisQuery, GetNeighborhoodAnalysisResult?>
    {
        private readonly IMarketStatRepository _marketStatRepository;

        public GetNeighborhoodAnalysisQueryHandler(IMarketStatRepository marketStatRepository)
        {
            _marketStatRepository = marketStatRepository;
        }

        public async Task<GetNeighborhoodAnalysisResult?> Handle(GetNeighborhoodAnalysisQuery request, CancellationToken cancellationToken)
        {
            var primaryStat = await _marketStatRepository.GetByNeighborhoodIdAsync(request.NeighborhoodId);
            if (primaryStat == null)
                return null;

            var primaryResult = NeighborhoodAnalysisCalculator.Calculate(primaryStat, request.PropertySegment);

            NeighborhoodAnalysisItemResult? comparisonResult = null;
            if (request.CompareWithNeighborhoodId.HasValue)
            {
                var comparisonStat = await _marketStatRepository.GetByNeighborhoodIdAsync(request.CompareWithNeighborhoodId.Value);
                if (comparisonStat != null)
                {
                    comparisonResult = NeighborhoodAnalysisCalculator.Calculate(comparisonStat, request.PropertySegment);
                }
            }

            return new GetNeighborhoodAnalysisResult
            {
                Primary = primaryResult,
                Comparison = comparisonResult,
                Disclaimer = FibiNeighborhoodAnalysisConstants.Disclaimer
            };
        }
    }
}
