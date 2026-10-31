using System;
using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Features.Results.AuditLogResults;

/// <summary>
/// Sayfalanmış denetim günlüğü kayıtları sorgu sonucu.
/// </summary>
public class GetAuditLogPagedResult
{
    /// <summary>
    /// Toplam kayıt sayısı.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Geçerli sayfa numarası.
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Sayfa başına kayıt sayısı.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Denetim günlüğü kayıtları listesi.
    /// </summary>
    public List<AuditLogItemResult> Items { get; set; } = new();
}

/// <summary>
/// Tekil denetim günlüğü öğesi.
/// </summary>
public class AuditLogItemResult
{
    /// <summary>
    /// Denetim kaydı benzersiz kimliği.
    /// </summary>
    public int AuditLogId { get; set; }

    /// <summary>
    /// İşlemi gerçekleştiren personelin kimliği.
    /// </summary>
    public int? AgentId { get; set; }

    /// <summary>
    /// İşlemi gerçekleştiren personelin adı soyadı.
    /// </summary>
    public string? AgentName { get; set; }

    /// <summary>
    /// Gerçekleştirilen eylem türü (Created, Updated, Deleted).
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// İşlem gören varlığın adı.
    /// </summary>
    public string EntityName { get; set; } = string.Empty;

    /// <summary>
    /// Varlığın birincil anahtar değeri.
    /// </summary>
    public string? EntityKey { get; set; }

    /// <summary>
    /// Değişiklik verilerini içeren JSON metni.
    /// </summary>
    public string? ChangesJson { get; set; }

    /// <summary>
    /// İstekte bulunan istemcinin IP adresi.
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// Kaydın oluşturulduğu UTC zamanı.
    /// </summary>
    public DateTime CreatedAtUtc { get; set; }
}
