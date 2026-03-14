using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;

namespace Hashi.Rules.Helpers;

/// <summary>
///     Provides query and analysis methods for rule neighbor inspection.
/// </summary>
public class RuleNeighborAnalyzer : IRuleNeighborAnalyzer
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

    /// <inheritdoc />
    public List<IIslandViewModel> GetAllVisibleNeighbors(IIslandViewModel source)
    {
        return islandProvider.GetAllVisibleNeighbors(source);
    }

    /// <inheritdoc />
    public List<IIslandViewModel> GetConnectableNeighbors(IEnumerable<IIslandViewModel> allNeighbors)
    {
        return allNeighbors.Where(x => !x.MaxConnectionsReached).ToList();
    }

    /// <inheritdoc />
    public List<IIslandViewModel> GetConnectableNeighborsWithoutConnection(IIslandViewModel source,
        IEnumerable<IIslandViewModel> allNeighbors)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(allNeighbors);

        return GetConnectableNeighbors(allNeighbors).Where(x =>
            !x.AllConnections.Any(connection => DoCoordinatesMatch(source.Coordinates, connection))).ToList();
    }

    /// <inheritdoc />
    public bool AreAllNeighborsConnected(IIslandViewModel source, IEnumerable<IIslandViewModel> allNeighbors)
    {
        return allNeighbors.All(x =>
            x.AllConnections.Any(connection => DoCoordinatesMatch(source.Coordinates, connection)));
    }

    /// <inheritdoc />
    public List<IIslandViewModel> GetConnectedNeighbors(IIslandViewModel source,
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

    /// <inheritdoc />
    public int CountConnectionsToNeighbors(IIslandViewModel source, IEnumerable<IIslandViewModel> neighbors)
    {
        var result = neighbors.Sum(x => x.AllConnections.Count(y => DoCoordinatesMatch(source.Coordinates, y)));
        return result;
    }

    /// <inheritdoc />
    public bool AreRemainingConnectionsWithinRange(IIslandViewModel source, int minValue, int maxValue)
    {
        var result = source.RemainingConnections >= minValue && source.RemainingConnections <= maxValue;
        return result;
    }

    /// <inheritdoc />
    public List<IIslandViewModel> GetMaxedOutConnectedNeighbors(IIslandViewModel source,
        IEnumerable<IIslandViewModel> allNeighbors, int? amountConnections)
    {
        var result = GetConnectedNeighbors(source, allNeighbors, amountConnections).Where(x => x.MaxConnectionsReached)
            .ToList();
        return result;
    }

    /// <inheritdoc />
    public bool DoCoordinatesMatch(IHashiPoint source, IHashiPoint target)
    {
        return source.X == target.X && source.Y == target.Y;
    }
}
