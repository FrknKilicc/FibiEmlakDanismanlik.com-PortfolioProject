namespace FibiEmlakDanismanlik.AdminUI.Models.Content;

/// <summary>
/// API'den gelen tek site metni (tanım + güncel değer).
/// </summary>
public sealed class SiteTextDto
{
    public string Key { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Default { get; set; } = string.Empty;
    public string Kind { get; set; } = string.Empty;
    public int MaxLength { get; set; }
    public int Min { get; set; }
    public int Max { get; set; }
    public string? Help { get; set; }
}

/// <summary>
/// Site metinleri düzenleme sayfası modeli.
/// </summary>
public sealed class SiteTextsViewModel
{
    public List<SiteTextDto> Items { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public Dictionary<string, string[]> FieldErrors { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
