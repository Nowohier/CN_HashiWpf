using Hashi.Gui.Interfaces.Helpers;
using Hashi.Gui.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hashi.Gui.Helpers.Extensions;

/// <summary>
///     Extension methods for registering helper services.
/// </summary>
public static class HelperServiceExtensions
{
    /// <summary>
    ///     Adds helper services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddHelperServices(this IServiceCollection services)
    {
        services.AddSingleton<IHashiBrushResolver, HashiBrushResolver>();
        services.AddSingleton<IDragDropService, DragDropService>();
        return services;
    }
}
