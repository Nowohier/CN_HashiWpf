using Hashi.LinearSolver.Interfaces.Models;

namespace Hashi.LinearSolver.Models;

/// <inheritdoc cref="IHelper" />
/// >
public class Helper : IHelper
{
    public Helper(IList<int> islands, IList<int> bridges)
    {
        Islands = islands;
        Bridges = bridges;
    }

    /// <inheritdoc />
    public IList<int> Islands { get; }

    /// <inheritdoc />
    public IList<int> Bridges { get; }
}