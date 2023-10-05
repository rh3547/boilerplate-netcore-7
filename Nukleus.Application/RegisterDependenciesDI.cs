using System.Reflection;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Nukleus.Application;

    public static class RegisterDependenciesDI
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddApplicationMapsterMappings();

            return services;
        }

        private static IServiceCollection AddApplicationMapsterMappings(this IServiceCollection services)
        {
            // Mappings
            var config = TypeAdapterConfig.GlobalSettings;

            // Set the max-depth for Mapster to avoid infinite loops when mapping
            config.Default.MaxDepth(2);
            config.Default.IgnoreNullValues(true);
            
            config.Scan(Assembly.GetExecutingAssembly());
            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>(); // ServiceMapper is included with DI package.

            return services;
        }
    }
