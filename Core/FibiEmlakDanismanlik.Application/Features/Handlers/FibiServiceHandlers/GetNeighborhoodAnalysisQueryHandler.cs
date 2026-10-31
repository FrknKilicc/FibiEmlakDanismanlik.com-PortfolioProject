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
        private readonly IFibiMarketContextService _marketContextService;

        public GetNeighborhoodAnalysisQueryHandler(IFibiMarketContextService marketContextService)
        {
            _marketContextService = marketContextService;
        }

        public async Task<GetNeighborhoodAnalysisResult?> Handle(GetNeighborhoodAnalysisQuery request, CancellationToken cancellationToken)
        {
            var primaryContext = await _marketContextService.GetForNeighborhoodAsync(request.NeighborhoodId);
            if (primaryContext == null)
                return null;

            var primaryResult = NeighborhoodAnalysisCalculator.Calculate(primaryContext, request.PropertySegment);

            NeighborhoodAnalysisItemResult? comparisonResult = null;
            if (request.CompareWithNeighborhoodId.HasValue)
            {
                var comparisonContext = await _marketContextService.GetForNeighborhoodAsync(request.CompareWithNeighborhoodId.Value);
                if (comparisonContext != null)
                {
                    comparisonResult = NeighborhoodAnalysisCalculator.Calculate(comparisonContext, request.PropertySegment);
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
