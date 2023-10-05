using Nukleus.Application.Common.Services;
using Nukleus.Infrastructure.Common.Authentication;
using Nukleus.Infrastructure.Common.Logging;
using Nukleus.Infrastructure.Common.Persistence;
using Nukleus.Infrastructure.Common.Services;
using Nukleus.Infrastructure.UserModule;
using Nukleus.Infrastructure.AccountModule;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nukleus.Application.AccountModule;
using Nukleus.Application.Authentication;
using Nukleus.Infrastructure.Authentication;
using Nukleus.Application.UserModule;

namespace Nukleus.Infrastructure
{
    public static class RegisterDependenciesDI
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
            
            // Services - Domain-Agnostic Functionality (Boilerplate)
            services.AddScoped<IJwtTokenService,JwtTokenService>();
            services.AddSingleton<IHashingService, HashingService>();
            services.AddSingleton<IDateTimeProvider,DateTimeProvider>();
            services.AddSingleton<IEmailService, AmazonSESEmailService>();
            services.AddSingleton<INukleusLogger,SerilogLogger>();
            services.AddScoped<ISession, Session>();

            // Services - Domain-Specific Functionality
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAccountService, AccountService>();

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();

            // Add DB Context
            services.AddDbContext<NukleusDbContext>();
            
            return services;
        }
    }
}