using Hashi.Gui.Interfaces.Managers;
using Microsoft.Extensions.DependencyInjection;

namespace Hashi.Gui.Managers.Extensions;

/// <summary>
///     Extension methods for registering manager services.
/// </summary>
public static class ManagerServiceExtensions
{
    /// <summary>
    ///     Adds manager services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddManagerServices(this IServiceCollection services)
    {
        services.AddSingleton<IResourceManager, ResourceManager>();
        return services;
    }
}
