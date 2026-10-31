using System;
using System.Collections.Generic;

namespace FibiEmlakDanismanlik.AdminUI.Models.Settings;

/// <summary>
/// İş kuralı ayarları API DTO modeli.
/// </summary>
public sealed class BusinessSettingsDto
{
    public decimal SaleBuyerCommissionRate { get; set; }
    public decimal SaleSellerCommissionRate { get; set; }
    public decimal RentCommissionMonths { get; set; }
    public int BestDealMaxPerUsageType { get; set; }
    public int ListingAlertMaxActivePerPhone { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public int? UpdatedByAgentId { get; set; }
}

/// <summary>
/// İş kuralı ayarları form modeli.
/// </summary>
public sealed class BusinessSettingsForm
{
    public decimal SaleBuyerCommissionRate { get; set; }
    public decimal SaleSellerCommissionRate { get; set; }
    public decimal RentCommissionMonths { get; set; }
    public int BestDealMaxPerUsageType { get; set; }
    public int ListingAlertMaxActivePerPhone { get; set; }
}

/// <summary>
/// İş kuralı ayarları sayfası görünüm modeli.
/// </summary>
public sealed class SettingsViewModel
{
    public BusinessSettingsForm Form { get; set; } = new();
    public DateTime? UpdatedAtUtc { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, string[]> FieldErrors { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public string? GetFieldError(string key)
    {
        if (FieldErrors.TryGetValue(key, out var errors) && errors.Length > 0)
        {
            return errors[0];
        }
        return null;
    }
}
