using FibiEmlakDanismanlik.Persistence.Context;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FibiEmlakDanismanlik.WebApi.Services
{
    /// <summary>
    /// Deploy sonrası doğrulama için veritabanına bağlanılabildiğini kontrol eder.
    /// </summary>
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly FibiEmlakDanismanlikContext _context;

        public DatabaseHealthCheck(FibiEmlakDanismanlikContext context)
        {
            _context = context;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return await _context.Database.CanConnectAsync(cancellationToken)
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Unhealthy();
        }
    }
}
