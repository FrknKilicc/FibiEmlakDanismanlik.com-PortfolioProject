using FibiEmlakDanismanlik.Domain.Enums;

namespace FibiEmlakDanismanlik.Application.Interfaces.AgentInterfaces;

/// <summary>
/// Danışmanın kimlik ve yetki durumunu doğrulamak için kullanılan hafif durum kaydı.
/// </summary>
public sealed record AgentAuthState(bool IsActive, string SecurityStamp, AgentRole Role);
