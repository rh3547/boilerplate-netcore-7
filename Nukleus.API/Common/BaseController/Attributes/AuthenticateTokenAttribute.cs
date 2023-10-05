using Microsoft.AspNetCore.Mvc.Filters;
using Nukleus.API.Common.BaseController.ActionResults;
using Nukleus.Application.Common.Services;
using Nukleus.Application.Common.Validation;

namespace Nukleus.API.Common.BaseController.Attributes
{

    public class AuthenticateTokenAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable => false;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<INukleusLogger>();
            var jwtTokenProvider = serviceProvider.GetRequiredService<IJwtTokenService>();
            var environment = serviceProvider.GetRequiredService<IWebHostEnvironment>();
            return new AuthenticateTokenFilter(logger, jwtTokenProvider, environment);
        }
    }

    public class AuthenticateTokenFilter : IAuthorizationFilter
    {
        private readonly INukleusLogger _logger;
        private readonly IJwtTokenService _jwtTokenProvider;
        private readonly IWebHostEnvironment _environment;

        private static FromErrorWithCode UNAUTHORIZED_RESULT = new FromErrorWithCode(Error.Unauthorized("Unauthorized. Invalid Token."), StatusCodes.Status401Unauthorized);


        public AuthenticateTokenFilter(INukleusLogger logger, IJwtTokenService jwtTokenProvider, IWebHostEnvironment environment)
        {
            _logger = logger;
            _jwtTokenProvider = jwtTokenProvider;
            _environment = environment;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            _logger.LogDebug("[AuthenticateToken] Authenticating token...");

            // Should we skip token validation?
            if(ShouldSkip(context))
            {
                return;
            }

            // Grab the token header.
            var tokenHeader = context.HttpContext.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(tokenHeader) || !tokenHeader.StartsWith("Bearer "))
            {
                // Unauthorized
                context.Result = UNAUTHORIZED_RESULT;
                _logger.LogDebug("[AuthenticateToken] Invalid access token header.");
                return;
            }

            var token = tokenHeader.Substring("Bearer ".Length);

            if (!_jwtTokenProvider.VerifyToken(token).Result)
            {
                context.Result = UNAUTHORIZED_RESULT;
                _logger.LogDebug("[AuthenticateToken] Access Token Verification Failed.");
                return;
            }

            _logger.LogInfo("[AuthenticateToken] Token Authenticated");
            
        }

        private bool ShouldSkip(AuthorizationFilterContext context)
        {
            // If we're in the development environment, skip token authorization
            // if (_environment.IsDevelopment() || _environment.EnvironmentName == "Development")
            // {
            //     return true;
            // }

            // This will be auto-applied to all endpoints, but we want to have the ability to exclude it with [AllowAnonymous]
            // Check if AllowAnonymous is applied to the action or controller
            var hasBaseAllowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute>().Any();
            var hasNukleusAllowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<Nukleus.API.Common.BaseController.Attributes.AllowAnonymousAttribute>().Any();

            if (hasBaseAllowAnonymous || hasNukleusAllowAnonymous)
            {
                return true;
            }

            return false;
        }
    }
}