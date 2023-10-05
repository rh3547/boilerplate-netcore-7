using Nukleus.API.Common.BaseController.ActionFilters;
using Nukleus.API.Common.ResponseEnvelope;
using Nukleus.API.Common.Validation;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Nukleus.API;

public static class RegisterDependenciesDI
{
    public static IServiceCollection AddPresentationServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        // Only works for object. Since we do some swapping for our response envelope,
        // may need to be changed/added if we decide to return primitives in the future
        // https://stackoverflow.com/questions/62452873/when-working-with-filters-should-you-set-the-contexts-result-always-to-content
        // https://github.com/dotnet/aspnetcore/blob/c565386a3ed135560bc2e9017aa54a950b4e35dd/src/Mvc/Mvc.Core/src/ContentResult.cs ?
        services.AddScoped<IActionResultExecutor<ObjectResult>, APIResponseExecutor>();

        // Action Filters (need to do it this way because of logger DI)
        // Normally don't have to do this and can just use the decorator, but we need the DI framework for logger injection.
        services.AddScoped<ValidationModelBindingActionFilter>();
        services.AddScoped<LoggingActionFilter>();

        // Adds all our validators. All validators must implement 'IAssemblyMarker'
        services.AddValidatorsFromAssemblyContaining<Program>();
        services.AddFluentValidationAutoValidation(config =>
        {
            //config.DisableDataAnnotationsValidation = true;
        });

        // services.Configure<FluentValidationAutoValidationConfiguration>(options =>
        // );

        services.Configure<ValidationSettings>(configuration.GetSection(ValidationSettings.SectionName));

        services.Configure<ApiBehaviorOptions>(options =>
        {
            // We want to use our own error handling.
            // Suppresses automatic 400 BadRequest provided via the [ApiController]
            // https://stackoverflow.com/questions/59922693/fluentvalidation-use-custom-iactionfilter
            options.SuppressModelStateInvalidFilter = true;
        });

        services.Configure<RouteHandlerOptions>(options =>
        {
            // We want to use our own error handling.
            //options.ThrowOnBadRequest = true;
        });

        return services;
    }
}
