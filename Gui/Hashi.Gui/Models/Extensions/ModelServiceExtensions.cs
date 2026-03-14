using Hashi.Enums;
using Hashi.Gui.Interfaces.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Hashi.Gui.Models.Extensions;

/// <summary>
///     Extension methods for registering model services.
/// </summary>
public static class ModelServiceExtensions
{
    /// <summary>
    ///     Adds model services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddModelServices(this IServiceCollection services)
    {
        services.AddTransient<IHashiBrush, HashiBrush>();
        services.AddTransient<IHashiPoint, HashiPoint>();
        services.AddTransient<IHashiBridge, HashiBridge>();

        services.AddSingleton<Func<int, int, HashiPointTypeEnum, IHashiPoint>>(sp =>
            (x, y, pointType) =>
                ActivatorUtilities.CreateInstance<HashiPoint>(sp, x, y, pointType));

        services.AddSingleton<Func<BridgeOperationTypeEnum, IHashiPoint, IHashiPoint, IHashiBridge>>(sp =>
            (actionType, point1, point2) =>
                ActivatorUtilities.CreateInstance<HashiBridge>(sp, actionType, point1, point2));

        return services;
    }
}
