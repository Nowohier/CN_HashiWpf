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

    /// <inheritdoc />
    public IBridge CreateReverseBridgeAndApplyDirections()
    {
        var otherSideBridge = new Bridge(Island2, Island1, AmountBridgesSet);
        ApplyDirectionBridgeCounts(Island1, Island2, AmountBridgesSet);
        return otherSideBridge;
    }

    /// <inheritdoc />
    public void AddBridge()
    {
        AmountBridgesSet++;
        ApplyDirectionBridgeCountsByNeighbor(Island1, Island2, AmountBridgesSet);

        Island1.IncrementAmountBridgesConnectable(1);
        Island2.IncrementAmountBridgesConnectable(1);
    }

    /// <summary>
    ///     Determines the direction between two islands by coordinate comparison and sets bridge counts.
    ///     Used by <see cref="CreateReverseBridgeAndApplyDirections"/> where neighbor references are not yet established.
    /// </summary>
    private static void ApplyDirectionBridgeCounts(IIsland source, IIsland target, int bridgeCount)
    {
        if (source.X == target.X)
        {
            if (source.Y > target.Y)
            {
                source.IncrementAmountBridgesUp(bridgeCount);
                target.IncrementAmountBridgesDown(bridgeCount);
            }
            else if (source.Y < target.Y)
            {
                source.IncrementAmountBridgesDown(bridgeCount);
                target.IncrementAmountBridgesUp(bridgeCount);
            }
        }
        else if (source.Y == target.Y)
        {
            if (source.X > target.X)
            {
                source.IncrementAmountBridgesLeft(bridgeCount);
                target.IncrementAmountBridgesRight(bridgeCount);
            }
            else
            {
                source.IncrementAmountBridgesRight(bridgeCount);
                target.IncrementAmountBridgesLeft(bridgeCount);
            }
        }
    }

    /// <summary>
    ///     Determines the direction between two islands by neighbor reference and sets bridge counts.
    ///     Used by <see cref="AddBridge"/> where neighbor references are already established.
    /// </summary>
    private static void ApplyDirectionBridgeCountsByNeighbor(IIsland source, IIsland target, int bridgeCount)
    {
        if (source.IslandUp == target)
        {
            source.SetAmountBridgesUp(bridgeCount);
            target.SetAmountBridgesDown(bridgeCount);
        }
        else if (source.IslandDown == target)
        {
            source.SetAmountBridgesDown(bridgeCount);
            target.SetAmountBridgesUp(bridgeCount);
        }
        else if (source.IslandLeft == target)
        {
            source.SetAmountBridgesLeft(bridgeCount);
            target.SetAmountBridgesRight(bridgeCount);
        }
        else if (source.IslandRight == target)
        {
            source.SetAmountBridgesRight(bridgeCount);
            target.SetAmountBridgesLeft(bridgeCount);
        }
    }
}