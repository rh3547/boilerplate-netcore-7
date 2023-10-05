using System.Net;
using Nukleus.Application.Common.Services;
using Nukleus.Application.Common.Validation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Nukleus.API.Middleware
{
    // This global handler is never intended to be used.
    // Exceptions should be caught as Errors and logged in lower layers.
    // Exceptions should never be thrown.
    // This middleware exists as a 'catch-all'.
    public class ErrorHandlerMiddleware
    {
        private const string GENERIC_UNKNOWN_ERROR_MESSAGE = "An unhandled exception occurred and was caught by the middleware.";
        private readonly RequestDelegate _next;
        private readonly INukleusLogger _logger;

        private readonly IConfiguration _configuration;

        public ErrorHandlerMiddleware(RequestDelegate next, [FromServices] INukleusLogger logger, IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                if(_configuration.GetValue<bool>("ErrorHandlingSettings:LogUnsanitizedErrorMessages") == true)
                {
                    _logger.LogUnknownError(GENERIC_UNKNOWN_ERROR_MESSAGE + " - " + ex.Message);
                    _logger.LogUnknownError(ex.ToString());
                }
                else
                {
                    _logger.LogUnknownError(GENERIC_UNKNOWN_ERROR_MESSAGE + " Ommited from logging due to the appsettings configuration.");
                }

                var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();

                if (exceptionHandlerFeature != null)
                {
                    // Specific error is logged here
                    //error = new Error(ErrorType.Unknown, exceptionHandlerFeature.Error, exceptionHandlerFeature.Error.Message);
                }

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                // Only creating an error here (and not just using a message) because the APIResponseExecutor maps Error Object Results to the appropriate fields.

                ObjectResult errorToClient = null;

                if(_configuration.GetValue<bool>("ErrorHandlingSettings:ShowUnsanitizedErrorMessages") == true)
                {
                    errorToClient = new ObjectResult(GENERIC_UNKNOWN_ERROR_MESSAGE + " - " + ex.Message)
                        {
                            StatusCode = (int)HttpStatusCode.InternalServerError
                        };
                }
                else
                {
                    errorToClient = new ObjectResult(Error.UnknownError())
                        {
                            StatusCode = (int)HttpStatusCode.InternalServerError
                        };
                }

                await errorToClient.ExecuteResultAsync(new ActionContext
                {
                    HttpContext = context
                });
            }
        }
    }
}