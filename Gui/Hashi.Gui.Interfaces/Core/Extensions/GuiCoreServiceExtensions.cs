using Hashi.Gui.Core.Helpers;
using Hashi.Gui.Core.Providers;
using Hashi.Gui.Interfaces.Helpers;
using Hashi.Gui.Interfaces.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Hashi.Gui.Core.Extensions;

/// <summary>
///     Extension methods for registering GUI core services.
/// </summary>
public static class GuiCoreServiceExtensions
{
    /// <summary>
    ///     Adds GUI core services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddGuiCoreServices(this IServiceCollection services)
    {
        services.TryAddSingleton<IIslandViewModelHelper, IslandViewModelHelper>();
        services.TryAddSingleton<IIslandProviderCore, IslandProviderCore>();
        return services;
    }
}
