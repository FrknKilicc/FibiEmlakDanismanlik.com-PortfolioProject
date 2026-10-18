using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Queries.FibiServiceQueries;
using FibiEmlakDanismanlik.Application.Features.Results.FibiServiceResults;
using FibiEmlakDanismanlik.Application.Helpers;
using FibiEmlakDanismanlik.Application.Interfaces.FibiServiceInterfaces;
using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.FibiServiceHandlers
{
    public class CalculateBuildingRiskQueryHandler : IRequestHandler<CalculateBuildingRiskQuery, CalculateBuildingRiskResult?>
    {
        private readonly IMarketStatRepository _marketStatRepository;

        public CalculateBuildingRiskQueryHandler(IMarketStatRepository marketStatRepository)
        {
            _marketStatRepository = marketStatRepository;
        }

        public async Task<CalculateBuildingRiskResult?> Handle(CalculateBuildingRiskQuery request, CancellationToken cancellationToken)
        {
            var stat = await _marketStatRepository.GetByNeighborhoodIdAsync(request.NeighborhoodId);
            if (stat == null)
                return null;

            var cityAverageSalePrice = await _marketStatRepository.GetCityAverageSalePriceAsync(stat.Neighborhood!.District.CityId);

            var structuralScore = BuildingRiskScorer.CalculateStructuralScore(request, stat.SoilRiskClass ?? SoilRiskClass.Medium);
            var (riskLabel, riskCode) = BuildingRiskScorer.GetRiskLevel(structuralScore);

            var transformationScore = BuildingRiskScorer.CalculateTransformationScore(
                request.ConstructionYearRange,
                stat.UrbanTransformationAppetiteScore,
                stat.BaseSalePricePerSqm,
                cityAverageSalePrice);

            var transformationVerdict = BuildingRiskScorer.GetTransformationVerdict(transformationScore);
            var insights = BuildingRiskScorer.BuildInsights(request, stat.SoilRiskClass ?? SoilRiskClass.Medium, transformationScore);

            return new CalculateBuildingRiskResult
            {
                Location = FibiLocationSummaryFactory.FromNeighborhood(stat.Neighborhood!),
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
