namespace Hashi.LinearSolver.Interfaces.Models;

/// <summary>
/// Represents an edge in the Hashi puzzle, connecting two islands.
/// </summary>
public interface IEdge
{
    /// <summary>
    /// Represents an edge between two islands in the Hashi puzzle.
    /// </summary>
    int Id { get; }

    /// <summary>
    /// The ID of the first island in the edge.
    /// </summary>
    int IslandA { get; }

    /// <summary>
    /// The ID of the second island in the edge.
    /// </summary>
    int IslandB { get; }
}
