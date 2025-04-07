using Hashi.Gui.Enums;
using Hashi.LinearSolver.Interfaces.Models;

namespace Hashi.LinearSolver.Interfaces;

/// <summary>
///     Interface for solving linear equations with iterative methods.
/// </summary>
public interface ILinearSolutionSolverWithIterativ
{
    /// <summary>
    ///     Solves the linear equations represented by the main field.
    /// </summary>
    /// <param name="mainField">The generated main hashi field.</param>
    /// <returns>a status enum.</returns>
    SolverStatusEnum Solve(int[][] mainField);

    /// <summary>
    /// </summary>
    /// <param name="bridges"></param>
    /// <param name="amountIslands"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    List<IHelper> FindComponents(List<IBridge> bridges, int amountIslands, long[] value);

    /// <summary>
    ///     Prints the bridges on the field.
    /// </summary>
    /// <param name="bridge"></param>
    /// <param name="edgeNumber"></param>
    void PrintBridges(IBridge bridge, long edgeNumber);
}