// ReSharper disable UnusedMember.Global

namespace Hashi.Enums;

/// <summary>
///     Represents the type of operation for the bridge.
/// </summary>
public enum BridgeOperationTypeEnum
{
    Add,
    Remove,
    RemoveAll,
    None
}

/// <summary>
///     Represents the type of connection for the bridge.
/// </summary>
public enum ConnectionTypeEnum
{
    Horizontal,
    Vertical,
    Diagonal
}

/// <summary>
///     Represents the direction of the bridge.
/// </summary>
public enum DirectionEnum
{
    Up,
    Down,
    Left,
    Right
}

/// <summary>
///     Represents the difficulty level of the puzzle.
/// </summary>
public enum DifficultyEnum
{
    Easy1 = 0,
    Easy2 = 1,
    Easy3 = 2,
    Medium1 = 3,
    Medium2 = 4,
    Medium3 = 5,
    Hard1 = 6,
    Hard2 = 7,
    Hard3 = 8,
    Expert = 9
}

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

/// <summary>
///     Represents the type of bridge.
/// </summary>
public enum BridgeTypeEnum
{
    Horizontal = 0,
    HorizontalLeft = 1,
    HorizontalRight = 2,
    HorizontalDouble = 3,
    HorizontalDoubleLeft = 4,
    HorizontalDoubleRight = 5,
    Vertical = 6,
    VerticalUp = 7,
    VerticalDown = 8,
    VerticalDouble = 9,
    VerticalDoubleUp = 10,
    VerticalDoubleDown = 11,
    None = 12
}