using Nukleus.Application.Common.Services;

namespace Nukleus.API.Common.ResponseEnvelope
{
    public class TraceIDMiddleware
    {
        private readonly RequestDelegate _next;

        public TraceIDMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, INukleusLogger logger)
        {
            context.TraceIdentifier = Guid.NewGuid().ToString();
            string id = context.TraceIdentifier;
            context.Response.Headers["X-Trace-Id"] = id;

            logger.SetTraceID(id);
            logger.LogDebug($"Created GUID traceID: {id}");

            await _next(context);
        }
    }
}