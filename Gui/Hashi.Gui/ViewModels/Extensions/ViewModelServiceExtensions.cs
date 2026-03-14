using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.ViewModels.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Hashi.Gui.ViewModels.Extensions;

/// <summary>
///     Extension methods for registering view model services.
/// </summary>
public static class ViewModelServiceExtensions
{
    /// <summary>
    ///     Adds view model services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddViewModelServices(this IServiceCollection services)
    {
        services.AddTransient<ISettingsViewModel, SettingsViewModel>();
        services.AddTransient<ILanguageViewModel, LanguageViewModel>();
        services.AddTransient<IHighScorePerDifficultyViewModel, HighScorePerDifficultyViewModel>();
        services.AddSingleton<IMainViewModel, MainViewModel>();

        services.AddSingleton<Func<ISettingsViewModel>>(sp =>
            () => sp.GetRequiredService<ISettingsViewModel>());

        services.AddSingleton<Func<int, int, int, IIslandViewModel>>(sp =>
            (x, y, maxConnections) =>
                ActivatorUtilities.CreateInstance<IslandViewModel>(sp, x, y, maxConnections));

        return services;
    }
}
