using Hashi.Enums;
using Hashi.Gui.Interfaces.ViewModels;

namespace Hashi.Gui.Interfaces.Providers;

/// <summary>
///     Provides connection management operations for islands.
/// </summary>
public interface IIslandConnectionProvider
{
    /// <summary>
    ///     Adds a connection between two islands.
    /// </summary>
    /// <param name="sourceIsland">The source island.</param>
    /// <param name="targetIsland">The target island.</param>
    /// <param name="pointType">Determines the point type.</param>
    void AddConnection(IIslandViewModel? sourceIsland, IIslandViewModel? targetIsland,
        HashiPointTypeEnum pointType = HashiPointTypeEnum.Normal);

    /// <summary>
    ///     Removes a connection between two islands.
    /// </summary>
    /// <param name="sourceIsland">The source island.</param>
    /// <param name="targetIsland">The target island.</param>
    void RemoveAllConnections(IIslandViewModel? sourceIsland, IIslandViewModel? targetIsland);

    /// <summary>
    ///     Removes all bridges from the islands.
    /// </summary>
    /// <param name="pointType">The point type. Default is <see cref="HashiPointTypeEnum.All" />.</param>
    void RemoveAllBridges(HashiPointTypeEnum pointType);

    /// <summary>
    ///     Counts the number of isolated island groups in the game.
    /// </summary>
    /// <returns>The number of isolated groups.</returns>
    int CountIsolatedIslandGroups();

    /// <summary>
    ///     Gets the visible neighbor of the source island depending on the direction to the potential target island. The
    ///     potential target is not necessarily a visible neighbor.
    /// </summary>
    /// <param name="source">The source island.</param>
    /// <param name="direction">The direction.</param>
    /// <returns>the next visible neighbor in the given direction.</returns>
    IIslandViewModel? GetVisibleNeighbor(IIslandViewModel source, DirectionEnum direction);

    /// <summary>
    ///     Gets all visible neighbors of the source island.
    /// </summary>
    /// <param name="source">The source island.</param>
    /// <returns>all visible neighbors of the source island.</returns>
    List<IIslandViewModel> GetAllVisibleNeighbors(IIslandViewModel source);
}
