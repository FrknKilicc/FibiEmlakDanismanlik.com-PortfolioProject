using System.Collections.Generic;

namespace FibiEmlakDanismanlik.WebUI.Models.ListingSpecs
{
    /// <summary>
    /// İlan detay sayfası için ortak "İlan Özellikleri" görünüm modeli.
    /// </summary>
    public class ListingSpecsVm
    {
        public string UsageKey { get; set; } = "sale";        // sale | rent
        public string CategoryKey { get; set; } = "housing";  // housing | land | commercial
        public List<SpecFactVm> KeyFacts { get; set; } = new();
        public List<SpecGroupVm> Groups { get; set; } = new();
        public List<SpecAmenityGroupVm> AmenityGroups { get; set; } = new();
        public List<SpecInsightVm> Insights { get; set; } = new();
        public string ProtectedNote { get; set; } = "";
    }

    public class SpecFactVm
    {
        public string Icon { get; set; } = "";
        public string Label { get; set; } = "";
        public string Value { get; set; } = "";
    }

    public class SpecGroupVm
    {
        public string Title { get; set; } = "";
        public string Icon { get; set; } = "";
        public List<SpecRowVm> Rows { get; set; } = new();
    }

    public class SpecRowVm
    {
        public string Label { get; set; } = "";
        public string Value { get; set; } = "";
        public string Tone { get; set; } = ""; // "" | positive | negative | locked
    }

    public class SpecAmenityGroupVm
    {
        public string Title { get; set; } = "";
        public string Icon { get; set; } = "";
        public List<string> Items { get; set; } = new();
    }

    public class SpecInsightVm
    {
        public string Icon { get; set; } = "";
        public string Title { get; set; } = "";
        public string Text { get; set; } = "";
        public string? LinkText { get; set; }
        public string? LinkUrl { get; set; }
    }
}
