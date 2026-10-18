namespace FibiEmlakDanismanlik.Application.Features.Results.FibiServiceResults
{
    /// <summary>Gelecek çeyrek projeksiyon noktası sonucu (muhafazakar, dengeli, iyimser).</summary>
    public record FibiProjectionPointResult(string Quarter, decimal Conservative, decimal Balanced, decimal Optimistic);
}
