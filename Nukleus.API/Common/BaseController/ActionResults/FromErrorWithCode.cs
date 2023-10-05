using System.Net;
using Microsoft.AspNetCore.Mvc;
using Nukleus.Application.Common.Validation;

namespace Nukleus.API.Common.BaseController.ActionResults
{
    public class FromErrorWithCode : IActionResult
    {
        private readonly Error _result;

        private readonly int _desiredHttpCode;

        public FromErrorWithCode(Error result, int HttpCode)
        {
            _result = result;
            _desiredHttpCode = HttpCode;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            if(!Enum.IsDefined(typeof(HttpStatusCode), _desiredHttpCode))
            {
                // We don't want this to be handled by our Error Handler. 
                // This is not a runtime concern and should be caught during development.
                throw new Exception("Attempted returning a non-http status code in an object result.");
            }

            var objectResult = new ObjectResult(_result)
            {
                StatusCode = _desiredHttpCode
            };

            await objectResult.ExecuteResultAsync(context);
        }
    }
}