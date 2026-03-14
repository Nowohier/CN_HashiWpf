namespace Hashi.Generator.Interfaces.Models;

/// <summary>
///     Interface for an island in the Hashi game.
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
    ///     Gets the amount of bridges connectable.
    /// </summary>
    int AmountBridgesConnectable { get; }

    /// <summary>
    ///     Gets the amount of bridges up.
    /// </summary>
    int AmountBridgesUp { get; }

    /// <summary>
    ///     Gets the amount of bridges down.
    /// </summary>
    int AmountBridgesDown { get; }

    /// <summary>
    ///     Gets the amount of bridges left.
    /// </summary>
    int AmountBridgesLeft { get; }

    /// <summary>
    ///     Gets the amount of bridges right.
    /// </summary>
    int AmountBridgesRight { get; }

    /// <summary>
    ///     Increments the connectable bridge count by the specified delta.
    /// </summary>
    void IncrementAmountBridgesConnectable(int delta);

    /// <summary>
    ///     Increments the directional bridge count by the specified delta.
    /// </summary>
    void IncrementAmountBridgesUp(int delta);

    /// <inheritdoc cref="IncrementAmountBridgesUp"/>
    void IncrementAmountBridgesDown(int delta);

    /// <inheritdoc cref="IncrementAmountBridgesUp"/>
    void IncrementAmountBridgesLeft(int delta);

    /// <inheritdoc cref="IncrementAmountBridgesUp"/>
    void IncrementAmountBridgesRight(int delta);

    /// <summary>
    ///     Sets the directional bridge count to the specified value.
    /// </summary>
    void SetAmountBridgesUp(int value);

    /// <inheritdoc cref="SetAmountBridgesUp"/>
    void SetAmountBridgesDown(int value);

    /// <inheritdoc cref="SetAmountBridgesUp"/>
    void SetAmountBridgesLeft(int value);

    /// <inheritdoc cref="SetAmountBridgesUp"/>
    void SetAmountBridgesRight(int value);

    /// <summary>
    ///     Gets the upper neighbor.
    /// </summary>
    IIsland? IslandUp { get; }

    /// <summary>
    ///     Gets the lower neighbor.
    /// </summary>
    IIsland? IslandDown { get; }

    /// <summary>
    ///     Gets the left neighbor.
    /// </summary>
    IIsland? IslandLeft { get; }

    /// <summary>
    ///     Gets the right neighbor.
    /// </summary>
    IIsland? IslandRight { get; }

    /// <summary>
    ///     Set all neighbors of the island.
    /// </summary>
    /// <param name="field">The field array.</param>
    /// <param name="islands">The list of islands.</param>
    void SetAllNeighbors(int[][] field, IReadOnlyList<IIsland> islands);

    /// <summary>
    ///     Set all neighbors of the island using a pre-built lookup dictionary.
    /// </summary>
    /// <param name="field">The field array.</param>
    /// <param name="islandLookup">A dictionary mapping (Y, X) coordinates to islands.</param>
    void SetAllNeighbors(int[][] field, Dictionary<(int Y, int X), IIsland> islandLookup);
}