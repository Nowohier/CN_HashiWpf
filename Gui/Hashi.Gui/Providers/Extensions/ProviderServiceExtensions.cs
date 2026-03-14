using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Interfaces.Providers;
using Hashi.Generator.Providers;
using Hashi.Gui.Core.Helpers;
using Hashi.Gui.Core.Providers;
using Hashi.Gui.Interfaces.Helpers;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.Services;
using Hashi.Gui.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hashi.Gui.Providers.Extensions;

/// <summary>
///     Extension methods for registering provider services.
/// </summary>
public static class ProviderServiceExtensions
{
    /// <summary>
    ///     Adds provider services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddProviderServices(this IServiceCollection services)
    {
        services.AddSingleton<ISettingsProvider, SettingsProvider>();
        services.AddSingleton<ITimerProvider, TimerProvider>();
        services.AddSingleton<IIslandViewModelHelper, IslandViewModelHelper>();
        services.AddSingleton<IIslandProviderCore, IslandProviderCore>();
        services.AddSingleton<IIslandProvider, IslandProvider>();
        services.AddSingleton<IHintProvider, HintProvider>();
        services.AddSingleton<IRuleInfoProvider, RuleInfoProvider>();
        services.AddSingleton<ITestSolutionProvider, TestSolutionProvider>();
        services.AddSingleton<IPathProvider, PathProvider>();
        services.AddSingleton<IGameCompletionHandler, GameCompletionHandler>();
        services.AddSingleton<ITestFieldService, TestFieldService>();

        services.AddSingleton<Func<IReadOnlyList<int[]>?, IReadOnlyList<IBridgeCoordinates>?, string?, ISolutionProvider>>(_ =>
            (hashiField, bridgeCoordinates, name) =>
                new SolutionProvider(hashiField, bridgeCoordinates, name));

        return services;
    }
}
