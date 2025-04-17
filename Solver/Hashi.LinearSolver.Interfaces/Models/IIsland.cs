namespace Hashi.LinearSolver.Interfaces.Models;

/// <summary>
///     Represents an island in the Hashi game.
/// </summary>
public interface IIsland
{
    /// <summary>
    ///     Gets the y coordinate.
    /// </summary>
    int Y { get; }

    /// <summary>
    ///     Gets the x coordinate.
    /// </summary>
    int X { get; }

    /// <summary>
    ///     Gets the actual value of the island.
    /// </summary>
    int Value { get; set; }

    /// <summary>
    ///     Gets the upper neighbor.
    /// </summary>
    IIsland? Up { get; }

    /// <summary>
    ///     Gets the lower neighbor.
    /// </summary>
    IIsland? Down { get; }

    /// <summary>
    ///     Gets the right neighbor.
    /// </summary>
    IIsland? Right { get; }

    /// <summary>
    ///     Gets the left neighbor.
    /// </summary>
    IIsland? Left { get; }

    /// <summary>
    ///     Gets the island number.
    /// </summary>
    int Number { get; }

    /// <summary>
    ///     Gets the lower neighbors.
    /// </summary>
    List<IIsland> LowerNeighbors { get; }

    /// <summary>
    ///     Gets the upper neighbors.
    /// </summary>
    List<IIsland> UpNeighbors { get; }

    /// <summary>
    ///     Adds a bridge to the island.
    /// </summary>
    /// <param name="field">The array of fields.</param>
    /// <param name="islands">The list of islands.</param>
    void SetAllNeighbors(int[][] field, List<IIsland> islands);

    /// <summary>
    ///     Returns all islands between this island and the lower neighbor.
    /// </summary>
    /// <param name="island">A list of all islands.</param>
    /// <returns>all islands between this island and the lower neighbor.</returns>
    List<IIsland> DownBlocked(List<IIsland> island);

    /// <summary>
    ///     Returns all islands between this island and the right neighbor.
    /// </summary>
    /// <param name="island">A list of all islands.</param>
    /// <returns>all islands between this island and the right neighbor.</returns>
    List<IIsland> RightBlocked(List<IIsland> island);
}