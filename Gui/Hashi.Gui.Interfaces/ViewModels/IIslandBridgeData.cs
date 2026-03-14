using Hashi.Gui.Interfaces.Models;

namespace Hashi.Gui.Interfaces.ViewModels;

/// <summary>
///     Provides directional bridge data for an island.
/// </summary>
public interface IIslandBridgeData
{
    /// <summary>
    ///     Gets the bridges on the left side.
    /// </summary>
    List<IHashiPoint> BridgesLeft { get; }

    /// <summary>
    ///     Gets the bridges on the right side.
    /// </summary>
    List<IHashiPoint> BridgesRight { get; }

    /// <summary>
    ///     Gets the bridges on the top side.
    /// </summary>
    List<IHashiPoint> BridgesUp { get; }

    /// <summary>
    ///     Gets the bridges on the bottom side.
    /// </summary>
    List<IHashiPoint> BridgesDown { get; }

    /// <summary>
    ///     Notifies the bridge connections.
    /// </summary>
    void NotifyBridgeConnections();
}
