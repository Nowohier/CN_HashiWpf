using Hashi.SolvedPuzzles.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Hashi.SolvedPuzzles.Extensions;

/// <summary>
///     Extension methods for registering solved puzzles services.
/// </summary>
public static class SolvedPuzzlesServiceExtensions
{
    /// <summary>
    ///     Adds solved puzzles services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSolvedPuzzlesServices(this IServiceCollection services)
    {
        services.AddSingleton<IHashiPuzzleLoader, HashiPuzzleLoader>();
        return services;
    }
}
