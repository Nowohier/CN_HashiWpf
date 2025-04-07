using Hashi.LinearSolver.Interfaces.Models;

namespace Hashi.LinearSolver.Models;

/// <inheritdoc cref="IBridgePair" />
public class BridgePair : IBridgePair
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BridgePair" /> class.
    /// </summary>
    /// <param name="bridge1Node1">The first node of bridge 1.</param>
    /// <param name="bridge1Node2">The second node of bridge 1.</param>
    /// <param name="bridge2Node1">The first node of bridge 2.</param>
    /// <param name="bridge2Node2">The second node of bridge 2.</param>
    public BridgePair(int bridge1Node1, int bridge1Node2, int bridge2Node1, int bridge2Node2)
    {
        Bridge1 = new[] { bridge1Node1, bridge1Node2 };
        Bridge2 = new[] { bridge2Node1, bridge2Node2 };
    }

    /// <inheritdoc />
    public int[] Bridge1 { get; }

    /// <inheritdoc />
    public int[] Bridge2 { get; }
}