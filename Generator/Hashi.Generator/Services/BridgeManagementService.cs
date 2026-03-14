using Hashi.Enums;
using Hashi.Generator.Interfaces;
using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Models;
using System.Drawing;

namespace Hashi.Generator.Services;

/// <inheritdoc />
public class BridgeManagementService : IBridgeManagementService
{
    private readonly Func<IIsland, IIsland, int, IBridge> bridgeFactory;
    private readonly IBlockDetectionService blockDetectionService;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BridgeManagementService" /> class.
    /// </summary>
    public BridgeManagementService(
        Func<IIsland, IIsland, int, IBridge> bridgeFactory,
        IBlockDetectionService blockDetectionService)
    {
        this.bridgeFactory = bridgeFactory;
        this.blockDetectionService = blockDetectionService;
    }

    /// <inheritdoc />
    public void AddAdditionalBridges(int[][] mainField, int alpha, List<IIsland> islands, List<IBridge> bridges)
    {
        var bridgesAdded = 0;
        var targetBridges = (int)(islands.Count * (alpha / 100.0));

        var islandIndices = new int[islands.Count];
        for (var i = 0; i < islandIndices.Length; i++)
        {
            islandIndices[i] = i;
        }

        Shuffle(islandIndices);

        foreach (var idx in islandIndices)
        {
            if (bridgesAdded >= targetBridges)
            {
                break;
            }

            var island = islands[idx];

            if (TryAddBridgeDown(island, mainField, bridges))
            {
                bridgesAdded++;
                if (bridgesAdded >= targetBridges)
                {
                    break;
                }
            }

            if (TryAddBridgeRight(island, mainField, bridges))
            {
                bridgesAdded++;
            }
        }
    }

    /// <inheritdoc />
    public void SetBeta(int[][] mainField, int beta, List<IBridge> bridges)
    {
        if (beta <= 0)
        {
            return;
        }

        var bridgesToAdd = (int)Math.Ceiling(bridges.Count * 0.5 * (beta / 100.0));
        if (bridgesToAdd <= 0)
        {
            return;
        }

        var candidates = new List<int>(bridges.Count / 2);
        for (var i = bridges.Count - 1; i > 0; i -= 2)
        {
            candidates.Add(i);
        }

        Shuffle(candidates);

        for (var i = 0; i < Math.Min(bridgesToAdd, candidates.Count); i++)
        {
            bridges[candidates[i]].AddBridge(mainField);
        }
    }

    /// <inheritdoc />
    public List<IBridgeCoordinates> BuildBridgeCoordinates(List<IBridge> bridges)
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

    internal bool TryAddBridgeDown(IIsland island, int[][] mainField, IList<IBridge> bridges)
    {
        if (island.IslandDown == null ||
            island.AmountBridgesDown > 0 ||
            island.AmountBridgesConnectable + 1 > GeneratorConstants.MaxConnectableBridgesForAdding ||
            island.IslandDown.AmountBridgesConnectable + 1 > GeneratorConstants.MaxConnectableBridgesForAdding ||
            blockDetectionService.GetDownBlockedBetween(island, mainField, bridges) != -1)
        {
            return false;
        }

        var bridge = bridgeFactory.Invoke(island, island.IslandDown, 1);
        bridges.Add(bridge);
        bridges.Add(bridge.CreateReverseBridgeAndApplyDirections());

        island.AmountBridgesConnectable += 1;
        island.IslandDown.AmountBridgesConnectable += 1;
        mainField[island.Y][island.X] += 1;
        mainField[island.IslandDown.Y][island.X] += 1;

        blockDetectionService.UpdateDirectionCache(island, island.IslandDown);

        return true;
    }

    internal bool TryAddBridgeRight(IIsland island, int[][] mainField, IList<IBridge> bridges)
    {
        if (island.IslandRight == null ||
            island.AmountBridgesRight > 0 ||
            island.AmountBridgesConnectable + 1 > GeneratorConstants.MaxConnectableBridgesForAdding ||
            island.IslandRight.AmountBridgesConnectable + 1 > GeneratorConstants.MaxConnectableBridgesForAdding ||
            blockDetectionService.GetRightBlockedBetween(island, mainField, bridges) != -1)
        {
            return false;
        }

        var bridge = bridgeFactory.Invoke(island, island.IslandRight, 1);
        bridges.Add(bridge);
        bridges.Add(bridge.CreateReverseBridgeAndApplyDirections());

        island.AmountBridgesConnectable += 1;
        island.IslandRight.AmountBridgesConnectable += 1;
        mainField[island.Y][island.X] += 1;
        mainField[island.IslandRight.Y][island.IslandRight.X] += 1;

        blockDetectionService.UpdateDirectionCache(island, island.IslandRight);

        return true;
    }

    internal static void Shuffle<T>(IList<T> collection)
    {
        for (var i = collection.Count - 1; i > 0; i--)
        {
            var j = Random.Shared.Next(i + 1);
            (collection[i], collection[j]) = (collection[j], collection[i]);
        }
    }
}
