namespace FibiEmlakDanismanlik.Application.Features.Results.SiteTextResults
{
    /// <summary>
    /// Panelde düzenlenen tek site metni: tanım + güncel değer.
    /// </summary>
    public class GetAdminSiteTextResult
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
}
