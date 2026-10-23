using System.Collections.Generic;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Features.Results.AuditLogResults;

namespace FibiEmlakDanismanlik.Application.Interfaces.AuditLogInterfaces;

/// <summary>
/// Denetim günlüğü kayıtlarına erişim ve sorgulama işlemlerini tanımlayan arayüz.
/// </summary>
public interface IAuditLogRepository
{
    /// <summary>
    /// Sayfalanmış ve filtrelenmiş denetim günlüğü kayıtlarını getirir.
    /// </summary>
    /// <param name="skip">Atlanacak kayıt sayısı.</param>
    /// <param name="take">Alınacak kayıt sayısı.</param>
    /// <param name="entityName">İsteğe bağlı varlık adı filtresi.</param>
    /// <param name="agentId">İsteğe bağlı personel kimliği filtresi.</param>
    /// <returns>Toplam kayıt sayısı ve geçerli sayfadaki kayıtlar demeti.</returns>
    Task<(int Total, List<AuditLogItemResult> Items)> GetPagedAsync(int skip, int take, string? entityName, int? agentId);
}
