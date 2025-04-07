using System.Windows.Input;
using Hashi.Gui.Enums;
using Hashi.Gui.Interfaces.Models;

namespace Hashi.Gui.Interfaces.ViewModels;

public interface IIslandViewModel : IBaseViewModel
{
    /// <summary>
    ///     Gets or sets the color of the island.
    /// </summary>
    public IHashiBrush IslandColor { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the path to the island is highlighted left horizontally.
    /// </summary>
    public bool IsHighlightHorizontalLeft { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the path to the island is highlighted right horizontally.
    /// </summary>
    public bool IsHighlightHorizontalRight { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the path to the island is highlighted vertically top.
    /// </summary>
    public bool IsHighlightVerticalTop { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the path to the island is highlighted vertically bottom.
    /// </summary>
    public bool IsHighlightVerticalBottom { get; set; }

    public IHashiPoint? PotentialTargetIslandCoordinates { get; set; }

    /// <summary>
    ///     A list of all set connections for this island.
    /// </summary>
    public List<IHashiPoint> AllConnections { get; }

    /// <summary>
    ///     Determines if the max connections have been reached
    /// </summary>
    public bool MaxConnectionsReached { get; }

    /// <summary>
    ///     Gets the bridges on the left side.
    /// </summary>
    public List<IHashiPoint> BridgesLeft { get; }

    /// <summary>
    ///     Gets the bridges on the right side.
    /// </summary>
    public List<IHashiPoint> BridgesRight { get; }

    /// <summary>
    ///     Gets the bridges on the bottom side.
    /// </summary>
    public List<IHashiPoint> BridgesUp { get; }

    /// <summary>
    ///     Gets the bridges on the top side.
    /// </summary>
    public List<IHashiPoint> BridgesDown { get; }

    /// <summary>
    ///     Gets the maximum number of connections for this island.
    /// </summary>
    public int MaxConnections { get; }

    /// <summary>
    ///     Gets the coordinates of the island.
    /// </summary>
    public IHashiPoint Coordinates { get; }

    /// <summary>
    ///     Gets the command for the left mouse button down event.
    /// </summary>
    public ICommand MouseLeftButtonDownCommand { get; }

    /// <summary>
    ///     Gets the command for the left mouse button up event.
    /// </summary>
    public ICommand MouseLeftButtonUpCommand { get; }

    /// <summary>
    ///     Gets the command for the drag enter event.
    /// </summary>
    public ICommand DragEnterCommand { get; }

    /// <summary>
    ///     Gets the command for the drag over event.
    /// </summary>
    public ICommand DragOverCommand { get; }

    /// <summary>
    ///     Gets the command for the drag leave event.
    /// </summary>
    public ICommand DragLeaveCommand { get; }

    /// <summary>
    ///     Gets the command for the drop event.
    /// </summary>
    public ICommand DropCommand { get; }

    /// <summary>
    ///     Gets the command for the mouse move event.
    /// </summary>
    public ICommand MouseMoveCommand { get; }

    /// <summary>
    ///     Gets the connection type between two islands.
    /// </summary>
    /// <param name="targetIsland">The island to perform the check on.</param>
    /// <returns>a <see cref="ConnectionTypeEnum" />.</returns>
    public ConnectionTypeEnum GetConnectionType(IIslandViewModel targetIsland);

    /// <summary>
    ///     Notifies the bridge connections.
    /// </summary>
    public void NotifyBridgeConnections();

    /// <summary>
    ///     Adds a connection.
    /// </summary>
    /// <param name="connection">The connection to add.</param>
    public void AddConnection(IHashiPoint connection);

    /// <summary>
    ///     Removes all connections matching the given connection.
    /// </summary>
    /// <param name="connection">The connection to remove.</param>
    public void RemoveAllConnectionsMatchingCoordinates(IHashiPoint connection);
}