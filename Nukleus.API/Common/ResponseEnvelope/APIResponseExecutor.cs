using Nukleus.Application.Common.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;

// https://learn.microsoft.com/en-us/answers/questions/469027/proper-way-of-wrapping-the-response-along-with-exc
// Does not work with primitive payload types.
namespace Nukleus.API.Common.ResponseEnvelope
{
    internal class APIResponseExecutor : ObjectResultExecutor
    {
        public APIResponseExecutor( 
            OutputFormatterSelector formatterSelector, 
            IHttpResponseStreamWriterFactory writerFactory, 
            ILoggerFactory loggerFactory, 
            IOptions<MvcOptions> mvcOptions) : base(formatterSelector, writerFactory, loggerFactory, mvcOptions)
        {
            // Any stuff we need to instantiate for DI
        }

        public override Task ExecuteAsync(ActionContext context, ObjectResult result)
        {
            var response = new ResponseEnvelope<object>();

            // For non-generic Result objects (void return) to indicate succesful operation.
            if (result.Value == null)
            {
                response.Payload = "Operation Completed Successfully";
                response.ErrorMessage = null;
            }
            // For Errors
            else if (result.Value.GetType() == typeof(Error))
            {
                response.Payload = null;
                response.ErrorMessage = ((Error)result.Value).Description;
            }
            else if (result.Value.GetType() == typeof(Success<Object>)){
                response.Payload = ((Success<Object>)result.Value).Value;

                // Treat it as an object because we don't care what type, but can't use generics
                //response.SuccessMessage = ((Success<Object>)result.Value).Description;
            }
            else {
                response.Payload = result.Value;
                //response.SuccessMessage = null;
                response.ErrorMessage = null;
                // Switch on type and return a generic message (crud)
            }

            response.TraceID = context.HttpContext.TraceIdentifier;

            // Does not work with primitive payload types.
            TypeCode? typeCode = Type.GetTypeCode(result.Value?.GetType() ?? typeof(object));
            if (typeCode == TypeCode.Object) result.Value = response;

            return base.ExecuteAsync(context, result);
        }
    }
}