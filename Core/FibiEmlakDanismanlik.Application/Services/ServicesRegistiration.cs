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
            services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(ServicesRegistiration).Assembly));
            services.AddAutoMapper(typeof(ServicesRegistiration).Assembly);
        }


    }
}
