using FibiEmlakDanismanlik.Application.Features.Results.FibiServiceResults;
using FibiEmlakDanismanlik.Domain.Entities;

namespace FibiEmlakDanismanlik.Application.Models.FibiServiceModels
{
    /// <summary>Bir mahalle için çözümlenmiş emsal verisi, kaynağı ve deprem tehlikesi.</summary>
    public record FibiMarketContext(
        FibiLocationSummaryResult Location,
        ExternalMarketStat Stat,
        FibiDataSourceResult DataSource,
        SeismicHazardAssessment Seismic);
}
