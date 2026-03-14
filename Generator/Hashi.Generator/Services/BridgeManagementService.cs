using Hashi.Enums;
using Hashi.Generator.Extensions;
using Hashi.Generator.Interfaces;
using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Models;
using System.Drawing;

namespace Hashi.Generator.Services;

/// <inheritdoc />
public class BridgeManagementService : IBridgeManagementService
{
    private readonly Func<IIsland, IIsland, int, IBridge> bridgeFactory;
    private readonly BlockDetectionService blockDetectionService;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BridgeManagementService" /> class.
    /// </summary>
    public BridgeManagementService(
        Func<IIsland, IIsland, int, IBridge> bridgeFactory,
        BlockDetectionService blockDetectionService)
    {
        this.bridgeFactory = bridgeFactory;
        this.blockDetectionService = blockDetectionService;
    }

    /// <inheritdoc />
    public void AddAdditionalBridges(int[][] mainField, int alpha, IList<IIsland> islands, IList<IBridge> bridges)
    {
        var bridgesAdded = 0;
        var targetBridges = (int)(islands.Count * (alpha / 100.0));

        var islandIndices = new int[islands.Count];
        for (var i = 0; i < islandIndices.Length; i++)
        {
            islandIndices[i] = i;
        }

        islandIndices.Shuffle();

        foreach (var idx in islandIndices)
        {
            if (bridgesAdded >= targetBridges)
            {
                break;
            }

            var island = islands[idx];

            if (TryAddBridge(island, DirectionEnum.Down, mainField, bridges))
            {
                bridgesAdded++;
                if (bridgesAdded >= targetBridges)
                {
                    break;
                }
            }

            if (TryAddBridge(island, DirectionEnum.Right, mainField, bridges))
            {
                bridgesAdded++;
            }
        }
    }

    /// <inheritdoc />
    public void SetBeta(int[][] mainField, int beta, IList<IBridge> bridges)
    {
        if (beta <= 0)
        {
            return;
        }

        var bridgesToAdd = (int)Math.Ceiling(bridges.Count * GeneratorConstants.BetaScalingFactor * (beta / 100.0));
        if (bridgesToAdd <= 0)
        {
            return;
        }

        var candidates = new List<int>(bridges.Count / 2);
        for (var i = bridges.Count - 1; i > 0; i -= 2)
        {
            candidates.Add(i);
        }

        candidates.Shuffle();

        for (var i = 0; i < Math.Min(bridgesToAdd, candidates.Count); i++)
        {
            var bridge = bridges[candidates[i]];
            bridge.AddBridge();
            mainField[bridge.Island1.Y][bridge.Island1.X]++;
            mainField[bridge.Island2.Y][bridge.Island2.X]++;
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<IBridgeCoordinates> BuildBridgeCoordinates(IList<IBridge> bridges)
    {
        var bridgeCoordinates = new List<IBridgeCoordinates>(bridges.Count);
        foreach (var bridge in bridges)
        {
            bridgeCoordinates.Add(new BridgeCoordinates(
                new Point(bridge.Island1.X, bridge.Island1.Y),
                new Point(bridge.Island2.X, bridge.Island2.Y),
                bridge.AmountBridgesSet));
        }

        return bridgeCoordinates;
    }

    internal bool TryAddBridge(IIsland island, DirectionEnum direction, int[][] mainField, IList<IBridge> bridges)
    {
        var (neighbor, bridgeCount, isBlocked) = direction switch
        {
            DirectionEnum.Down => (island.IslandDown, island.AmountBridgesDown,
                blockDetectionService.GetDownBlockedBetween(island, mainField, bridges) != -1),
            DirectionEnum.Right => (island.IslandRight, island.AmountBridgesRight,
                blockDetectionService.GetRightBlockedBetween(island, mainField, bridges) != -1),
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, @"Only Down and Right are supported.")
        };

        if (neighbor == null ||
            bridgeCount > 0 ||
            island.AmountBridgesConnectable + 1 > GeneratorConstants.MaxConnectableBridgesForAdding ||
            neighbor.AmountBridgesConnectable + 1 > GeneratorConstants.MaxConnectableBridgesForAdding ||
            isBlocked)
        {
            return false;
        }

        var bridge = bridgeFactory.Invoke(island, neighbor, 1);
        bridges.Add(bridge);
        bridges.Add(bridge.CreateReverseBridgeAndApplyDirections());

        island.IncrementAmountBridgesConnectable(1);
        neighbor.IncrementAmountBridgesConnectable(1);
        mainField[island.Y][island.X] += 1;
        mainField[neighbor.Y][neighbor.X] += 1;

        blockDetectionService.UpdateDirectionCache(island, neighbor);

        return true;
    }

}
