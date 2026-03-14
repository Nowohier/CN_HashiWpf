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

    /// <summary>
    ///     Adds another bridge to this bridge.
    /// </summary>
    /// <param name="mainField">The main field array.</param>
    public void AddBridge(int[][] mainField)
    {
        AmountBridgesSet++;
        ApplyDirectionBridgeCountsByNeighbor(Island1, Island2, AmountBridgesSet);

        Island1.AmountBridgesConnectable += 1;
        Island2.AmountBridgesConnectable += 1;
        mainField[Island1.Y][Island1.X]++;
        mainField[Island2.Y][Island2.X]++;
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
                source.AmountBridgesUp += bridgeCount;
                target.AmountBridgesDown += bridgeCount;
            }
            else if (source.Y < target.Y)
            {
                source.AmountBridgesDown += bridgeCount;
                target.AmountBridgesUp += bridgeCount;
            }
        }
        else if (source.Y == target.Y)
        {
            if (source.X > target.X)
            {
                source.AmountBridgesLeft += bridgeCount;
                target.AmountBridgesRight += bridgeCount;
            }
            else
            {
                source.AmountBridgesRight += bridgeCount;
                target.AmountBridgesLeft += bridgeCount;
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
            source.AmountBridgesUp = bridgeCount;
            target.AmountBridgesDown = bridgeCount;
        }
        else if (source.IslandDown == target)
        {
            source.AmountBridgesDown = bridgeCount;
            target.AmountBridgesUp = bridgeCount;
        }
        else if (source.IslandLeft == target)
        {
            source.AmountBridgesLeft = bridgeCount;
            target.AmountBridgesRight = bridgeCount;
        }
        else if (source.IslandRight == target)
        {
            source.AmountBridgesRight = bridgeCount;
            target.AmountBridgesLeft = bridgeCount;
        }
    }
}