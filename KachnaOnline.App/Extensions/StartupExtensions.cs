using Microsoft.Extensions.DependencyInjection;
using Serilog.AspNetCore;
using Serilog.Events;

namespace KachnaOnline.App.Extensions
{
    public static class StartupExtensions
    {
        public static void ConfigureSerilogRequestLogging(this IServiceCollection services)
        {
            services.Configure<RequestLoggingOptions>(options =>
            {
                options.EnrichDiagnosticContext = (diag, httpContext) =>
                {
                    if (httpContext == null)
                        return;

                    // Log HTTP Referer if there's one
                    if (httpContext.Request.Headers.TryGetValue("Referer", out var refererValues))
                    {
                        var host = httpContext.Request.Host.Host;
                        var referer = refererValues.Count > 0 ? refererValues[0] : null;
                        if (host != null && referer != null && !referer.Contains(host))
                        {
                            diag.Set("RequestReferer", referer);
                        }
                    }

                    // Log remote IP
                    diag.Set("RemoteIp", httpContext.Connection.RemoteIpAddress);
                };

                // Configures the event level based on the status code (only if there's no exception)
                options.GetLevel = (context, elapsed, exception) => exception == null
                    ? context.Response.StatusCode switch
                    {
                        // If the request takes more than 5 seconds to handle, there's something wrong
                        // and it is worth logging
                        <400 => (elapsed > 5000 ? LogEventLevel.Warning : LogEventLevel.Debug),
                        404 => LogEventLevel.Debug,
                        <500 => LogEventLevel.Information,
                        _ => LogEventLevel.Warning
                    }
                    : LogEventLevel.Error;
            });
        }
    }
}
