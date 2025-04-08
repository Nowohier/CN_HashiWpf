using Hashi.Gui.Enums;
using Hashi.Gui.Interfaces.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Hashi.Gui.Interfaces.ViewModels;

/// <summary>
///   Represents the view model for an island in the Hashi game.
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
    ///     Gets the coordinates of the island.
    /// </summary>
    IHashiPoint Coordinates { get; }

    /// <summary>
    ///     Gets the command for the left mouse button down event.
    /// </summary>
    ICommand MouseLeftButtonDownCommand { get; }

    /// <summary>
    ///     Gets the command for the left mouse button up event.
    /// </summary>
    ICommand MouseLeftButtonUpCommand { get; }

    /// <summary>
    ///     Gets the command for the drag enter event.
    /// </summary>
    ICommand DragEnterCommand { get; }

    /// <summary>
    ///     Gets the command for the drag over event.
    /// </summary>
    ICommand DragOverCommand { get; }

    /// <summary>
    ///     Gets the command for the drag leave event.
    /// </summary>
    ICommand DragLeaveCommand { get; }

    /// <summary>
    ///     Gets the command for the drop event.
    /// </summary>
    ICommand DropCommand { get; }

    /// <summary>
    ///     Gets the command for the mouse move event.
    /// </summary>
    ICommand MouseMoveCommand { get; }

    /// <summary>
    /// Resets the drop target.
    /// </summary>
    void ResetDropTarget();

    /// <summary>
    /// Checks the island color and sets it to the default color if it is not set.
    /// </summary>
    void CheckIslandColor();

    /// <summary>
    ///     Gets the connection type between two islands.
    /// </summary>
    /// <param name="targetIsland">The island to perform the check on.</param>
    /// <returns>a <see cref="ConnectionTypeEnum" />.</returns>
    ConnectionTypeEnum GetConnectionType(IIslandViewModel targetIsland);

    /// <summary>
    /// Gets all neighbors with MaxConnections > 0.
    /// </summary>
    /// <param name="islands">All islands.</param>
    /// <returns>a list of visible neighbor islands.</returns>
    List<IIslandViewModel> GetAllVisibleNeighbors(ObservableCollection<ObservableCollection<IIslandViewModel>> islands);

    /// <summary>
    ///    Gets the visible neighbor of the current island in the given direction of potential target island.
    /// </summary>
    /// <param name="islands">All islands.</param>
    /// <param name="potentialTargetIsland">The potential target island to give a hint in which direction to search.</param>
    /// <returns>the visible neighbor of the current island in the given direction of potential target island. Null if not found.</returns>
    IIslandViewModel? GetVisibleNeighbor(ObservableCollection<ObservableCollection<IIslandViewModel>> islands,
        IIslandViewModel potentialTargetIsland);

    /// <summary>
    ///    Gets the visible neighbor of the current island in the given direction.
    /// </summary>
    /// <param name="direction">The direction.</param>
    /// <param name="islands">All islands.</param>
    /// <returns>the visible neighbor of the current island in the given direction. Null if not found.</returns>
    IIslandViewModel? GetVisibleNeighbor(DirectionEnum direction,
        ObservableCollection<ObservableCollection<IIslandViewModel>> islands);

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
}