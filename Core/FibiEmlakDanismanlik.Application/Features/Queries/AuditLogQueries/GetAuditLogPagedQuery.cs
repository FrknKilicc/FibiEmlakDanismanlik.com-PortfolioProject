using FibiEmlakDanismanlik.Application.Features.Results.AuditLogResults;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Queries.AuditLogQueries;

/// <summary>
/// Sayfalanmış denetim günlüğü kayıtlarını getirme sorgusu.
/// </summary>
public class GetAuditLogPagedQuery : IRequest<GetAuditLogPagedResult>
{
    private int _page = 1;
    private int _pageSize = 50;

    /// <summary>
    /// Sayfa numarası (en az 1).
    /// </summary>
    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    /// <summary>
    /// Sayfa başına kayıt sayısı (1-100 arası, varsayılan 50).
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value switch
        {
            < 1 => 50,
            > 100 => 100,
            _ => value
        };
    }

    /// <summary>
    /// Filtrelenecek varlık adı (isteğe bağlı).
    /// </summary>
    public string? EntityName { get; set; }

    /// <summary>
    /// Filtrelenecek personel kimliği (isteğe bağlı).
    /// </summary>
    public int? AgentId { get; set; }
}
