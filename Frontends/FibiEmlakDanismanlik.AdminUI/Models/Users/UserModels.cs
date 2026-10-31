using System.Globalization;

namespace FibiEmlakDanismanlik.AdminUI.Models.Users;

public sealed class AgentAdminDto
{
    public int AgentId { get; set; }
    public string? AgentName { get; set; }
    public string? AgentTitle { get; set; }
    public string? Mail { get; set; }
    public string? AgentPhoneNumber { get; set; }
    public string? AgentImgUrl { get; set; }
    public string? Role { get; set; }
    public bool IsActive { get; set; }
    public bool ShowOnWebsite { get; set; }
    public bool CanViewAllListings { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public bool MustChangePassword { get; set; }
    public bool IsLockedOut { get; set; }
    public DateTime? LastLoginAtUtc { get; set; }
}

public sealed class AgentPublicDto
{
    public int AgentId { get; set; }
    public string? AgentDescription { get; set; }
}

public sealed class UserFormModel
{
    public int? AgentId { get; set; }
    public string? AgentName { get; set; }
    public string? AgentTitle { get; set; }
    public string? AgentDescription { get; set; }
    public string? AgentImgUrl { get; set; }
    public string? Mail { get; set; }
    public string? AgentPhoneNumber { get; set; }
    public string? Password { get; set; }
    public int Role { get; set; } = 3;
    public bool IsActive { get; set; } = true;
    public bool ShowOnWebsite { get; set; } = true;
    public bool CanViewAllListings { get; set; }
    public bool ResetTwoFactor { get; set; }
    public bool Unlock { get; set; }
}

public sealed class UserIndexViewModel
{
    public List<AgentAdminDto> Users { get; set; } = new();
    public string? Ara { get; set; }
    public string? Rol { get; set; }
    public bool PasifleriGoster { get; set; }
    public string? ErrorMessage { get; set; }
    public bool IsOwner { get; set; }
    public int? CurrentAgentId { get; set; }
}

public sealed class UserFormViewModel
{
    public UserFormModel Form { get; set; } = new();
    public AgentAdminDto? Existing { get; set; }
    public bool IsNew { get; set; }
    public bool IsOwner { get; set; }
    public bool IsSelf { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, string[]> FieldErrors { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public string? GetFieldError(string key)
    {
        if (FieldErrors.TryGetValue(key, out var errs) && errs != null && errs.Length > 0)
        {
            return errs[0];
        }
        return null;
    }
}

public sealed class AuditLogItemDto
{
    public long AuditLogId { get; set; }
    public int? AgentId { get; set; }
    public string? AgentName { get; set; }
    public string? Action { get; set; }
    public string? EntityName { get; set; }
    public string? EntityKey { get; set; }
    public string? ChangesJson { get; set; }
    public string? IpAddress { get; set; }
    public DateTime? CreatedAtUtc { get; set; }
}

public sealed class AuditLogResponse
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<AuditLogItemDto> Items { get; set; } = new();
}

public sealed class AuditLogViewModel
{
    public AuditLogResponse? Data { get; set; }
    public string? Tablo { get; set; }
    public int? Kullanici { get; set; }
    public int Sayfa { get; set; } = 1;
    public List<AgentAdminDto> Agents { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public int TotalPages { get; set; }
}

public static class UserLabels
{
    private static readonly TimeZoneInfo TurkeyTimeZone = ResolveTurkeyTimeZone();

    private static TimeZoneInfo ResolveTurkeyTimeZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Europe/Istanbul");
        }
        catch (TimeZoneNotFoundException)
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {
                return TimeZoneInfo.Utc;
            }
        }
        catch
        {
            return TimeZoneInfo.Utc;
        }
    }

    public static string RoleName(string? role) => role switch
    {
        "Owner" => "Sahip",
        "Manager" => "Yönetici",
        "Consultant" => "Danışman",
        _ => string.IsNullOrWhiteSpace(role) ? "—" : role
    };

    public static string RoleBadge(string? role) => role switch
    {
        "Owner" => "danger",
        "Manager" => "primary",
        "Consultant" => "secondary",
        _ => "secondary"
    };

    public static int RoleValue(string? role) => role switch
    {
        "Owner" => 1,
        "Manager" => 2,
        "Consultant" => 3,
        _ => 3
    };

    public static string ToTurkeyTime(DateTime? utc)
    {
        if (!utc.HasValue)
        {
            return "—";
        }

        var val = utc.Value.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(utc.Value, DateTimeKind.Utc)
            : utc.Value.ToUniversalTime();

        var local = TimeZoneInfo.ConvertTimeFromUtc(val, TurkeyTimeZone);
        return local.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
    }

    public static string ActionName(string? action) => action switch
    {
        "Created" or "Added" => "Eklendi",
        "Updated" or "Modified" => "Güncellendi",
        "Deleted" => "Silindi",
        _ => string.IsNullOrWhiteSpace(action) ? "—" : action
    };

    public static string ActionBadge(string? action) => action switch
    {
        "Created" or "Added" => "success",
        "Updated" => "primary",
        "Modified" => "info text-dark",
        "Deleted" => "danger",
        _ => "secondary"
    };
}
