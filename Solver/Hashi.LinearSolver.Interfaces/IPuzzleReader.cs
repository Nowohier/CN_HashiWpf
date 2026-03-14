using Hashi.LinearSolver.Interfaces.Models;

namespace Hashi.LinearSolver.Interfaces;

/// <summary>
///     Reads Hashi puzzle data from files or arrays.
/// </summary>
public interface IPuzzleReader
{
    /// <summary>
    ///     Reads a Hashi puzzle from a file and converts it into a list of islands and intersections.
    /// </summary>
    /// <param name="file">The file path.</param>
    /// <returns>A list of islands and a list of intersections.</returns>
    Task<(List<IIsland>, List<(int, int, int, int)>)> ReadFile(string file);
}
