using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FibiEmlakDanismanlik.Application.Helpers
{
    /// <summary>
    /// Çok adımlı kimlik doğrulama süreçleri (şifre değiştirme, 2FA vb.) için kısa ömürlü challenge token yardımcısı.
    /// </summary>
    public static class ChallengeTokenHelper
    {
        /// <summary>
        /// Belirtilen amaç (purpose) için kısa ömürlü bir challenge token üretir.
        /// API ana uç noktaları yalnızca Jwt:Audience kabul ettiğinden bu token normal API erişimi için kullanılamaz.
        /// </summary>
        public static string Create(Agent agent, string purpose, IConfiguration configuration)
        {
            var signingKey = new SymmetricSecurityKey(JwtTokenGenerator.GetSigningKey(configuration));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var expireDate = DateTime.UtcNow.AddMinutes(AuthConstants.ChallengeTokenMinutes);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, agent.AgentId.ToString()),
                new Claim(AuthConstants.ChallengePurposeClaimType, purpose),
                new Claim(AuthConstants.SecurityStampClaimType, agent.SecurityStamp ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: AuthConstants.ChallengeAudience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expireDate,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Challenge token'ı doğrular ve geçerliyse danışman kimliğini (AgentId) ile güvenlik damgasını döndürür.
        /// </summary>
        public static int? Validate(string? token, string expectedPurpose, IConfiguration configuration, out string? securityStamp)
        {
            securityStamp = null;

            if (string.IsNullOrWhiteSpace(token))
                return null;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = JwtTokenGenerator.GetSigningKey(configuration);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = AuthConstants.ChallengeAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                if (validatedToken is not JwtSecurityToken jwtToken ||
                    !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                var purpose = principal.FindFirst(AuthConstants.ChallengePurposeClaimType)?.Value
                              ?? principal.FindFirst("purpose")?.Value;

                if (!string.Equals(purpose, expectedPurpose, StringComparison.Ordinal))
                    return null;

                securityStamp = principal.FindFirst(AuthConstants.SecurityStampClaimType)?.Value
                                ?? principal.FindFirst("sstamp")?.Value;

                var nameId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                             ?? principal.FindFirst("sub")?.Value;

                if (int.TryParse(nameId, out var agentId))
                {
                    return agentId;
                }

                return null;
            }
            catch
            {
                securityStamp = null;
                return null;
            }
        }
    }
}
