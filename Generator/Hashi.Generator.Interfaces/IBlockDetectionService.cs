using Hashi.Enums;
using Hashi.Generator.Interfaces.Models;

namespace Hashi.Generator.Interfaces;

/// <summary>
///     Detects blocked paths between islands on the game field.
/// </summary>
public interface IBlockDetectionService
{
    /// <summary>
    ///     Gets the blocked position in the given direction from the island.
    /// </summary>
    int GetBlocked(IIsland mainIsland, int[][] mainField, DirectionEnum direction, IList<IBridge> bridges);

    /// <summary>
    ///     Gets the blocked position between an island and its downward neighbor.
    /// </summary>
    int GetDownBlockedBetween(IIsland mainIsland, int[][] mainField, IList<IBridge> bridges);

    /// <summary>
    ///     Gets the blocked position between an island and its rightward neighbor.
    /// </summary>
    int GetRightBlockedBetween(IIsland mainIsland, int[][] mainField, IList<IBridge> bridges);

    /// <summary>
    ///     Checks if the island has a bridge in the given direction.
    /// </summary>
    bool HasBridgeInDirection(IIsland island, DirectionEnum direction, IList<IBridge> bridges);

    /// <summary>
    ///     Clears all cached block detection results.
    /// </summary>
    void ClearCaches();
}
