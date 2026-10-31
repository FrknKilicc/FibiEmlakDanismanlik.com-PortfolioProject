using FibiEmlakDanismanlik.Application.Features.Results.FibiServiceResults;
using FibiEmlakDanismanlik.Domain.Enums;

namespace FibiEmlakDanismanlik.Application.Models.FibiServiceModels
{
    /// <summary>Puanlamada kullanılacak deprem tehlikesi seviyesi (null = bilinmiyor) ve kullanıcıya gösterilecek özeti.</summary>
    public record SeismicHazardAssessment(SoilRiskClass? HazardLevel, FibiSeismicSummaryResult Summary);
}
