using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Interfaces;

/// <summary>
/// Bot koruması (Captcha) doğrulama servisi arayüzü.
/// </summary>
public interface ICaptchaVerifier
{
    Task<bool> VerifyAsync(string? token, string? remoteIp, CancellationToken cancellationToken);
}
