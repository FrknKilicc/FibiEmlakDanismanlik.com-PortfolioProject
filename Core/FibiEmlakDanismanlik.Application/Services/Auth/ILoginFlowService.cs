using FibiEmlakDanismanlik.Application.Features.Results.AuthResults;
using FibiEmlakDanismanlik.Domain.Entities;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Services.Auth;

/// <summary>
/// Giriş adımlarını, başarısızlık kayıtlarını ve hesap kilitleme kontrollerini yöneten ortak servis arayüzü.
/// </summary>
public interface ILoginFlowService
{
    /// <summary>
    /// Kimlik doğrulandıktan sonra akıştaki sıradaki adımı (şifre değişikliği, 2FA veya başarılı token) belirler ve tamamlar.
    /// </summary>
    Task<GetLoginResult> CompleteAsync(Agent agent);

    /// <summary>
    /// Başarısız giriş denemesini kaydeder; azami deneme sayısına ulaşılırsa hesabı kilitler.
    /// </summary>
    Task RegisterFailureAsync(Agent agent);

    /// <summary>
    /// Hesabın kilitli olup olmadığını denetler.
    /// </summary>
    bool IsLockedOut(Agent agent);

    /// <summary>
    /// Giriş denetim günlüğü kaydı (LoginAuditLog) oluşturur.
    /// </summary>
    Task LogAsync(string attemptedMail, int? agentId, bool succeeded, string? failureReason);
}
