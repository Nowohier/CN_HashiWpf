using Hashi.Enums;
using Hashi.LinearSolver.Interfaces.Models;

namespace Hashi.LinearSolver.Interfaces;

/// <summary>
///     Converts puzzle data and solves Hashi puzzles using constraint solving.
/// </summary>
public interface IPuzzleSolver
{
    /// <summary>
    ///     Converts the given 2D array of integers into a list of islands and a list of intersections.
    /// </summary>
    /// <param name="data">The 2D array.</param>
    /// <returns>A list of islands and a list of intersections.</returns>
    Task<(List<IIsland>, List<(int, int, int, int)>)> ConvertData(int[][] data);

    /// <summary>
    ///     Solves the Hashi puzzle using lazy constraints.
    /// </summary>
    /// <param name="data">The 2D array of integer data representing the hashi field.</param>
    /// <param name="prettyPrint">Determines if the solution should be printed.</param>
    /// <returns>The state after trying to resolve the puzzle.</returns>
    Task<SolverStatusEnum> SolveLazy(int[][] data, bool prettyPrint = true);

    /// <summary>
    ///     Solves the Hashi puzzle from the given file using lazy constraints.
    /// </summary>
    /// <param name="file">The file path.</param>
    /// <param name="prettyPrint">Determines if the solution should be printed.</param>
    /// <returns>The state after trying to resolve the puzzle.</returns>
    Task<SolverStatusEnum> SolveLazy(string file, bool prettyPrint = true);

    /// <summary>
    ///     Solves the Hashi puzzle using lazy constraints with pre-parsed data.
    /// </summary>
    /// <param name="islands">A list of Hashi islands.</param>
    /// <param name="intersections">A list of intersections.</param>
    /// <param name="prettyPrint">Determines if the solution should be printed.</param>
    /// <returns>The state after trying to resolve the puzzle.</returns>
    Task<SolverStatusEnum> SolveLazy(List<IIsland> islands, List<(int, int, int, int)> intersections,
        bool prettyPrint = true);
}
