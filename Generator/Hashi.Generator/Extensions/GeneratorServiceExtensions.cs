using System.Drawing;
using Hashi.Generator.Interfaces;
using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Interfaces.Providers;
using Hashi.Generator.Models;
using Hashi.Generator.Providers;
using Hashi.Generator.Services;
using Hashi.Generator.Simulation;
using Microsoft.Extensions.DependencyInjection;

namespace Hashi.Generator.Extensions;

/// <summary>
///     Extension methods for registering generator services.
/// </summary>
public static class GeneratorServiceExtensions
{
    /// <summary>
    ///     Adds generator services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddGeneratorServices(this IServiceCollection services)
    {
        services.AddSingleton<IHashiGenerator, HashiGenerator>();
        services.AddSingleton<IDifficultySettingsProvider, DifficultySettingsProvider>();
        services.AddSingleton<BlockDetectionService>();
        services.AddSingleton<IBlockDetectionService>(sp => sp.GetRequiredService<BlockDetectionService>());
        services.AddSingleton<IIslandLayoutService, IslandLayoutService>();
        services.AddSingleton<IBridgeManagementService, BridgeManagementService>();
        services.AddSingleton<ISimulationFactory, SimulationFactory>();
        services.AddSingleton<IRuleSolvabilityValidator, RuleSolvabilityValidator>();
        services.AddTransient<IIsland, Island>();
        services.AddTransient<IBridge, Bridge>();
        services.AddTransient<IBridgeCoordinates, BridgeCoordinates>();
        services.AddTransient<ISolutionProvider, SolutionProvider>();

        services.AddSingleton<Func<int, int, int, IIsland>>(sp =>
            (amountBridgesConnectable, row, column) =>
                ActivatorUtilities.CreateInstance<Island>(sp, amountBridgesConnectable, row, column));

        services.AddSingleton<Func<Point, Point, int, IBridgeCoordinates>>(sp =>
            (location1, location2, amountBridges) =>
                ActivatorUtilities.CreateInstance<BridgeCoordinates>(sp, location1, location2, amountBridges));

        services.AddSingleton<Func<int[][], IReadOnlyList<IBridgeCoordinates>, ISolutionProvider>>(sp =>
            (hashiField, bridgeCoordinates) =>
                ActivatorUtilities.CreateInstance<SolutionProvider>(sp,
                    (IReadOnlyList<int[]>)hashiField, bridgeCoordinates));

        services.AddSingleton<Func<IIsland, IIsland, int, IBridge>>(sp =>
            (island1, island2, amountBridgesSet) =>
                ActivatorUtilities.CreateInstance<Bridge>(sp, island1, island2, amountBridgesSet));

        return services;
    }
}
