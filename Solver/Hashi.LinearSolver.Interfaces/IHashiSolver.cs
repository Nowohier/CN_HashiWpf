using Google.OrTools.Sat;
using Hashi.Enums;
using Hashi.LinearSolver.Interfaces.Models;

namespace Hashi.LinearSolver.Interfaces
{
    public interface IHashiSolver
    {
        /// <summary>
        /// Solves the Hashi puzzle from the given file using the Google OR-Tools CP-SAT solver. This method is considerably slower than the
        /// lazy version, as it does not use lazy constraints to iteratively refine the solution.
        /// </summary>
        /// <param name="file">The file to read from</param>
        /// <returns>A double value representing the seconds taken to solve the puzzle.</returns>
        SolverStatusEnum Solve(string file);

        /// <summary>
        /// Converts the given 2D array of integers into a list of islands and a list of intersections.
        /// </summary>
        /// <param name="data">The 2D array.</param>
        /// <returns>A list of islands and a list of intersections.</returns>
        (List<IIsland>, List<(int, int, int, int)>) ConvertData(int[][] data);

        /// <summary>
        /// Reads a Hashi puzzle from a file and converts it into a list of islands and intersections.
        /// </summary>
        /// <param name="file">The file path.</param>
        /// <returns>A list of islands and a list of intersections.</returns>
        (List<IIsland>, List<(int, int, int, int)>) ReadFile(string file);

        /// <summary>
        /// Pretty prints the solution of the Hashi puzzle in a human-readable format. This method constructs a grid representation of the puzzle, displaying islands and bridges based on the solution provided by the solver.
        /// </summary>
        /// <param name="islands">The islands.</param>
        /// <param name="x">x is a 2D array of boolean decision variables representing the number of bridges between islands.</param>
        /// <param name="solver">The <see cref="CpSolver"/> instance.</param>
        /// <param name="edgeMap">A dictionary with <see cref="IEdge"/> mapping information.</param>
        void PrettyPrint(List<IIsland> islands, IntVar[,] x, CpSolver solver, Dictionary<int, IEdge> edgeMap);

        /// <summary>
        /// Solves the Hashi puzzle using lazy constraints. This method iteratively refines the solution by adding cuts for disconnected components until a connected solution is found or declared infeasible.
        /// </summary>
        /// <param name="data">The 2D array of integer data representing the hashi field.</param>
        /// <returns>The state after trying to resolve the puzzle.</returns>
        SolverStatusEnum SolveLazy(int[][] data);

        /// <summary>
        /// Solves the Hashi puzzle from the given file using lazy constraints. This method iteratively refines the solution by adding cuts for disconnected components until a connected solution is found or declared infeasible.
        /// </summary>
        /// <param name="file">The file path.</param>
        /// <returns>The state after trying to resolve the puzzle.</returns>
        SolverStatusEnum SolveLazy(string file);

        /// <summary>
        /// Solves the Hashi puzzle using lazy constraints. This method iteratively refines the solution by adding cuts for disconnected components until a connected solution is found or declared infeasible.
        /// </summary>
        /// <param name="islands">A list of Hashi islands.</param>
        /// <param name="intersections">A list of intersections.</param>
        /// <returns></returns>
        SolverStatusEnum SolveLazy(List<IIsland> islands, List<(int, int, int, int)> intersections);
    }
}
