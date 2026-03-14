namespace Hashi.Enums;

/// <summary>
///     Represents the status of the solver.
/// </summary>
public enum SolverStatusEnum
{
    Unknown = 0,
    ModelInvalid = 1,
    Feasible = 2,
    Infeasible = 3,
    Optimal = 4
}
