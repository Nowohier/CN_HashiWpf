using Hashi.LinearSolver.Interfaces.Models;

namespace Hashi.LinearSolver.Interfaces;

/// <summary>
///     Represents a solved edge with its bridge count for visualization purposes.
/// </summary>
/// <param name="Island1Id">The ID of the first island.</param>
/// <param name="Island2Id">The ID of the second island.</param>
/// <param name="BridgeCount">The number of bridges on this edge.</param>
public record SolvedEdge(int Island1Id, int Island2Id, long BridgeCount);

/// <summary>
///     Visualizes solved Hashi puzzles.
/// </summary>
public interface IPuzzleVisualizer
{
    /// <summary>
    ///     Pretty prints the solution of the Hashi puzzle in a human-readable format.
    /// </summary>
    /// <param name="islands">The islands.</param>
    /// <param name="solvedEdges">The solved edges with bridge counts.</param>
    /// <param name="edgeMap">A dictionary with edge mapping information.</param>
    Task PrettyPrint(List<IIsland> islands, IReadOnlyList<SolvedEdge> solvedEdges,
        Dictionary<int, IEdge> edgeMap);
}
