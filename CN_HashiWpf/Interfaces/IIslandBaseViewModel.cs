using CNHashiWpf.Enums;
using CNHashiWpf.ViewModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CNHashiWpf.Interfaces
{
    public interface IIslandBaseViewModel
    {
        /// <summary>
        /// Gets or sets the color of the island.
        /// </summary>
        Brush IslandColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the path to the island is highlighted left horizontally.
        /// </summary>
        bool IsHighlightHorizontalLeft { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the path to the island is highlighted right horizontally.
        /// </summary>
        bool IsHighlightHorizontalRight { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the path to the island is highlighted vertically top.
        /// </summary>
        bool IsHighlightVerticalTop { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the path to the island is highlighted vertically bottom.
        /// </summary>
        bool IsHighlightVerticalBottom { get; set; }

        Point? PotentialTargetIslandCoordinates { get; set; }

        /// <summary>
        /// A list of all set connections for this island.
        /// </summary>
        List<Point> AllConnections { get; }

        /// <summary>
        /// Determines if the max connections have been reached
        /// </summary>
        bool MaxConnectionsReached { get; }

        /// <summary>
        /// Gets the bridges on the left side.
        /// </summary>
        List<Point> BridgesLeft { get; }

        /// <summary>
        /// Gets the bridges on the right side.
        /// </summary>
        List<Point> BridgesRight { get; }

        /// <summary>
        /// Gets the bridges on the bottom side.
        /// </summary>
        List<Point> BridgesUp { get; }

        /// <summary>
        /// Gets the bridges on the top side.
        /// </summary>
        List<Point> BridgesDown { get; }

        /// <summary>
        /// Gets the maximum number of connections for this island.
        /// </summary>
        int MaxConnections { get; }

        /// <summary>
        /// Gets the coordinates of the island.
        /// </summary>
        Point Coordinates { get; }

        /// <summary>
        /// Gets the command for the left mouse button down event.
        /// </summary>
        ICommand MouseLeftButtonDownCommand { get; }

        /// <summary>
        /// Gets the command for the left mouse button up event.
        /// </summary>
        ICommand MouseLeftButtonUpCommand { get; }

        /// <summary>
        /// Gets the command for the drag enter event.
        /// </summary>
        ICommand DragEnterCommand { get; }

        /// <summary>
        /// Gets the command for the drag over event.
        /// </summary>
        ICommand DragOverCommand { get; }

        /// <summary>
        /// Gets the command for the drag leave event.
        /// </summary>
        ICommand DragLeaveCommand { get; }

        /// <summary>
        /// Gets the command for the drop event.
        /// </summary>
        ICommand DropCommand { get; }

        /// <summary>
        /// Gets the command for the mouse move event.
        /// </summary>
        ICommand MouseMoveCommand { get; }

        /// <summary>
        /// Gets the connection type between two islands.
        /// </summary>
        /// <param name="targetIsland">The island to perform the check on.</param>
        /// <returns>a <see cref="ConnectionTypeEnum"/>.</returns>
        ConnectionTypeEnum GetConnectionType(IslandViewModel targetIsland);

        /// <summary>
        /// Notifies the bridge connections.
        /// </summary>
        void NotifyBridgeConnections();

        event PropertyChangedEventHandler? PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string propertyName = null);
        bool Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = "");
    }
}
