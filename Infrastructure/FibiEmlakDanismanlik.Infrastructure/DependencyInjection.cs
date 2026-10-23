using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Interfaces.FibiServiceInterfaces;
using FibiEmlakDanismanlik.Infrastructure.MarketData;
using FibiEmlakDanismanlik.Infrastructure.MarketData.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Infrastructure.Security;
namespace FibiEmlakDanismanlik.Infrastructure
{
    /// <summary>Altyapı katmanı servis kayıtları.</summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddFibiInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var syncSection = configuration.GetSection(MarketDataSyncOptions.SectionName);
            services.Configure<MarketDataSyncOptions>(syncSection);

            var userAgentContact = syncSection[nameof(MarketDataSyncOptions.UserAgentContact)];
            var userAgent = !string.IsNullOrWhiteSpace(userAgentContact)
                ? $"{FibiMarketDataSyncConstants.UserAgent} (+{userAgentContact.Trim()})"
                : FibiMarketDataSyncConstants.UserAgent;

            services.AddHttpClient(FibiMarketDataSyncConstants.HttpClientName, client =>
            {
                client.Timeout = TimeSpan.FromSeconds(FibiMarketDataSyncConstants.HttpTimeoutSeconds);
                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", userAgent);
            });

            services.AddScoped<IMarketDataProvider, SeedMarketDataProvider>();
            services.AddScoped<IMarketDataProvider, IbbSeismicScenarioProvider>();
            services.AddScoped<IMarketDataProvider, EvdsUnitPriceProvider>();
            services.AddScoped<IMarketDataProvider, TcmbKfeBulletinProvider>();
            services.AddScoped<IMarketDataProvider, ListingGeocodeProvider>();

            services.AddHostedService<MarketDataSyncBackgroundService>();

            services.AddHttpClient("turnstile", c => c.Timeout = TimeSpan.FromSeconds(10));
            services.AddScoped<ICaptchaVerifier, TurnstileCaptchaVerifier>();

            return services;
        }
    }
}
