using Google.OrTools.Sat;
using Hashi.LinearSolver.Interfaces.Models;

namespace Hashi.LinearSolver.Interfaces;

/// <summary>
///     Composite interface for reading, solving, and visualizing Hashi puzzles.
///     Combines <see cref="IPuzzleReader" />, <see cref="IPuzzleSolver" />,
///     and <see cref="IPuzzleVisualizer" />.
/// </summary>
public interface IHashiSolver : IPuzzleReader, IPuzzleSolver, IPuzzleVisualizer
{
    /// <summary>
    ///     Pretty prints the solution using OR-Tools solver internals.
    ///     Prefer <see cref="IPuzzleVisualizer.PrettyPrint(List{IIsland}, IReadOnlyList{SolvedEdge}, Dictionary{int, IEdge})" />
    ///     for abstracted visualization.
    /// </summary>
    /// <param name="islands">The islands.</param>
    /// <param name="x">A 2D array of boolean decision variables representing the number of bridges between islands.</param>
    /// <param name="solver">The <see cref="CpSolver" /> instance.</param>
    /// <param name="edgeMap">A dictionary with <see cref="IEdge" /> mapping information.</param>
    Task PrettyPrint(List<IIsland> islands, IntVar[,] x, CpSolver solver, Dictionary<int, IEdge> edgeMap);
}
