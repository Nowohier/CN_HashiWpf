using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.ViewModels;

namespace Hashi.Rules.Helpers;

/// <summary>
///     Provides query and analysis methods for rule neighbor inspection.
/// </summary>
public interface IRuleNeighborAnalyzer
{
    /// <summary>
    ///     Gets all visible neighbors of the source island.
    /// </summary>
    List<IIslandViewModel> GetAllVisibleNeighbors(IIslandViewModel source);

    /// <summary>
    ///     Gets the connectable neighbors that have not reached the maximum connections.
    /// </summary>
    List<IIslandViewModel> GetConnectableNeighbors(IEnumerable<IIslandViewModel> allNeighbors);

    /// <summary>
    ///     Gets the connectable neighbors that do not have a connection set to the source island.
    /// </summary>
    List<IIslandViewModel> GetConnectableNeighborsWithoutConnection(IIslandViewModel source,
        IEnumerable<IIslandViewModel> allNeighbors);

    /// <summary>
    ///     Checks if all islands are connected to the source island.
    /// </summary>
    bool AreAllNeighborsConnected(IIslandViewModel source, IEnumerable<IIslandViewModel> allNeighbors);

    /// <summary>
    ///     Gets the islands connected to the source island.
    /// </summary>
    List<IIslandViewModel> GetConnectedNeighbors(IIslandViewModel source,
        IEnumerable<IIslandViewModel> allNeighbors, int? amountConnections);

    /// <summary>
    ///     Gets the amount of connections to the source island from the neighbors.
    /// </summary>
    int CountConnectionsToNeighbors(IIslandViewModel source, IEnumerable<IIslandViewModel> neighbors);

    /// <summary>
    ///     Checks if the remaining connections of the island are within the range of the two values.
    /// </summary>
    bool AreRemainingConnectionsWithinRange(IIslandViewModel source, int minValue, int maxValue);

    /// <summary>
    ///     Gets the islands connected to the source island which have reached the maximum connections.
    /// </summary>
    List<IIslandViewModel> GetMaxedOutConnectedNeighbors(IIslandViewModel source,
        IEnumerable<IIslandViewModel> allNeighbors, int? amountConnections);

    /// <summary>
    ///     Compares two HashiPoint coordinates.
    /// </summary>
    bool DoCoordinatesMatch(IHashiPoint source, IHashiPoint target);
}
