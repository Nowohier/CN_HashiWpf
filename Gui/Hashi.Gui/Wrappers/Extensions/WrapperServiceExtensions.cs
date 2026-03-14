using Hashi.Gui.Interfaces.Wrappers;
using Microsoft.Extensions.DependencyInjection;

namespace Hashi.Gui.Wrappers.Extensions;

/// <summary>
///     Extension methods for registering wrapper services.
/// </summary>
public static class WrapperServiceExtensions
{
    /// <summary>
    ///     Adds wrapper services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddWrapperServices(this IServiceCollection services)
    {
        services.AddSingleton<IDialogWrapper, DialogWrapper>();
        services.AddSingleton<IJsonWrapper, JsonWrapper>();
        services.AddSingleton<IFileWrapper, FileWrapper>();
        services.AddSingleton<IDirectoryWrapper, DirectoryWrapper>();
        services.AddSingleton<IApplicationWrapper, ApplicationWrapper>();
        return services;
    }
}
