using Hashi.LinearSolver.Interfaces;
using Hashi.LinearSolver.Interfaces.Models;
using Hashi.LinearSolver.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Hashi.LinearSolver.Extensions;

/// <summary>
///     Extension methods for registering linear solver services.
/// </summary>
public static class LinearSolverServiceExtensions
{
    /// <summary>
    ///     Adds linear solver services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddLinearSolverServices(this IServiceCollection services)
    {
        services.AddSingleton<HashiSolver>();
        services.AddSingleton<IHashiSolver>(sp => sp.GetRequiredService<HashiSolver>());
        services.AddSingleton<IPuzzleReader>(sp => sp.GetRequiredService<HashiSolver>());
        services.AddSingleton<IPuzzleSolver>(sp => sp.GetRequiredService<HashiSolver>());
        services.AddSingleton<IPuzzleVisualizer>(sp => sp.GetRequiredService<HashiSolver>());

        services.AddSingleton<Func<int, int, int, int, IIsland>>(sp =>
            (id, row, col, bridgesRequired) =>
                ActivatorUtilities.CreateInstance<Island>(sp, id, row, col, bridgesRequired));

        services.AddSingleton<Func<int, int, int, IEdge>>(sp =>
            (id, a, b) =>
                ActivatorUtilities.CreateInstance<Edge>(sp, id, a, b));

        return services;
    }
}
