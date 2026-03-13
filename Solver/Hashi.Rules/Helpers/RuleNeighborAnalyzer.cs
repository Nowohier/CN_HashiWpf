using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;

namespace Hashi.Rules.Helpers;

/// <summary>
///     Provides query and analysis methods for rule neighbor inspection.
/// </summary>
public class RuleNeighborAnalyzer
{
    private readonly IIslandProvider islandProvider;

    /// <summary>
    ///     Initializes a new instance of the <see cref="RuleNeighborAnalyzer" /> class.
    /// </summary>
    public RuleNeighborAnalyzer(IIslandProvider islandProvider)
    {
        ArgumentNullException.ThrowIfNull(islandProvider);
        this.islandProvider = islandProvider;
    }

    /// <summary>
    ///     Gets all visible neighbors of the source island.
    /// </summary>
    internal List<IIslandViewModel> GetAllVisibleNeighbors(IIslandViewModel source)
    {
        return islandProvider.GetAllVisibleNeighbors(source);
    }

    /// <summary>
    ///     Gets the connectable neighbors that have not reached the maximum connections.
    /// </summary>
    internal List<IIslandViewModel> GetConnectableNeighbors(IEnumerable<IIslandViewModel> allNeighbors)
    {
        return allNeighbors.Where(x => !x.MaxConnectionsReached).ToList();
    }

    /// <summary>
    ///     Gets the connectable neighbors that do not have a connection set to the source island.
    /// </summary>
    internal List<IIslandViewModel> GetConnectableNeighborsWithoutConnection(IIslandViewModel source,
        IEnumerable<IIslandViewModel> allNeighbors)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(allNeighbors);

        return GetConnectableNeighbors(allNeighbors).Where(x =>
            !x.AllConnections.Any(connection => DoCoordinatesMatch(source.Coordinates, connection))).ToList();
    }

    /// <summary>
    ///     Checks if all islands are connected to the source island.
    /// </summary>
    internal bool AreAllNeighborsConnected(IIslandViewModel source, IEnumerable<IIslandViewModel> allNeighbors)
    {
        return allNeighbors.All(x =>
            x.AllConnections.Any(connection => DoCoordinatesMatch(source.Coordinates, connection)));
    }

    /// <summary>
    ///     Gets the islands connected to the source island.
    /// </summary>
    internal List<IIslandViewModel> GetConnectedNeighbors(IIslandViewModel source,
        IEnumerable<IIslandViewModel> allNeighbors, int? amountConnections)
    {
        if (amountConnections == null)
        {
            var result = allNeighbors.Where(x => x.AllConnections.Any(y => DoCoordinatesMatch(source.Coordinates, y)))
                .ToList();
            return result;
        }
        else
        {
            var result = allNeighbors
                .Where(x => x.AllConnections.Count(y => DoCoordinatesMatch(source.Coordinates, y)) ==
                            (int)amountConnections)
                .ToList();
            return result;
        }
    }

    /// <summary>
    ///     Gets the amount of connections to the source island from the neighbors.
    /// </summary>
    internal int CountConnectionsToNeighbors(IIslandViewModel source, IEnumerable<IIslandViewModel> neighbors)
    {
        var result = neighbors.Sum(x => x.AllConnections.Count(y => DoCoordinatesMatch(source.Coordinates, y)));
        return result;
    }

    /// <summary>
    ///     Checks if the remaining connections of the island are within the range of the two values.
    /// </summary>
    internal bool AreRemainingConnectionsWithinRange(IIslandViewModel source, int minValue, int maxValue)
    {
        var result = source.RemainingConnections >= minValue && source.RemainingConnections <= maxValue;
        return result;
    }

    /// <summary>
    ///     Gets the islands connected to the source island which have reached the maximum connections.
    /// </summary>
    internal List<IIslandViewModel> GetMaxedOutConnectedNeighbors(IIslandViewModel source,
        IEnumerable<IIslandViewModel> allNeighbors, int? amountConnections)
    {
        var result = GetConnectedNeighbors(source, allNeighbors, amountConnections).Where(x => x.MaxConnectionsReached)
            .ToList();
        return result;
    }

    /// <summary>
    ///     Compares two HashiPoint coordinates.
    /// </summary>
    internal bool DoCoordinatesMatch(IHashiPoint source, IHashiPoint target)
    {
        return source.X == target.X && source.Y == target.Y;
    }
}
