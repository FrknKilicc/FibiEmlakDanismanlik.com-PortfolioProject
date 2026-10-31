using FibiEmlakDanismanlik.Application.Features.Results.BusinessSettingResults;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Queries.BusinessSettingQueries
{
    /// <summary>
    /// İş kuralı ayarlarını getirme sorgusu.
    /// </summary>
    public class GetBusinessSettingsQuery : IRequest<GetBusinessSettingsResult>
    {
    }
}
