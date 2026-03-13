using Hashi.Generator.Interfaces.Models;

namespace Hashi.Generator.Interfaces;

/// <summary>
///     Handles island placement and position calculations during puzzle generation.
/// </summary>
public interface IIslandLayoutService
{
    /// <summary>
    ///     Creates a new island adjacent to an existing island.
    /// </summary>
    /// <returns>True if an island was created.</returns>
    bool CreateIsland(int[][] mainField, IIsland island, List<IIsland> islands, List<IBridge> bridges);

    /// <summary>
    ///     Checks whether there is an adjacent island at the given position.
    /// </summary>
    bool HasAdjacentIsland(int row, int col, int[][] mainField);

    /// <summary>
    ///     Initializes an empty field with the given dimensions.
    /// </summary>
    int[][] InitializeField(int sizeLength, int sizeWidth);
}
