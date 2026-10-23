using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Results.AuthResults;
using FibiEmlakDanismanlik.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FibiEmlakDanismanlik.Application.Helpers
{
    public static class JwtTokenGenerator
    {
        public const int MinKeyBytes = 32;

        // Jwt:Key User Secrets'ta (ya da ortam değişkeninde) tutulur, repoya girmez.
        public static byte[] GetSigningKey(IConfiguration configuration)
        {
            var key = configuration["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(key) || Encoding.UTF8.GetByteCount(key) < MinKeyBytes)
                throw new InvalidOperationException(
                    "Jwt:Key ayarlanmamış veya 32 bayttan kısa. dotnet user-secrets set \"Jwt:Key\" <değer> ile ayarlayın.");
            return Encoding.UTF8.GetBytes(key);
        }

        public static GetLoginResult GenerateToken(Agent agent, IConfiguration configuration)
        {
            var signingKey = new SymmetricSecurityKey(GetSigningKey(configuration));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var expireMinutes = int.TryParse(configuration["Jwt:ExpireMinutes"], out var minutes) ? minutes : 60;
            var expireDate = DateTime.UtcNow.AddMinutes(expireMinutes);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, agent.AgentId.ToString()),
                new Claim(ClaimTypes.Name, agent.AgentName ?? string.Empty),
                new Claim(ClaimTypes.Email, agent.Mail ?? string.Empty),
                new Claim(ClaimTypes.Role, agent.Role.ToString()),
                new Claim(AuthConstants.SecurityStampClaimType, agent.SecurityStamp ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expireDate,
                signingCredentials: credentials);

            return new GetLoginResult
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpireDate = expireDate
            };
        }
    }
}
