using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Interfaces.FibiServiceInterfaces;
using FibiEmlakDanismanlik.Infrastructure.MarketData;
using FibiEmlakDanismanlik.Infrastructure.MarketData.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FibiEmlakDanismanlik.Infrastructure
{
    /// <summary>Altyapı katmanı servis kayıtları.</summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddFibiInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MarketDataSyncOptions>(configuration.GetSection(MarketDataSyncOptions.SectionName));

            services.AddHttpClient(FibiMarketDataSyncConstants.HttpClientName, client =>
            {
                client.Timeout = TimeSpan.FromSeconds(FibiMarketDataSyncConstants.HttpTimeoutSeconds);
                client.DefaultRequestHeaders.UserAgent.ParseAdd(FibiMarketDataSyncConstants.UserAgent);
            });

            services.AddScoped<IMarketDataProvider, SeedMarketDataProvider>();
            services.AddScoped<IMarketDataProvider, IbbSeismicScenarioProvider>();
            services.AddScoped<IMarketDataProvider, EvdsUnitPriceProvider>();
            services.AddScoped<IMarketDataProvider, TcmbKfeBulletinProvider>();

            services.AddHostedService<MarketDataSyncBackgroundService>();

            return services;
        }
    }
}
