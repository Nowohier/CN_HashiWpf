using Hashi.LinearSolver.Interfaces.Models;

namespace Hashi.LinearSolver.Models;

/// <summary>
/// Represents an island in the Hashiwokakero puzzle.
/// </summary>
/// <param name="id">The id.</param>
/// <param name="row">The row.</param>
/// <param name="col">The column.</param>
/// <param name="bridgesRequired">The amount of bridges required.</param>
public class Island(int id, int row, int col, int bridgesRequired) : IIsland
{
    /// <inheritdoc />
    public int Id { get; } = id;

    /// <inheritdoc />
    public int Row { get; } = row;

    /// <inheritdoc />
    public int Col { get; } = col;

    /// <inheritdoc />
    public int BridgesRequired { get; } = bridgesRequired;

    /// <inheritdoc />
    public IReadOnlyList<int> Neighbors => neighbors;

    /// <inheritdoc />
    public void AddNeighbor(int neighborId)
    {
        neighbors.Add(neighborId);
    }

    private readonly List<int> neighbors = [];
}
