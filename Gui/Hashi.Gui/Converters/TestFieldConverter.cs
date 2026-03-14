using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Interfaces.Providers;
using Hashi.Generator.Models;
using Hashi.Gui.Interfaces.ViewModels;
using System.Drawing;

namespace Hashi.Gui.Converters;

/// <summary>
///     Converts island view models to a solution provider for test field persistence.
/// </summary>
public static class TestFieldConverter
{
    /// <summary>
    ///     Converts a collection of island view models to a solution provider.
    /// </summary>
    /// <param name="allIslandEnumerable">The island view models to convert.</param>
    /// <param name="solutionName">The name for the solution.</param>
    /// <param name="solutionProviderFactory">The factory for creating solution providers.</param>
    /// <returns>A new solution provider containing the converted data.</returns>
    public static ISolutionProvider ConvertIslandsToSolutionProvider(
        IEnumerable<IIslandViewModel> allIslandEnumerable, string solutionName,
        Func<IReadOnlyList<int[]>?, IReadOnlyList<IBridgeCoordinates>?, string?, ISolutionProvider> solutionProviderFactory)
    {
        ArgumentNullException.ThrowIfNull(allIslandEnumerable);
        ArgumentNullException.ThrowIfNull(solutionName);
        ArgumentNullException.ThrowIfNull(solutionProviderFactory);

        var allIslands = allIslandEnumerable.ToList();

        // Determine the size of the Hashi field
        var maxX = allIslands.Max(island => island.Coordinates.X);
        var maxY = allIslands.Max(island => island.Coordinates.Y);
        var hashiField = new int[maxY + 1][];
        for (var i = 0; i <= maxY; i++)
        {
            hashiField[i] = new int[maxX + 1];
        }

        // Populate the Hashi field and map island coordinates
        foreach (var island in allIslands)
        {
            var x = island.Coordinates.X;
            var y = island.Coordinates.Y;
            hashiField[y][x] = island.MaxConnections;
        }

        // Create a list of bridge coordinates
        var bridgeCoordinates = new List<IBridgeCoordinates>();

        foreach (var island in allIslands.Where(x => x.MaxConnections > 0))
        {
            // Group connections by their target coordinates
            var groupedConnections = island.AllConnections
                .GroupBy(connection => new { connection.X, connection.Y })
                .Select(group => new
                {
                    Target = group.Key,
                    Amount = group.Count()
                });

            foreach (var connectionGroup in groupedConnections)
            {
                var bridge = new BridgeCoordinates(
                    new Point(island.Coordinates.X, island.Coordinates.Y),
                    new Point(connectionGroup.Target.X, connectionGroup.Target.Y),
                    connectionGroup.Amount
                );

                // Avoid duplicate bridges by checking if the reverse connection already exists
                if (!bridgeCoordinates.Any(b =>
                        (b.Location1 == bridge.Location1 && b.Location2 == bridge.Location2) ||
                        (b.Location1 == bridge.Location2 && b.Location2 == bridge.Location1)))
                {
                    bridgeCoordinates.Add(bridge);
                }
            }
        }

        bridgeCoordinates = bridgeCoordinates.Distinct().ToList();

        return solutionProviderFactory(hashiField, bridgeCoordinates, solutionName);
    }
}
