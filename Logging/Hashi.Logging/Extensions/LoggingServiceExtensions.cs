using Hashi.Logging.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Hashi.Logging.Extensions;

/// <summary>
///     Extension methods for registering logging services.
/// </summary>
public static class LoggingServiceExtensions
{
    /// <summary>
    ///     Adds logging services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddLoggingServices(this IServiceCollection services)
    {
        services.AddSingleton<ILoggerFactory, LoggerFactory>();
        return services;
    }
}
