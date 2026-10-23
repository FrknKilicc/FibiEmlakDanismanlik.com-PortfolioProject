using FibiEmlakDanismanlik.Application.Behaviors;
using FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces;
using FibiEmlakDanismanlik.Application.Services.AdminListings;
using FibiEmlakDanismanlik.Application.Interfaces.FibiServiceInterfaces;
using FibiEmlakDanismanlik.Application.Services.Auth;
using FibiEmlakDanismanlik.Application.Services.FibiServices;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Services
{
    public static class ServicesRegistiration
    {
        public static void AddApplicationService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(x =>
            {
                x.RegisterServicesFromAssembly(typeof(ServicesRegistiration).Assembly);
                x.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });
            services.AddValidatorsFromAssembly(typeof(ServicesRegistiration).Assembly);
            services.AddAutoMapper(typeof(ServicesRegistiration).Assembly);
            services.AddScoped<IFibiMarketContextService, FibiMarketContextService>();
            services.AddScoped<IMarketDataSyncRunner, MarketDataSyncRunner>();
            services.AddScoped<ILoginFlowService, LoginFlowService>();
            services.AddScoped<IListingAccessService, ListingAccessService>();
        }


    }
}
