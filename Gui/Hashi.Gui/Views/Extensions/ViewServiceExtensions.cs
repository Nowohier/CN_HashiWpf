using Hashi.Gui.Interfaces.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Hashi.Gui.Views.Extensions;

/// <summary>
///     Extension methods for registering view services.
/// </summary>
public static class ViewServiceExtensions
{
    /// <summary>
    ///     Adds view services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddViewServices(this IServiceCollection services)
    {
        services.AddSingleton<IWindow, HashiMainView>();
        services.AddSingleton<IViewBoxControl>(sp => (IViewBoxControl)sp.GetRequiredService<IWindow>());
        return services;
    }
}
