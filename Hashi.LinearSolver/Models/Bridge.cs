using Hashi.LinearSolver.Interfaces.Models;

namespace Hashi.LinearSolver.Models;

/// <inheritdoc cref="IBridge" />
public class Bridge : IBridge
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Bridge" /> class.
    /// </summary>
    /// <param name="island1">The first island.</param>
    /// <param name="island2">The second island.</param>
    public Bridge(IIsland island1, IIsland island2)
    {
        if (island1.Number < island2.Number)
        {
            Island1 = island1;
            Island2 = island2;
        }
        else
        {
            Island1 = island2;
            Island2 = island1;
        }
    }

    /// <inheritdoc />
    public IIsland Island1 { get; }

    /// <inheritdoc />
    public IIsland Island2 { get; }
}