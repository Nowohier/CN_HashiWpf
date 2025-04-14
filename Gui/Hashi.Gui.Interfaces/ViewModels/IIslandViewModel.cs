using Hashi.Enums;
using Hashi.Gui.Interfaces.Models;
using System.Collections.ObjectModel;

namespace Hashi.Gui.Interfaces.ViewModels;

/// <summary>
///     Represents the view model for an island in the Hashi game.
/// </summary>
public interface IIslandViewModel
{
    /// <summary>
    ///     Gets or sets the color of the island.
    /// </summary>
    IHashiBrush IslandColor { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the path to the island is highlighted left horizontally.
    /// </summary>
    bool IsHighlightHorizontalLeft { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the path to the island is highlighted right horizontally.
    /// </summary>
    bool IsHighlightHorizontalRight { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the path to the island is highlighted vertically top.
    /// </summary>
    bool IsHighlightVerticalTop { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the path to the island is highlighted vertically bottom.
    /// </summary>
    bool IsHighlightVerticalBottom { get; set; }

    /// <summary>
    ///     A list of all set connections for this island.
    /// </summary>
    ObservableCollection<IHashiPoint> AllConnections { get; }

    /// <summary>
    ///     Determines if the max connections have been reached
    /// </summary>
    bool MaxConnectionsReached { get; }

    /// <summary>
    ///     Gets the bridges on the left side.
    /// </summary>
    List<IHashiPoint> BridgesLeft { get; }

    /// <summary>
    ///     Gets the bridges on the right side.
    /// </summary>
    List<IHashiPoint> BridgesRight { get; }

    /// <summary>
    ///     Gets the bridges on the bottom side.
    /// </summary>
    List<IHashiPoint> BridgesUp { get; }

    /// <summary>
    ///     Gets the bridges on the top side.
    /// </summary>
    List<IHashiPoint> BridgesDown { get; }

    /// <summary>
    ///     Gets the maximum number of connections for this island.
    /// </summary>
    int MaxConnections { get; }

    /// <summary>
    ///     Gets the remaining number of connections for this island.
    /// </summary>
    int RemainingConnections { get; }

    /// <summary>
    ///     Gets the coordinates of the island.
    /// </summary>
    IHashiPoint Coordinates { get; }

    /// <summary>
    ///     Resets the drop target.
    /// </summary>
    void ResetDropTarget();

    /// <summary>
    ///     Refreshes the island color and sets it to the default color if it is not set.
    /// </summary>
    void RefreshIslandColor();

    /// <summary>
    ///     Gets the connection type between two islands.
    /// </summary>
    /// <param name="targetIsland">The island to perform the check on.</param>
    /// <returns>a <see cref="ConnectionTypeEnum" />.</returns>
    ConnectionTypeEnum GetConnectionType(IIslandViewModel targetIsland);

    /// <summary>
    ///     Gets all neighbors with MaxConnections > 0 and no colliding connections.
    /// </summary>
    /// <returns>a list of visible neighbor islands.</returns>
    List<IIslandViewModel> GetAllVisibleNeighbors();

    /// <summary>
    ///     Gets the visible neighbor of the current island in the given direction of potential target island.
    /// </summary>
    /// <param name="potentialTargetIsland">The potential target island to give a hint in which direction to search.</param>
    /// <returns>
    ///     the visible neighbor of the current island in the given direction of potential target island. Null if not
    ///     found.
    /// </returns>
    IIslandViewModel? GetVisibleNeighbor(IIslandViewModel potentialTargetIsland);

    /// <summary>
    ///     Notifies the bridge connections.
    /// </summary>
    void NotifyBridgeConnections();

    /// <summary>
    ///     Adds a connection.
    /// </summary>
    /// <param name="connection">The connection to add.</param>
    void AddConnection(IHashiPoint connection);

    /// <summary>
    ///     Removes all connections matching the given connection.
    /// </summary>
    /// <param name="connection">The connection to remove.</param>
    void RemoveAllConnectionsMatchingCoordinates(IHashiPoint connection);

    /// <summary>
    /// Determines if the island is a valid drop target.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    bool IsValidDropTarget(IIslandViewModel? target);

    /// <summary>
    /// Determines if the maximum number of bridges has been reached to the target island. Returns null if target is null.
    /// </summary>
    /// <param name="target">The target island.</param>
    /// <returns>a boolean value if the maximum number of bridges has been reached to the target island. Returns null if target is null.</returns>
    bool? MaxBridgesReachedToTarget(IIslandViewModel? target);
}