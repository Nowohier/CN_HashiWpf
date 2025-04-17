using Hashi.LinearSolver.Interfaces.Models;

namespace Hashi.LinearSolver.Models;

/// <inheritdoc cref="IHelper" />
/// >
public class Helper(IList<int> islands, IList<int> bridges) : IHelper
{
    /// <inheritdoc />
    public IList<int> Islands { get; } = islands;

    /// <inheritdoc />
    public IList<int> Bridges { get; } = bridges;
}