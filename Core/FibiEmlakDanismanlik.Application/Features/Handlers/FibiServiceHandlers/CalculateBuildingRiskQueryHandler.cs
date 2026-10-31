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
    public class CalculateBuildingRiskQueryHandler : IRequestHandler<CalculateBuildingRiskQuery, CalculateBuildingRiskResult?>
    {
        private readonly IMarketStatRepository _marketStatRepository;
        private readonly IFibiMarketContextService _marketContextService;

        public CalculateBuildingRiskQueryHandler(
            IMarketStatRepository marketStatRepository,
            IFibiMarketContextService marketContextService)
        {
            _marketStatRepository = marketStatRepository;
            _marketContextService = marketContextService;
        }

        public async Task<CalculateBuildingRiskResult?> Handle(CalculateBuildingRiskQuery request, CancellationToken cancellationToken)
        {
            var context = await _marketContextService.GetForNeighborhoodAsync(request.NeighborhoodId);
            if (context == null)
                return null;

            var cityAverageSalePrice = await _marketStatRepository.GetCityAverageSalePriceAsync(context.Location.CityId);

            var structuralScore = BuildingRiskScorer.CalculateStructuralScore(request, context.Seismic.HazardLevel);
            var (riskLabel, riskCode) = BuildingRiskScorer.GetRiskLevel(structuralScore);

            var transformationScore = BuildingRiskScorer.CalculateTransformationScore(
                request.ConstructionYearRange,
                context.Stat.UrbanTransformationAppetiteScore,
                context.Stat.BaseSalePricePerSqm,
                cityAverageSalePrice);

            var transformationVerdict = BuildingRiskScorer.GetTransformationVerdict(transformationScore);
            var insights = BuildingRiskScorer.BuildInsights(request, context.Seismic, transformationScore);

            return new CalculateBuildingRiskResult
            {
                Location = context.Location,
                DataSource = context.DataSource,
                Seismic = context.Seismic.Summary,
                StructuralRiskScore = structuralScore,
                RiskLevel = riskLabel,
                RiskLevelCode = riskCode,
                UrbanTransformationPotentialScore = transformationScore,
                UrbanTransformationVerdict = transformationVerdict,
                ActionableInsights = insights,
                LegalDisclaimer = FibiRiskAnalysisConstants.LegalDisclaimer
            };
        }
    }
}
