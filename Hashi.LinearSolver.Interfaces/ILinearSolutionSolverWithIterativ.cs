using Hashi.Enums;

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
    Task<SolverStatusEnum> SolveAsync(int[][] mainField);
}