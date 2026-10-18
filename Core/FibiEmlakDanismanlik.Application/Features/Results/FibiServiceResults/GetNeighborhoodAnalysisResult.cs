namespace FibiEmlakDanismanlik.Application.Features.Results.FibiServiceResults
{
    /// <summary>Bölge analizi sorgusu ana sonucu (birincil mahalle, opsiyonel karşılaştırma ve yasal uyarı).</summary>
    public class GetNeighborhoodAnalysisResult
    {
        public NeighborhoodAnalysisItemResult Primary { get; set; } = null!;
        public NeighborhoodAnalysisItemResult? Comparison { get; set; }
        public string Disclaimer { get; set; } = null!;
    }
}
