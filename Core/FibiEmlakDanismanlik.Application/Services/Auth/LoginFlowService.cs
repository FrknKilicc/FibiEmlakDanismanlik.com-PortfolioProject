using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Results.AuthResults;
using FibiEmlakDanismanlik.Application.Helpers;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Services.Auth;

/// <summary>
/// Giriş adımlarını, başarısızlık kayıtlarını ve hesap kilitleme kontrollerini yöneten ortak servis uygulaması.
/// </summary>
public class LoginFlowService : ILoginFlowService
{
    private readonly IRepository<Agent> _agentRepository;
    private readonly IRepository<LoginAuditLog> _loginAuditLogRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IConfiguration _configuration;

    public LoginFlowService(
        IRepository<Agent> agentRepository,
        IRepository<LoginAuditLog> loginAuditLogRepository,
        ICurrentUserService currentUserService,
        IConfiguration configuration)
    {
        _agentRepository = agentRepository;
        _loginAuditLogRepository = loginAuditLogRepository;
        _currentUserService = currentUserService;
        _configuration = configuration;
    }

    /// <summary>
    /// Kimlik doğrulandıktan sonra sonraki adımı belirler ve akışı tamamlar.
    /// </summary>
    public async Task<GetLoginResult> CompleteAsync(Agent agent)
    {
        // 1. Şifre değiştirme zorunluluğu varsa challenge token üretilir
        if (agent.MustChangePassword)
        {
            var challenge = ChallengeTokenHelper.Create(agent, AuthConstants.PurposePasswordChange, _configuration);
            return new GetLoginResult
            {
                Status = AuthConstants.LoginStatusPasswordChangeRequired,
                ChallengeToken = challenge,
                Token = null,
                ExpireDate = null,
                AgentName = agent.AgentName,
                Role = agent.Role.ToString()
            };
        }

        // 2. İki adımlı doğrulama (2FA) etkinleştirilmişse challenge token üretilir
        if (agent.TwoFactorEnabled)
        {
            var challenge = ChallengeTokenHelper.Create(agent, AuthConstants.PurposeTwoFactor, _configuration);
            return new GetLoginResult
            {
                Status = AuthConstants.LoginStatusTwoFactorRequired,
                ChallengeToken = challenge,
                Token = null,
                ExpireDate = null,
                AgentName = agent.AgentName,
                Role = agent.Role.ToString()
            };
        }

        // 3. Sahip veya Yönetici rollerinde 2FA kurulumu zorunludur
        if (agent.Role == AgentRole.Owner || agent.Role == AgentRole.Manager)
        {
            var challenge = ChallengeTokenHelper.Create(agent, AuthConstants.PurposeTwoFactorSetup, _configuration);
            return new GetLoginResult
            {
                Status = AuthConstants.LoginStatusTwoFactorSetupRequired,
                ChallengeToken = challenge,
                Token = null,
                ExpireDate = null,
                AgentName = agent.AgentName,
                Role = agent.Role.ToString()
            };
        }

        // 4. Tüm aşamalar tamamlandı: durum güncellenir, başarılı denetim kaydı yazılır ve JWT üretilir
        agent.LastLoginAtUtc = DateTime.UtcNow;
        agent.FailedLoginCount = 0;
        agent.LockoutEndUtc = null;
        await _agentRepository.UpdateAsync(agent);

        await LogAsync(agent.Mail ?? string.Empty, agent.AgentId, succeeded: true, failureReason: null);

        return JwtTokenGenerator.GenerateToken(agent, _configuration);
    }

    /// <summary>
    /// Başarısız giriş denemesini sayar, limit aşıldığında hesabı kilitler.
    /// </summary>
    public async Task RegisterFailureAsync(Agent agent)
    {
        agent.FailedLoginCount++;
        if (agent.FailedLoginCount >= AuthConstants.MaxFailedLoginAttempts)
        {
            agent.LockoutEndUtc = DateTime.UtcNow.AddMinutes(AuthConstants.LockoutMinutes);
            agent.FailedLoginCount = 0;
        }

        await _agentRepository.UpdateAsync(agent);
    }

    /// <summary>
    /// Hesabın geçici olarak kilitlenip kilitlenmediğini kontrol eder.
    /// </summary>
    public bool IsLockedOut(Agent agent)
    {
        return agent.LockoutEndUtc.HasValue && agent.LockoutEndUtc.Value > DateTime.UtcNow;
    }

    /// <summary>
    /// Giriş denemesi denetim günlüğü kaydı oluşturur.
    /// </summary>
    public async Task LogAsync(string attemptedMail, int? agentId, bool succeeded, string? failureReason)
    {
        var mail = attemptedMail;
        if (!string.IsNullOrEmpty(mail) && mail.Length > 256)
        {
            mail = mail.Substring(0, 256);
        }

        var log = new LoginAuditLog
        {
            AttemptedMail = mail ?? string.Empty,
            AgentId = agentId,
            IpAddress = _currentUserService.IpAddress,
            UserAgent = _currentUserService.UserAgent,
            Succeeded = succeeded,
            FailureReason = failureReason,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _loginAuditLogRepository.CreateAsync(log);
    }
}
