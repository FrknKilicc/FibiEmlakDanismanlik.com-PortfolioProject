using System;

namespace FibiEmlakDanismanlik.Application.Features.Results.MeResults
{
    /// <summary>
    /// Giriş denemesi geçmişi öğesini temsil eden sonuç modeli.
    /// </summary>
    public class GetMyLoginResult
    {
        public DateTime CreatedAtUtc { get; set; }
        public bool Succeeded { get; set; }
        public string? FailureReason { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }
}
