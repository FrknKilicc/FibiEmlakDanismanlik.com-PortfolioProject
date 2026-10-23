using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FibiEmlakDanismanlik.Persistence.Interceptors;

/// <summary>
/// Veritabanı değişikliklerini izleyip oturum açmış personellerin yaptığı ekleme, güncelleme ve silme
/// işlemlerini AuditLogs tablosuna otomatik olarak kaydeden Entity Framework Core interceptor sınıfı.
/// </summary>
public class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    private static readonly HashSet<string> IgnoredEntityTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        nameof(AuditLog),
        nameof(LoginAuditLog),
        nameof(AgentRecoveryCode)
    };

    private static readonly HashSet<string> SensitiveProperties = new(StringComparer.OrdinalIgnoreCase)
    {
        "Password",
        "PasswordHash",
        "TwoFactorSecretProtected",
        "SecurityStamp",
        "CodeHash"
    };

    private static readonly HashSet<string> AgentLoginLedgerProperties = new(StringComparer.OrdinalIgnoreCase)
    {
        "FailedLoginCount",
        "LockoutEndUtc",
        "LastLoginAtUtc"
    };

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    private readonly ICurrentUserService _currentUserService;
    private readonly List<AuditEntryRecord> _pendingEntries = new();
    private bool _isSavingAuditLogs;

    public AuditSaveChangesInterceptor(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (!_isSavingAuditLogs)
        {
            CollectPendingEntries(eventData.Context);
        }

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (!_isSavingAuditLogs)
        {
            CollectPendingEntries(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        if (_isSavingAuditLogs)
        {
            return base.SavedChanges(eventData, result);
        }

        try
        {
            var context = eventData.Context;
            if (context != null && _pendingEntries.Count > 0)
            {
                var auditLogs = CreateAuditLogs();
                if (auditLogs.Count > 0)
                {
                    _isSavingAuditLogs = true;
                    try
                    {
                        context.Set<AuditLog>().AddRange(auditLogs);
                        context.SaveChanges();
                    }
                    finally
                    {
                        _isSavingAuditLogs = false;
                    }
                }
            }
        }
        finally
        {
            _pendingEntries.Clear();
        }

        return base.SavedChanges(eventData, result);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (_isSavingAuditLogs)
        {
            return await base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        try
        {
            var context = eventData.Context;
            if (context != null && _pendingEntries.Count > 0)
            {
                var auditLogs = CreateAuditLogs();
                if (auditLogs.Count > 0)
                {
                    _isSavingAuditLogs = true;
                    try
                    {
                        context.Set<AuditLog>().AddRange(auditLogs);
                        await context.SaveChangesAsync(cancellationToken);
                    }
                    finally
                    {
                        _isSavingAuditLogs = false;
                    }
                }
            }
        }
        finally
        {
            _pendingEntries.Clear();
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public override void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        _pendingEntries.Clear();
        base.SaveChangesFailed(eventData);
    }

    public override Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
    {
        _pendingEntries.Clear();
        return base.SaveChangesFailedAsync(eventData, cancellationToken);
    }

    private void CollectPendingEntries(DbContext? context)
    {
        _pendingEntries.Clear();

        if (context == null || !_currentUserService.IsAuthenticated)
        {
            return;
        }

        var entries = context.ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        foreach (var entry in entries)
        {
            if (entry.Entity is AuditLog or LoginAuditLog or AgentRecoveryCode)
            {
                continue;
            }

            var entityType = entry.Metadata.ClrType;
            var entityName = entityType.Name;

            if (IgnoredEntityTypes.Contains(entityName))
            {
                continue;
            }

            var changes = new Dictionary<string, object?>();

            switch (entry.State)
            {
                case EntityState.Added:
                    foreach (var property in entry.Properties)
                    {
                        var propName = property.Metadata.Name;
                        changes[propName] = IsSensitive(propName) ? "***" : property.CurrentValue;
                    }
                    break;

                case EntityState.Modified:
                    foreach (var property in entry.Properties)
                    {
                        if (!property.IsModified)
                        {
                            continue;
                        }

                        var original = property.OriginalValue;
                        var current = property.CurrentValue;

                        if (ValuesAreEqual(original, current))
                        {
                            continue;
                        }

                        var propName = property.Metadata.Name;
                        if (IsSensitive(propName))
                        {
                            changes[propName] = new Dictionary<string, object?>
                            {
                                ["old"] = "***",
                                ["new"] = "***"
                            };
                        }
                        else
                        {
                            changes[propName] = new Dictionary<string, object?>
                            {
                                ["old"] = original,
                                ["new"] = current
                            };
                        }
                    }

                    if (changes.Count == 0)
                    {
                        continue;
                    }

                    // Agent güncellemesinde yalnızca giriş defter alanları değiştiyse kaydı atla
                    if (string.Equals(entityName, "Agent", StringComparison.OrdinalIgnoreCase) &&
                        changes.Keys.All(k => AgentLoginLedgerProperties.Contains(k)))
                    {
                        continue;
                    }
                    break;

                case EntityState.Deleted:
                    foreach (var property in entry.Properties)
                    {
                        var propName = property.Metadata.Name;
                        changes[propName] = IsSensitive(propName) ? "***" : property.OriginalValue;
                    }
                    break;
            }

            _pendingEntries.Add(new AuditEntryRecord
            {
                Entry = entry,
                State = entry.State,
                EntityName = entityName,
                Changes = changes
            });
        }
    }

    private List<AuditLog> CreateAuditLogs()
    {
        var auditLogs = new List<AuditLog>(_pendingEntries.Count);
        var nowUtc = DateTime.UtcNow;
        var agentId = _currentUserService.AgentId;
        var ipAddress = _currentUserService.IpAddress;

        foreach (var pending in _pendingEntries)
        {
            if (pending.State == EntityState.Added)
            {
                foreach (var pkProp in pending.Entry.Properties.Where(p => p.Metadata.IsPrimaryKey()))
                {
                    pending.Changes[pkProp.Metadata.Name] = pkProp.CurrentValue;
                }
            }

            var keyValues = pending.Entry.Properties
                .Where(p => p.Metadata.IsPrimaryKey())
                .Select(p => (pending.State == EntityState.Deleted ? (p.OriginalValue ?? p.CurrentValue) : (p.CurrentValue ?? p.OriginalValue))?.ToString())
                .Where(v => !string.IsNullOrEmpty(v));

            var entityKey = string.Join(",", keyValues);

            string action = pending.State switch
            {
                EntityState.Added => "Created",
                EntityState.Modified => "Updated",
                EntityState.Deleted => "Deleted",
                _ => pending.State.ToString()
            };

            var changesJson = JsonSerializer.Serialize(pending.Changes, JsonOptions);
            if (changesJson.Length > 16000)
            {
                changesJson = changesJson.Substring(0, 16000) + "…";
            }

            auditLogs.Add(new AuditLog
            {
                AgentId = agentId,
                Action = action,
                EntityName = pending.EntityName,
                EntityKey = string.IsNullOrEmpty(entityKey) ? null : entityKey,
                ChangesJson = changesJson,
                IpAddress = ipAddress,
                CreatedAtUtc = nowUtc
            });
        }

        return auditLogs;
    }

    private static bool IsSensitive(string propertyName)
    {
        return SensitiveProperties.Contains(propertyName) ||
               propertyName.Contains("Password", StringComparison.OrdinalIgnoreCase);
    }

    private static bool ValuesAreEqual(object? a, object? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        if (a is byte[] bytesA && b is byte[] bytesB)
            return bytesA.AsSpan().SequenceEqual(bytesB);
        return Equals(a, b);
    }

    private sealed class AuditEntryRecord
    {
        public required EntityEntry Entry { get; init; }
        public required EntityState State { get; init; }
        public required string EntityName { get; init; }
        public required Dictionary<string, object?> Changes { get; init; }
    }
}
