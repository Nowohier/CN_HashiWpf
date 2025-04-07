using Hashi.Generator.Interfaces.Models;

namespace Hashi.Generator.Models;

/// <summary>
///     Represents an edge between two nodes.
/// </summary>
public class Bridge : IBridge
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Bridge" /> class.
    /// </summary>
    /// <param name="island1">The first island.</param>
    /// <param name="island2">The second island.</param>
    /// <param name="amountBridgesSet">The amount of bridges set between two the two islands.</param>
    public Bridge(IIsland island1, IIsland island2, int amountBridgesSet)
    {
        Island1 = island1;
        Island2 = island2;
        AmountBridgesSet = amountBridgesSet;
    }

    /// <summary>
    ///     Gets the first island of the bridge.
    /// </summary>
    public IIsland Island1 { get; }

    /// <summary>
    ///     Gets the second island of the bridge.
    /// </summary>
    public IIsland Island2 { get; }

    /// <summary>
    ///     Gets the amount of bridges set between the two islands.
    /// </summary>
    public int AmountBridgesSet { get; private set; }

    /// <summary>
    ///     Adds the other side of the bridge.
    /// </summary>
    /// <returns>the added bridge.</returns>
    public IBridge AddOtherSide()
    {
        var otherSideBridge = new Bridge(Island2, Island1, AmountBridgesSet);
        if (Island1.X == Island2.X && Island1.Y > Island2.Y)
        {
            Island1.AmountBridgesUp += AmountBridgesSet;
            Island2.AmountBridgesDown += AmountBridgesSet;
        }

        if (Island1.X == Island2.X && Island1.Y < Island2.Y)
        {
            Island1.AmountBridgesDown += AmountBridgesSet;
            Island2.AmountBridgesUp += AmountBridgesSet;
        }

        if (Island1.X > Island2.X && Island1.Y == Island2.Y)
        {
            Island1.AmountBridgesLeft += AmountBridgesSet;
            Island2.AmountBridgesRight += AmountBridgesSet;
        }

        if (Island1.X < Island2.X && Island1.Y == Island2.Y)
        {
            Island1.AmountBridgesRight += AmountBridgesSet;
            Island2.AmountBridgesLeft += AmountBridgesSet;
        }

        return otherSideBridge;
    }

    /// <summary>
    ///     Adds another bridge to this bridge.
    /// </summary>
    /// <param name="mainField">The main field array.</param>
    public void AddBridge(int[][] mainField)
    {
        AmountBridgesSet++;
        if (Island1.IslandUp == Island2)
        {
            Island1.AmountBridgesUp = AmountBridgesSet;
            Island2.AmountBridgesDown = AmountBridgesSet;
        }

        if (Island1.IslandDown == Island2)
        {
            Island1.AmountBridgesDown = AmountBridgesSet;
            Island2.AmountBridgesUp = AmountBridgesSet;
        }

        if (Island1.IslandLeft == Island2)
        {
            Island1.AmountBridgesLeft = AmountBridgesSet;
            Island2.AmountBridgesRight = AmountBridgesSet;
        }

        if (Island1.IslandRight == Island2)
        {
            Island1.AmountBridgesRight = AmountBridgesSet;
            Island2.AmountBridgesLeft = AmountBridgesSet;
        }

        Island1.AmountBridgesConnectable += 1;
        Island2.AmountBridgesConnectable += 1;
        mainField[Island1.Y][Island1.X]++;
        mainField[Island2.Y][Island2.X]++;
    }
}