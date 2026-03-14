using Hashi.Enums;
using Hashi.Gui.Interfaces.Models;
using System.Collections.ObjectModel;

namespace Hashi.Gui.Interfaces.ViewModels;

/// <summary>
///     Provides coordinate, connection count, and connection management data for an island.
/// </summary>
public interface IIslandData
{
    /// <summary>
    ///     Gets the coordinates of the island.
    /// </summary>
    IHashiPoint Coordinates { get; }

    /// <summary>
    ///     Gets the maximum number of connections for this island.
    /// </summary>
    int MaxConnections { get; }

    /// <summary>
    ///     Gets the remaining number of connections for this island.
    /// </summary>
    int RemainingConnections { get; }

    /// <summary>
    ///     Determines if the max connections have been reached.
    /// </summary>
    bool MaxConnectionsReached { get; }

    /// <summary>
    ///     A list of all set connections for this island.
    /// </summary>
    ObservableCollection<IHashiPoint> AllConnections { get; }

    /// <summary>
    ///     Gets the connection type between two islands.
    /// </summary>
    ConnectionTypeEnum GetConnectionType(IIslandViewModel targetIsland);

    /// <summary>
    ///     Adds a connection.
    /// </summary>
    void AddConnection(IHashiPoint connection);

    /// <summary>
    ///     Removes all connections matching the given connection.
    /// </summary>
    void RemoveAllConnectionsMatchingCoordinates(IHashiPoint connection);
}
