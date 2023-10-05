using Nukleus.Application.Common.Services;
using Serilog;
using Serilog.Exceptions;

namespace Nukleus.Infrastructure.Common.Logging
{
    internal class SerilogLogger : INukleusLogger
    {

        private string? traceID = default;

        public SerilogLogger()
        {

            Serilog.Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.WithExceptionDetails()
                .WriteTo.Console()
                .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        public void SetTraceID(string tId){
            traceID = tId;
        }

        public void LogDebug(string message)
        {
            // Performance wrapper. Saves unneccesary call and heap allocation.
            if (Serilog.Log.Logger.IsEnabled(Serilog.Events.LogEventLevel.Debug))
            {
                Log(message, LogLevel.Information);
            }
        }

        public void LogInfo(string message)
        {
            // Performance wrapper. Saves unneccesary call and heap allocation.
            if (Serilog.Log.Logger.IsEnabled(Serilog.Events.LogEventLevel.Information))
            {
                Log(message, LogLevel.Information);
            }
        }

        public void LogKnownError(string message)
        {
            // Performance wrapper. Saves unneccesary call and heap allocation.
            if (Serilog.Log.Logger.IsEnabled(Serilog.Events.LogEventLevel.Error))
            {
                Log(message, LogLevel.Error);
            }
        }

        public void LogUnknownError(string message)
        {
            // Performance wrapper. Saves unneccesary call and heap allocation.
           if(Serilog.Log.Logger.IsEnabled(Serilog.Events.LogEventLevel.Error)){
            Log(message, LogLevel.UnexpectedError);
           }
        }

        public void LogWarning(string message)
        {
            // Performance wrapper. Saves unneccesary call and heap allocation.
            if (Serilog.Log.Logger.IsEnabled(Serilog.Events.LogEventLevel.Warning))
            {
                Log(message, LogLevel.Warning);
            }
        }

        /*
        Helper
        Logs the request, and optionally adds a public-facing status message to the API Response.
        Abstraction layer from our logging status to Serilog's status.
        */
        private void Log(string message, LogLevel severity)
        {
            switch(severity){
                case LogLevel.Debug:
                    // TODO Check if debug is set in configuration.
                    Serilog.Log.Debug(traceID + ": " + message);
                    break;
                case LogLevel.Information:
                    Serilog.Log.Information(traceID + ": " + message);
                    break;
                case LogLevel.Error or LogLevel.UnexpectedError:
                    Serilog.Log.Error(traceID + ": " + message);
                    break;
                case LogLevel.Warning:
                    Serilog.Log.Warning(traceID + ": " + message);
                    break;
                default:
                    Serilog.Log.Warning(traceID + ": " + message);
                    break;
            }
        }
    }
}